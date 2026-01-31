using Backend.Shared.FriendlyMessageProvider;
using Backend.WebAPI.Hades.Features.Clients.Codes;
using Backend.WebAPI.Hades.Features.Clients.DTOs.Create;
using Backend.WebAPI.Hades.Features.Clients.DTOs.Update;
using Backend.WebAPI.Hades.Features.Clients.FriendlyMessages;
using Backend.WebAPI.Hades.Features.Clients.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.WebAPI.Hades.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientController : ControllerBase
    {
        private readonly IClienteService _clienteService;

        public ClientController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        [HttpPost]
        public async Task<IActionResult> CrearCliente([FromBody] CreateClienteRequest clienteRequest)
        {
            if (!ModelState.IsValid) return BadRequest();

            var result = await _clienteService.CrearClienteAsync(clienteRequest);

            if (!result.IsSuccess)
            {
                var errorCode = (CodigosCliente)result.ErrorCode;
                var errorMessage = MessageProvider.Get(ClienteDictionary.Messages, errorCode);
                return BadRequest(errorMessage);
            }

            return Ok(result.Value);
        }

        [HttpPut]
        public async Task<IActionResult> ModificarCliente([FromBody] UpdateClienteRequest clienteRequest)
        {
            if (!ModelState.IsValid) return BadRequest();

            var result = await _clienteService.ModificarClienteAsync(clienteRequest);

            if (!result.IsSuccess)
            {
                var errorCode = (CodigosCliente)result.ErrorCode;
                var errorMessage = MessageProvider.Get(ClienteDictionary.Messages, errorCode);

                if (errorCode == CodigosCliente.cliente_no_encontrado)
                    return NotFound(errorMessage);

                return BadRequest(errorMessage);
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DesactivarCliente(int id)
        {
            var result = await _clienteService.DesactivarClienteAsync(id);

            if (!result.IsSuccess)
            {
                var errorCode = (CodigosCliente)result.ErrorCode;
                var errorMessage = MessageProvider.Get(ClienteDictionary.Messages, errorCode);

                if (errorCode == CodigosCliente.cliente_no_encontrado)
                    return NotFound(errorMessage);

                return BadRequest(errorMessage);
            }

            return NoContent();
        }

        [HttpPatch("{id}/activar")]
        public async Task<IActionResult> ActivarCliente(int id)
        {
            var result = await _clienteService.ActivarClienteAsync(id);

            if (!result.IsSuccess)
            {
                var errorCode = (CodigosCliente)result.ErrorCode;
                var errorMessage = MessageProvider.Get(ClienteDictionary.Messages, errorCode);

                if (errorCode == CodigosCliente.cliente_no_encontrado)
                    return NotFound(errorMessage);

                return BadRequest(errorMessage);
            }

            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetClienteConDetalles(int id)
        {
            var result = await _clienteService.GetClienteDetallesAsync(id);

            if (!result.IsSuccess)
            {
                var errorCode = (CodigosCliente)result.ErrorCode;
                var errorMessage = MessageProvider.Get(ClienteDictionary.Messages, errorCode);

                if (errorCode == CodigosCliente.cliente_no_encontrado)
                    return NotFound(errorMessage);

                return BadRequest(errorMessage);
            }

            return Ok(result.Value);
        }

        [HttpGet]
        public async Task<IActionResult> GetClientes(
           [FromQuery] int page = 1,
           [FromQuery] int pageSize = 10,
           [FromQuery] bool estaActivo = true,
           [FromQuery] string searchTerm = ""
        )
        {
            var result = await _clienteService.GetClientesPaginadosAsync(page, pageSize, estaActivo, searchTerm);

            if (!result.IsSuccess)
            {
                var errorCode = (CodigosCliente)result.ErrorCode;
                var errorMessage = MessageProvider.Get(ClienteDictionary.Messages, errorCode);
                return BadRequest(errorMessage);
            }

            return Ok(result.Value);
        }

    }
}