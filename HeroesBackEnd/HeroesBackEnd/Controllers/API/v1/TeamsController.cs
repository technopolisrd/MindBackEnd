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
public class TeamsController : BaseController
{

    private iTeamService serv;

    public TeamsController(iTeamService _serv)
    {
        this.serv = _serv;
    }

    [Authorize(Role.Admin, Role.User, Role.SuperUser)]
    [HttpGet]
    [ProducesResponseType(200, Type = typeof(ResponseDTO<List<TeamDTO>>))]
    [Route("GetTeams")]
    public async Task<IActionResult> GetTeams()
    {
        ResponseDTO<List<TeamDTO>> respuesta = new ResponseDTO<List<TeamDTO>>();

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

    [Authorize(Role.Admin, Role.User, Role.SuperUser)]
    [HttpGet]
    [ProducesResponseType(200, Type = typeof(ResponseDTO<List<DropDownDTO>>))]
    [Route("GetTeamsForDropDown")]
    public async Task<IActionResult> GetTeamsForDropDown()
    {
        ResponseDTO<List<DropDownDTO>> respuesta = new ResponseDTO<List<DropDownDTO>>();

        var result = await serv.GetAllForDropDown();

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

    [Authorize(Role.Admin, Role.User, Role.SuperUser)]
    [HttpGet("GetTeamBySearch/{searchString}", Name = nameof(GetTeamBySearch))]
    [ProducesResponseType(200, Type = typeof(ResponseDTO<List<TeamDTO>>))]
    public async Task<IActionResult> GetTeamBySearch(string searchString)
    {
        ResponseDTO<List<TeamDTO>> respuesta = new ResponseDTO<List<TeamDTO>>();

        var result = await serv.GetAllBySearch(searchString);

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

    [Authorize(Role.Admin, Role.User, Role.SuperUser)]
    [HttpGet("GetTeamById/{id}", Name = nameof(GetTeamById))]
    [ProducesResponseType(200, Type = typeof(ResponseDTO<List<TeamDTO>>))]
    public async Task<IActionResult> GetTeamById(int id)
    {
        ResponseDTO<List<TeamDTO>> respuesta = new ResponseDTO<List<TeamDTO>>();

        var result = await serv.GetDataById(id);
        List<TeamDTO> resultList = new List<TeamDTO>
        {
            result
        };

        if (result != null)
        {
            respuesta.status = "200";
            respuesta.message = "Datos cargados correctamente.";
            respuesta.data = resultList;

            return Ok(respuesta);
        }

        respuesta.status = "404";
        respuesta.message = "Datos no encontrados.";
        respuesta.data = null;

        return Ok(respuesta);
    }

    [Authorize(Role.Admin, Role.User, Role.SuperUser)]
    [HttpPost("CreateTeam/{user}", Name = nameof(CreateTeam))]
    [ProducesResponseType(201, Type = typeof(ResponseDTO<List<TeamDTO>>))]
    public async Task<IActionResult> CreateTeam([FromBody] TeamDTO team, int user)
    {
        ResponseDTO<List<TeamDTO>> respuesta = new ResponseDTO<List<TeamDTO>>();

        if (team == null || user == 0)
        {
            respuesta.status = "400";
            respuesta.message = "Solicitud incorrecta, falta uno de los parámetros.";
            respuesta.data = null;

            return Ok(respuesta);
        }

        if (!ModelState.IsValid)
        {
            respuesta.status = "400";
            respuesta.message = "Modelo de datos inválido.";
            respuesta.data = null;

            return Ok(respuesta);
        }

        TeamDTO added = await serv.AddAsync(team, user);
        List<TeamDTO> addedList = new List<TeamDTO>
        {
            added
        };

        if (added != null)
        {
            respuesta.status = "201";
            respuesta.message = "Datos guardados correctamente.";
            respuesta.data = addedList;

            return Ok(respuesta);
        }

        respuesta.status = "404";
        respuesta.message = "Datos no guardados.";
        respuesta.data = null;

        return Ok(respuesta);

    }

    [Authorize(Role.Admin, Role.User, Role.SuperUser)]
    [HttpPut("UpdateTeam/{user}", Name = nameof(UpdateTeam))]
    [ProducesResponseType(204, Type = typeof(ResponseDTO<List<TeamDTO>>))]
    public async Task<IActionResult> UpdateTeam([FromBody] TeamDTO team, int user)
    {
        ResponseDTO<List<TeamDTO>> respuesta = new ResponseDTO<List<TeamDTO>>();

        if (team == null || team.ID == "0" || user == 0)
        {
            respuesta.status = "400";
            respuesta.message = "Solicitud incorrecta.";
            respuesta.data = null;

            return Ok(respuesta);
        }

        if (!ModelState.IsValid)
        {
            respuesta.status = "400";
            respuesta.message = "Modelo de datos inválido.";
            respuesta.data = null;

            return Ok(respuesta);
        }

        TeamDTO result = await serv.UpdateAsync(team, user);
        List<TeamDTO> resultList = new List<TeamDTO>
        {
            result
        };

        if (result != null)
        {
            if (result.ID == "0")
            {
                respuesta.status = "404";
                respuesta.message = "Registro no encontrado.";
                respuesta.data = null;

                return Ok(respuesta);
            }

            respuesta.status = "204";
            respuesta.message = "Datos guardados correctamente.";
            respuesta.data = resultList;

            return Ok(respuesta);
        }

        respuesta.status = "404";
        respuesta.message = "Cambios no guardados.";
        respuesta.data = null;

        return Ok(respuesta);

    }

    [Authorize(Role.Admin, Role.User, Role.SuperUser)]
    [HttpDelete("DeleteTeam/{id}", Name = nameof(DeleteTeam))]
    [ProducesResponseType(204, Type = typeof(ResponseDTO<List<TeamDTO>>))]
    public async Task<IActionResult> DeleteTeam(int id, int user)
    {
        ResponseDTO<List<TeamDTO>> respuesta = new ResponseDTO<List<TeamDTO>>();

        if (id <= 0 || user == 0)
        {
            respuesta.status = "400";
            respuesta.message = "Solicitud incorrecta.";
            respuesta.data = null;

            return Ok(respuesta);
        }

        TeamDTO result = await serv.RemoveAsync(id, user);
        List<TeamDTO> resultList = new List<TeamDTO>
        {
            result
        };

        if (result != null)
        {
            if (result.ID == "0")
            {
                respuesta.status = "404";
                respuesta.message = "Registro no encontrado.";
                respuesta.data = null;

                return Ok(respuesta);
            }

            respuesta.status = "204";
            respuesta.message = "Datos borrados correctamente.";
            respuesta.data = resultList;

            return Ok(respuesta);
        }

        respuesta.status = "404";
        respuesta.message = "Cambios no guardados.";
        respuesta.data = null;

        return Ok(respuesta);
    }

}
