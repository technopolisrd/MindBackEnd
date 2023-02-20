namespace MindBackEnd.Authorization;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Mind.Entity.SecurityAccount.Enum;
using Mind.Entity.SecurityAccount;

#nullable disable

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeAttribute : Attribute, IAuthorizationFilter
{
    private readonly IList<Role> _roles;

    public AuthorizeAttribute(params Role[] roles)
    {
        _roles = roles ?? new Role[] { };
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
        if (allowAnonymous)
            return;

        var account = (Account)context.HttpContext.Items["Account"];
        if (account == null || (_roles.Any() && !_roles.Contains(account.Role)))
        {
            context.Result = new JsonResult(new { message = "Sin autorización" }) { StatusCode = StatusCodes.Status401Unauthorized };
        }
    }
}
