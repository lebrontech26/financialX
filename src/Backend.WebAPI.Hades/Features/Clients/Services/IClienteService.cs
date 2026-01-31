using Backend.Shared.Paged;
using Backend.Shared.ResultPattern;
using Backend.WebAPI.Hades.Features.Clients.DTOs.Create;
using Backend.WebAPI.Hades.Features.Clients.DTOs.Get;
using Backend.WebAPI.Hades.Features.Clients.DTOs.Update;

namespace Backend.WebAPI.Hades.Features.Clients.Services
{
    public interface IClienteService
    {
        Task<Result<CreateClienteResponse>> CrearClienteAsync(CreateClienteRequest clienteDTO);
        Task<Result<bool>> ModificarClienteAsync(UpdateClienteRequest clienteModificadoDTO);
        Task<Result<bool>> ActivarClienteAsync(int id);
        Task<Result<bool>> DesactivarClienteAsync(int id);
        Task<Result<ClienteDetalleResponse>> GetClienteDetallesAsync(int id);
        Task<Result<PagedList<ClienteDTO>>> GetClientesPaginadosAsync(int page, int pageSize, bool estaActivo = true, string? searchTerm = "");
    }
}