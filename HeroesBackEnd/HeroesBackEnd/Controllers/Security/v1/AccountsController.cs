namespace MindBackEnd.Controllers.Security.v1;

using Microsoft.AspNetCore.Mvc;
using Mind.Entity.Account;
using Mind.Entity.SecurityAccount.Enum;
using Mind.Entity.SecurityAccount;
using MindBackEnd.Services.Contracts;
using MindBackEnd.Authorization;

#nullable disable

[Authorize]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class AccountsController : BaseController
{
    private readonly IAccountService _accountService;

    public AccountsController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [AllowAnonymous]
    [HttpPost("authenticate")]
    public ActionResult<AuthenticateResponse> Authenticate(AuthenticateRequest model)
    {
        var response = _accountService.Authenticate(model, ipAddress());
        setTokenCookie(response.RefreshToken);
        return Ok(response);
    }

    [AllowAnonymous]
    [HttpPost("refresh-token")]
    public ActionResult<AuthenticateResponse> RefreshToken()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        var response = _accountService.RefreshToken(refreshToken, ipAddress());
        setTokenCookie(response.RefreshToken);
        return Ok(response);
    }

    [HttpPost("revoke-token")]
    public IActionResult RevokeToken(RevokeTokenRequest model)
    {
        var token = model.Token ?? Request.Cookies["refreshToken"];

        if (string.IsNullOrEmpty(token))
            return BadRequest(new { message = "Se requiere Token" });

        if (!Account.OwnsToken(token) && Account.Role != Role.Admin)
            return Unauthorized(new { message = "Sin autorización" });

        _accountService.RevokeToken(token, ipAddress());
        return Ok(new { message = "Token revocado" });
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public IActionResult Register(RegisterRequest model)
    {
        _accountService.Register(model, Request.Headers["origin"]);
        return Ok(new { message = "Registro realizado con éxito, por favor, compruebe su correo electrónico para obtener instrucciones de verificación" });
    }

    [AllowAnonymous]
    [HttpPost("verify-email")]
    public IActionResult VerifyEmail(VerifyEmailRequest model)
    {
        _accountService.VerifyEmail(model.Token);
        return Ok(new { message = "Verificación correcta, ahora puede iniciar sesión" });
    }

    [AllowAnonymous]
    [HttpPost("forgot-password")]
    public IActionResult ForgotPassword(ForgotPasswordRequest model)
    {
        _accountService.ForgotPassword(model, Request.Headers["origin"]);
        return Ok(new { message = "Consulte su correo electrónico para obtener instrucciones sobre cómo restablecer la contraseña" });
    }

    [AllowAnonymous]
    [HttpPost("validate-reset-token")]
    public IActionResult ValidateResetToken(ValidateResetTokenRequest model)
    {
        _accountService.ValidateResetToken(model);
        return Ok(new { message = "Token es valido" });
    }

    [AllowAnonymous]
    [HttpPost("reset-password")]
    public IActionResult ResetPassword(ResetPasswordRequest model)
    {
        _accountService.ResetPassword(model);
        return Ok(new { message = "Se ha restablecido la contraseña, ya puede iniciar sesión" });
    }

    [Authorize(Role.Admin, Role.SuperUser)]
    [HttpGet]
    public ActionResult<IEnumerable<AccountResponse>> GetAll()
    {
        var accounts = _accountService.GetAll();
        return Ok(accounts);
    }

    [HttpGet("{id:int}")]
    public ActionResult<AccountResponse> GetById(int id)
    {
        if (id != Account.Id && Account.Role != Role.Admin)
            return Unauthorized(new { message = "Sin autorización" });

        var account = _accountService.GetById(id);
        return Ok(account);
    }

    [Authorize(Role.Admin, Role.SuperUser)]
    [HttpPost]
    public ActionResult<AccountResponse> Create(CreateRequest model)
    {
        var account = _accountService.Create(model);
        return Ok(account);
    }

    [HttpPut("{id:int}")]
    public ActionResult<AccountResponse> Update(int id, UpdateRequest model)
    {
        if (id != Account.Id && Account.Role == Role.User)
            return Unauthorized(new { message = "Sin autorización" });

        if (Account.Role == Role.User)
            model.Role = null;

        var account = _accountService.Update(id, model);
        return Ok(account);
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        if (id != Account.Id && Account.Role == Role.User)
            return Unauthorized(new { message = "Sin autorización" });

        _accountService.Delete(id);
        return Ok(new { message = "Cuenta eliminada correctamente" });
    }

    private void setTokenCookie(string token)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = DateTime.UtcNow.AddDays(7)
        };
        Response.Cookies.Append("refreshToken", token, cookieOptions);
    }

    private string ipAddress()
    {
        if (Request.Headers.ContainsKey("X-Forwarded-For"))
            return Request.Headers["X-Forwarded-For"];
        else
            return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
    }
}