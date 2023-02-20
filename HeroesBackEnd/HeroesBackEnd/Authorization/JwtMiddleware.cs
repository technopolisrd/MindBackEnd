namespace MindBackEnd.Authorization;

using Microsoft.Extensions.Options;
using Mind.Context.Data.Security;
using MindBackEnd.Authorization.Contracts;
using MindBackEnd.Helpers;

#nullable disable

public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly AppSettings _appSettings;

    public JwtMiddleware(RequestDelegate next, IOptions<AppSettings> appSettings)
    {
        _next = next;
        _appSettings = appSettings.Value;
    }

    public async Task Invoke(HttpContext context, MindContext dataContext, IJwtUtils jwtUtils)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        var accountId = jwtUtils.ValidateJwtToken(token);
        if (accountId != null)
        {
            context.Items["Account"] = await dataContext.Accounts.FindAsync(accountId.Value);
        }

        await _next(context);
    }
}
