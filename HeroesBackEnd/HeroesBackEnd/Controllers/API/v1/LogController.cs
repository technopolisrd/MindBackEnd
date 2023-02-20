using Microsoft.AspNetCore.Mvc;
using Mind.Entity.SecurityAccount.Enum;
using Mind.Entity.Tables.DTO;
using Mind.Repository.Mind.Service.Contracts;
using MindBackEnd.Controllers.Security.v1;
using MindBackEnd.Authorization;

namespace MindBackEnd.Controllers.API.v1;

#nullable disable

[Authorize]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class LogController : BaseController
{

    private iLogMovementService serv;

    public LogController(iLogMovementService _serv)
    {
        this.serv = _serv;
    }

    [Authorize(Role.Admin, Role.User, Role.SuperUser)]
    [HttpGet]
    [ProducesResponseType(200, Type = typeof(ResponseDTO<List<LogMovementsDTO>>))]
    [Route("GetLogs")]
    public async Task<IActionResult> GetLogs()
    {
        ResponseDTO<List<LogMovementsDTO>> respuesta = new ResponseDTO<List<LogMovementsDTO>>();

        var result = await serv.GetAll();

        if (result != null)
        {
            respuesta.status = "200";
            respuesta.message = "Datos cargados correctamente.";
            respuesta.data = result;

            return Ok(respuesta);
        }

        respuesta.status = "404";
        respuesta.message = "Datos no encontrados.";
        respuesta.data = null;

        return Ok(respuesta);
    }

}
