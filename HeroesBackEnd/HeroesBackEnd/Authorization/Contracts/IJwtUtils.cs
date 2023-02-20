using Mind.Entity.SecurityAccount;

namespace MindBackEnd.Authorization.Contracts;

public interface IJwtUtils
{
    public string GenerateJwtToken(Account account);
    public int? ValidateJwtToken(string token);
    public RefreshToken GenerateRefreshToken(string ipAddress);
}
