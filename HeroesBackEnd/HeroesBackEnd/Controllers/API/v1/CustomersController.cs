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
public class CustomersController : BaseController
{

    private iCustomerService serv;

    public CustomersController(iCustomerService _serv)
    {
        this.serv = _serv;
    }

    [Authorize(Role.Admin, Role.User, Role.SuperUser)]
    [HttpGet]
    [ProducesResponseType(200, Type = typeof(ResponseDTO<List<CustomerDTO>>))]
    [Route("GetCustomers")]
    public async Task<IActionResult> GetCustomers()
    {
        ResponseDTO<List<CustomerDTO>> respuesta = new ResponseDTO<List<CustomerDTO>>();

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
    [Route("GetCustomersForDropDown")]
    public async Task<IActionResult> GetCustomersForDropDown()
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
    [HttpGet("GetCustomerBySearch/{searchString}", Name = nameof(GetCustomerBySearch))]
    [ProducesResponseType(200, Type = typeof(ResponseDTO<List<CustomerDTO>>))]
    public async Task<IActionResult> GetCustomerBySearch(string searchString)
    {
        ResponseDTO<List<CustomerDTO>> respuesta = new ResponseDTO<List<CustomerDTO>>();

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
    [HttpGet("GetCustomerById/{id}", Name = nameof(GetCustomerById))]
    [ProducesResponseType(200, Type = typeof(ResponseDTO<List<CustomerDTO>>))]
    public async Task<IActionResult> GetCustomerById(int id)
    {
        ResponseDTO<List<CustomerDTO>> respuesta = new ResponseDTO<List<CustomerDTO>>();

        var result = await serv.GetDataById(id);
        List<CustomerDTO> resultList = new List<CustomerDTO>
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
    [HttpPost("CreateCustomer/{user}", Name = nameof(CreateCustomer))]
    [ProducesResponseType(201, Type = typeof(ResponseDTO<List<CustomerDTO>>))]
    public async Task<IActionResult> CreateCustomer([FromBody] CustomerDTO customer, int user)
    {
        ResponseDTO<List<CustomerDTO>> respuesta = new ResponseDTO<List<CustomerDTO>>();

        if (customer == null || user == 0)
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

        CustomerDTO added = await serv.AddAsync(customer, user);
        List<CustomerDTO> addedList = new List<CustomerDTO>
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
    [HttpPut("UpdateCustomer/{user}", Name = nameof(UpdateCustomer))]
    [ProducesResponseType(204, Type = typeof(ResponseDTO<List<CustomerDTO>>))]
    public async Task<IActionResult> UpdateCustomer([FromBody] CustomerDTO customer, int user)
    {
        ResponseDTO<List<CustomerDTO>> respuesta = new ResponseDTO<List<CustomerDTO>>();

        if (customer == null || customer.ID == "0" || user == 0)
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

        CustomerDTO result = await serv.UpdateAsync(customer, user);
        List<CustomerDTO> resultList = new List<CustomerDTO>
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
    [HttpDelete("DeleteCustomer/{id}", Name = nameof(DeleteCustomer))]
    [ProducesResponseType(204, Type = typeof(ResponseDTO<List<CustomerDTO>>))]
    public async Task<IActionResult> DeleteCustomer(int id, int user)
    {
        ResponseDTO<List<CustomerDTO>> respuesta = new ResponseDTO<List<CustomerDTO>>();

        if (id <= 0 || user == 0)
        {
            respuesta.status = "400";
            respuesta.message = "Solicitud incorrecta.";
            respuesta.data = null;

            return Ok(respuesta);
        }

        CustomerDTO result = await serv.RemoveAsync(id, user);
        List<CustomerDTO> resultList = new List<CustomerDTO>
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
