namespace MindBackEnd.Controllers.Security.v1;

using Microsoft.AspNetCore.Mvc;
using Mind.Entity.SecurityAccount;

#nullable disable

[Controller]
public abstract class BaseController : ControllerBase
{
    public Account Account => (Account)HttpContext.Items["Account"];
}