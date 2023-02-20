namespace MindBackEnd.Services.Services;

using AutoMapper;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Mind.Entity.Account;
using Mind.Entity.SecurityAccount.Enum;
using Mind.Entity.SecurityAccount;
using MindBackEnd.Helpers;
using MindBackEnd.Services.Contracts;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Mind.Context.Data.Security;
using MindBackEnd.Authorization.Contracts;
using BCrypt.Net;
using Mind.Business.Log;
using Mind.Business.Data;

#nullable disable

public class AccountService : IAccountService
{
    private readonly MindContext _context;
    private readonly IJwtUtils _jwtUtils;
    private readonly IMapper _mapper;
    private readonly AppSettings _appSettings;
    private readonly IEmailService _emailService;

    public AccountService(
        MindContext context,
        IJwtUtils jwtUtils,
        IMapper mapper,
        IOptions<AppSettings> appSettings,
        IEmailService emailService)
    {
        _context = context;
        _jwtUtils = jwtUtils;
        _mapper = mapper;
        _appSettings = appSettings.Value;
        _emailService = emailService;
    }

    private LogData log;

    public AuthenticateResponse Authenticate(AuthenticateRequest model, string ipAddress)
    {
        var account = _context.Accounts.SingleOrDefault(x => x.Email == model.Email);

        if (account == null || !account.IsVerified || !BCrypt.Verify(model.Password, account.PasswordHash))
        {
            log = new LogData();
            if (account != null)
            {
                log.email = account.Email;
            }
            log.Message = "El correo electrónico o la contraseña son incorrectos";
            log.action = "Autenticar usuario";
            log.logDate = System.DateTime.Now;

            Errors.SendLog(log);

            throw new AppException("El correo electrónico o la contraseña son incorrectos");
        }

        var jwtToken = _jwtUtils.GenerateJwtToken(account);
        var refreshToken = _jwtUtils.GenerateRefreshToken(ipAddress);
        account.RefreshTokens.Add(refreshToken);

        removeOldRefreshTokens(account);

        _context.Update(account);
        _context.SaveChanges();

        var response = _mapper.Map<AuthenticateResponse>(account);
        response.JwtToken = jwtToken;
        response.RefreshToken = refreshToken.Token;
        return response;
    }

    public AuthenticateResponse RefreshToken(string token, string ipAddress)
    {
        var account = getAccountByRefreshToken(token);
        var refreshToken = account.RefreshTokens.Single(x => x.Token == token);

        if (refreshToken.IsRevoked)
        {
            revokeDescendantRefreshTokens(refreshToken, account, ipAddress, $"Intento de reutilización de un token con ancestro revocado: {token}");
            _context.Update(account);
            _context.SaveChanges();
        }

        if (!refreshToken.IsActive)
            throw new AppException("Token no válido");

        var newRefreshToken = rotateRefreshToken(refreshToken, ipAddress);
        account.RefreshTokens.Add(newRefreshToken);

        removeOldRefreshTokens(account);

        _context.Update(account);
        _context.SaveChanges();

        var jwtToken = _jwtUtils.GenerateJwtToken(account);

        var response = _mapper.Map<AuthenticateResponse>(account);
        response.JwtToken = jwtToken;
        response.RefreshToken = newRefreshToken.Token;
        return response;
    }

    public void RevokeToken(string token, string ipAddress)
    {
        var account = getAccountByRefreshToken(token);
        var refreshToken = account.RefreshTokens.Single(x => x.Token == token);

        if (!refreshToken.IsActive)
            throw new AppException("Token no válido");

        revokeRefreshToken(refreshToken, ipAddress, "Revocada sin sustitución");
        _context.Update(account);
        _context.SaveChanges();
    }

    public void Register(RegisterRequest model, string origin)
    {
        if (_context.Accounts.Any(x => x.Email == model.Email))
        {
            sendAlreadyRegisteredEmail(model.Email, origin);
            return;
        }

        var account = _mapper.Map<Account>(model);

        var isFirstAccount = _context.Accounts.Count() == 0;
        account.Role = isFirstAccount ? Role.Admin : Role.User;
        account.Created = DateTime.UtcNow;
        account.VerificationToken = generateVerificationToken();

        account.PasswordHash = BCrypt.HashPassword(model.Password);

        _context.Accounts.Add(account);
        _context.SaveChanges();

        sendVerificationEmail(account, origin);
    }

    public void VerifyEmail(string token)
    {
        var account = _context.Accounts.SingleOrDefault(x => x.VerificationToken == token);

        if (account == null)
            throw new AppException("Verificación fallida");

        account.Verified = DateTime.UtcNow;
        account.VerificationToken = null;

        _context.Accounts.Update(account);
        _context.SaveChanges();
    }

    public void ForgotPassword(ForgotPasswordRequest model, string origin)
    {
        var account = _context.Accounts.SingleOrDefault(x => x.Email == model.Email);

        if (account == null) return;

        account.ResetToken = generateResetToken();
        account.ResetTokenExpires = DateTime.UtcNow.AddDays(1);

        _context.Accounts.Update(account);
        _context.SaveChanges();

        sendPasswordResetEmail(account, origin);
    }

    public void ValidateResetToken(ValidateResetTokenRequest model)
    {
        getAccountByResetToken(model.Token);
    }

    public void ResetPassword(ResetPasswordRequest model)
    {
        var account = getAccountByResetToken(model.Token);

        account.PasswordHash = BCrypt.HashPassword(model.Password);
        account.PasswordReset = DateTime.UtcNow;
        account.ResetToken = null;
        account.ResetTokenExpires = null;

        _context.Accounts.Update(account);
        _context.SaveChanges();
    }

    public IEnumerable<AccountResponse> GetAll()
    {
        var accounts = _context.Accounts;
        return _mapper.Map<IList<AccountResponse>>(accounts);
    }

    public AccountResponse GetById(int id)
    {
        var account = getAccount(id);
        return _mapper.Map<AccountResponse>(account);
    }

    public AccountResponse Create(CreateRequest model)
    {
        if (_context.Accounts.Any(x => x.Email == model.Email))
            throw new AppException($"Correo '{model.Email}' ya está registrado");

        var account = _mapper.Map<Account>(model);
        account.Created = DateTime.UtcNow;
        account.Verified = DateTime.UtcNow;

        account.PasswordHash = BCrypt.HashPassword(model.Password);

        _context.Accounts.Add(account);
        _context.SaveChanges();

        return _mapper.Map<AccountResponse>(account);
    }

    public AccountResponse Update(int id, UpdateRequest model)
    {
        var account = getAccount(id);

        if (account.Email != model.Email && _context.Accounts.Any(x => x.Email == model.Email))
            throw new AppException($"Correo '{model.Email}' ya está registrado");

        if (!string.IsNullOrEmpty(model.Password))
            account.PasswordHash = BCrypt.HashPassword(model.Password);

        _mapper.Map(model, account);
        account.Updated = DateTime.UtcNow;
        _context.Accounts.Update(account);
        _context.SaveChanges();

        return _mapper.Map<AccountResponse>(account);
    }

    public void Delete(int id)
    {
        var account = getAccount(id);
        _context.Accounts.Remove(account);
        _context.SaveChanges();
    }

    private Account getAccount(int id)
    {
        var account = _context.Accounts.Find(id);
        if (account == null) throw new KeyNotFoundException("Cuenta no encontrada");
        return account;
    }

    private Account getAccountByRefreshToken(string token)
    {
        var account = _context.Accounts.SingleOrDefault(u => u.RefreshTokens.Any(t => t.Token == token));
        if (account == null) throw new AppException("Token no válido");
        return account;
    }

    private Account getAccountByResetToken(string token)
    {
        var account = _context.Accounts.SingleOrDefault(x =>
            x.ResetToken == token && x.ResetTokenExpires > DateTime.UtcNow);
        if (account == null) throw new AppException("Token no válido");
        return account;
    }

    private string generateJwtToken(Account account)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim("id", account.Id.ToString()) }),
            Expires = DateTime.UtcNow.AddMinutes(15),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private string generateResetToken()
    {
        var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));

        var tokenIsUnique = !_context.Accounts.Any(x => x.ResetToken == token);
        if (!tokenIsUnique)
            return generateResetToken();

        return token;
    }

    private string generateVerificationToken()
    {
        var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));

        var tokenIsUnique = !_context.Accounts.Any(x => x.VerificationToken == token);
        if (!tokenIsUnique)
            return generateVerificationToken();

        return token;
    }

    private RefreshToken rotateRefreshToken(RefreshToken refreshToken, string ipAddress)
    {
        var newRefreshToken = _jwtUtils.GenerateRefreshToken(ipAddress);
        revokeRefreshToken(refreshToken, ipAddress, "Sustituido por un nuevo token", newRefreshToken.Token);
        return newRefreshToken;
    }

    private void removeOldRefreshTokens(Account account)
    {
        account.RefreshTokens.RemoveAll(x =>
            !x.IsActive &&
            x.Created.AddDays(_appSettings.RefreshTokenTTL) <= DateTime.UtcNow);
    }

    private void revokeDescendantRefreshTokens(RefreshToken refreshToken, Account account, string ipAddress, string reason)
    {
        if (!string.IsNullOrEmpty(refreshToken.ReplacedByToken))
        {
            var childToken = account.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken.ReplacedByToken);
            if (childToken.IsActive)
                revokeRefreshToken(childToken, ipAddress, reason);
            else
                revokeDescendantRefreshTokens(childToken, account, ipAddress, reason);
        }
    }

    private void revokeRefreshToken(RefreshToken token, string ipAddress, string reason = null, string replacedByToken = null)
    {
        token.Revoked = DateTime.UtcNow;
        token.RevokedByIp = ipAddress;
        token.ReasonRevoked = reason;
        token.ReplacedByToken = replacedByToken;
    }

    private void sendVerificationEmail(Account account, string origin)
    {
        string message;
        if (!string.IsNullOrEmpty(origin))
        {
            var verifyUrl = $"{origin}/account/verify-email?token={account.VerificationToken}";
            message = $@"<p>Haga clic en el siguiente enlace para verificar su dirección de correo electrónico:</p>
                            <p><a href=""{verifyUrl}"">{verifyUrl}</a></p>";
        }
        else
        {
            message = $@"<p>Utilice el token siguiente para verificar su dirección de correo electrónico con la ruta api <code>/accounts/verify-email</code>:</p>
                            <p><code>{account.VerificationToken}</code></p>";
        }

        _emailService.Send(
            to: account.Email,
            subject: "API de verificación de registro - Verificar correo electrónico",
            html: $@"<h4>Verificar correo electrónico</h4>
                        <p>Gracias por registrarse!</p>
                        {message}"
        );
    }

    private void sendAlreadyRegisteredEmail(string email, string origin)
    {
        string message;
        if (!string.IsNullOrEmpty(origin))
            message = $@"<p>Si no conoce su contraseña, visite la página <a href=""{origin}/account/forgot-password"">contraseña olvidada</a>.</p>";
        else
            message = "<p>Si no conoces tu contraseña puedes restablecerla a través de la ruta api <code>/accounts/forgot-password</code>.</p>";

        _emailService.Send(
            to: email,
            subject: "API de verificación de registro - Correo electrónico ya registrado",
            html: $@"<h4>Correo electrónico ya registrado</h4>
                        <p>Su correo electrónico <strong>{email}</strong> ya está registrado.</p>
                        {message}"
        );
    }

    private void sendPasswordResetEmail(Account account, string origin)
    {
        string message;
        if (!string.IsNullOrEmpty(origin))
        {
            var resetUrl = $"{origin}/account/reset-password?token={account.ResetToken}";
            message = $@"<p>Haga clic en el siguiente enlace para restablecer su contraseña, el enlace será válido durante 1 día:</p>
                            <p><a href=""{resetUrl}"">{resetUrl}</a></p>";
        }
        else
        {
            message = $@"<p>Por favor, utiliza el siguiente token para restablecer tu contraseña con la ruta api <code>/accounts/reset-password</code>:</p>
                            <p><code>{account.ResetToken}</code></p>";
        }

        _emailService.Send(
            to: account.Email,
            subject: "API de verificación de registro - Restablecer contraseña",
            html: $@"<h4>Restablecer contraseña</h4>
                        {message}"
        );
    }
}
