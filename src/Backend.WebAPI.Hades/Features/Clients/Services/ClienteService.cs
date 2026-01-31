using AutoMapper;
using Backend.Data.Repositories.ClienteRepository;
using Backend.Domain.Entities.Client;
using Backend.Infrastructure.BcraGateway.DTOs;
using Backend.Infrastructure.BcraGateway.Services;
using Backend.Shared.Paged;
using Backend.Shared.ResultPattern;
using Backend.WebAPI.Hades.Features.Clients.Codes;
using Backend.WebAPI.Hades.Features.Clients.DTOs.Create;
using Backend.WebAPI.Hades.Features.Clients.DTOs.Get;
using Backend.WebAPI.Hades.Features.Clients.DTOs.Update;
using Backend.WebAPI.Hades.Features.Clients.Scoring.DTOs;
using Backend.WebAPI.Hades.Features.Clients.Scoring.Services;

namespace Backend.WebAPI.Hades.Features.Clients.Services
{
    public class ClienteService : IClienteService
    {
        private readonly ILogger<ClienteService> _logger;
        private readonly IClienteRepository _clienteRepository;
        private readonly IBcraDataService _bcraDataService;
        private readonly IScoringService _scoringService;
        private readonly IMapper _mapper;

        public ClienteService(IClienteRepository clienteRepository, IBcraDataService bcraDataService, IScoringService scoringService, IMapper mapper, ILogger<ClienteService> logger)
        {
            _clienteRepository = clienteRepository;
            _bcraDataService = bcraDataService;
            _scoringService = scoringService;
            _mapper = mapper;
            _logger = logger;
        }

        private async Task<Result<bool>> ValidarCuil(string cuil)
        {
            if (string.IsNullOrWhiteSpace(cuil) || string.IsNullOrEmpty(cuil))
                return Result<bool>.Failure(CodigosCliente.cuil_nulo_o_vacio);

            if (cuil.Length != 11)
                return Result<bool>.Failure(CodigosCliente.cuil_longitud_error);

            if (!cuil.All(char.IsDigit))
                return Result<bool>.Failure(CodigosCliente.cuil_contiene_letras);

            var existeCuil = await _clienteRepository.ExisteCuilAsync(cuil);

            if (existeCuil)
                return Result<bool>.Failure(CodigosCliente.cuil_existente);

            return Result<bool>.Success();
        }

        public async Task<Result<CreateClienteResponse>> CrearClienteAsync(CreateClienteRequest clienteDTO)
        {
            try
            {
                var validacionCuilResult = await ValidarCuil(clienteDTO.Cuil);

                if (!validacionCuilResult.IsSuccess)
                    return Result<CreateClienteResponse>.Failure(validacionCuilResult.ErrorCode);

                var bcraDataResult = await _bcraDataService.GetBcraDataAsync(clienteDTO.Cuil);

                if (!bcraDataResult.IsSuccess)
                    return Result<CreateClienteResponse>.Failure(bcraDataResult.ErrorCode);

                BcraData bcraData = bcraDataResult.Value;
                var scoringResult = await _scoringService.CalcularScoringAsync(bcraData);

                if (!scoringResult.IsSuccess)
                    return Result<CreateClienteResponse>.Failure(scoringResult.ErrorCode);

                ScoringResult scoring = scoringResult.Value;

                var direccion = new Direccion
                {
                    Calle = clienteDTO.Calle,
                    Localidad = clienteDTO.Localidad,
                    Provincia = clienteDTO.Provincia
                };

                var cliente = Cliente.Crear(
                    cuil: clienteDTO.Cuil,
                    nombre: clienteDTO.Nombre,
                    apellido: clienteDTO.Apellido,
                    fechaNacimiento: clienteDTO.FechaNacimiento,
                    telefono: clienteDTO.Telefono,
                    domicilio: direccion
                );

                var historialScoring = HistorialScoring.Crear
                (
                    clienteId: 0,
                    puntajeBase: scoring.PuntajeBase,
                    puntajeFinal: scoring.PuntajeFinal,
                    categoria: scoring.Categoria,
                    sinEvidenciaCrediticia: scoring.SinEvidenciaCrediticia
                );

                // Mapear features del BCRA
                historialScoring.FeaturesBcra = _mapper.Map<FeaturesBcra>(scoring.ScoringFeaturesDTO);
                historialScoring.Ajustes = _mapper.Map<List<AjusteScoring>>(scoring.Ajustes);
                historialScoring.Alertas = _mapper.Map<List<AlertaScoring>>(scoring.Alertas);

                cliente.HistorialesScoring.Add(historialScoring);

                await _clienteRepository.CrearClienteAsync(cliente);

                var response = new CreateClienteResponse
                {
                    Id = cliente.Id,
                    Cuil = cliente.Cuil,
                    Nombre = cliente.Nombre,
                    Apellido = cliente.Apellido,
                    ScoreFinal = scoring.PuntajeFinal,
                    Categoria = scoring.Categoria,
                    SinEvidenciaCrediticia = scoring.SinEvidenciaCrediticia
                };

                return Result<CreateClienteResponse>.Success(response);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear cliente. CUIL: {Cuil}", clienteDTO.Cuil);
                return Result<CreateClienteResponse>.Failure(CodigosCliente.error_desconocido);
            }
        }

        public async Task<Result<bool>> ModificarClienteAsync(UpdateClienteRequest clienteModificadoDTO)
        {
            try
            {
                var cliente = await _clienteRepository.GetCliente(clienteModificadoDTO.Id);

                if (cliente is null)
                    return Result<bool>.Failure(CodigosCliente.cliente_no_encontrado);

                var direcion = new Direccion(
                 clienteModificadoDTO.Calle,
                 clienteModificadoDTO.Localidad,
                 clienteModificadoDTO.Provincia
                );

                cliente.ActualizarDatosPersonales(
                    nombre: clienteModificadoDTO.Nombre,
                    apellido: clienteModificadoDTO.Apellido,
                    fechaNacimiento: clienteModificadoDTO.FechaNacimiento,
                    telefono: clienteModificadoDTO.Telefono,
                    domicilio: direcion
                );

                await _clienteRepository.ModificarCliente(cliente);

                return Result<bool>.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al modificar cliente. Id : {Id} - Nombre : {nombre}", clienteModificadoDTO.Id, clienteModificadoDTO.Nombre + clienteModificadoDTO.Apellido);
                return Result<bool>.Failure(CodigosCliente.error_desconocido);
            }
        }

        public async Task<Result<bool>> ActivarClienteAsync(int id)
        {
            try
            {
                var cliente = await _clienteRepository.GetCliente(id);

                if (cliente is null)
                    return Result<bool>.Failure(CodigosCliente.cliente_no_encontrado);

                if (cliente.EstaActivo)
                    return Result<bool>.Failure(CodigosCliente.cliente_ya_activo);

                cliente.ModificarEstado();

                await _clienteRepository.ModificarCliente(cliente);

                return Result<bool>.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al desactivar cliente. Id : {Id}", id);
                return Result<bool>.Failure(CodigosCliente.error_desconocido);
            }
        }
        public async Task<Result<bool>> DesactivarClienteAsync(int id)
        {
            try
            {
                var cliente = await _clienteRepository.GetCliente(id);

                if (cliente is null)
                    return Result<bool>.Failure(CodigosCliente.cliente_no_encontrado);

                if (!cliente.EstaActivo)
                    return Result<bool>.Failure(CodigosCliente.cliente_ya_inactivo);

                cliente.ModificarEstado(false);

                await _clienteRepository.ModificarCliente(cliente);

                return Result<bool>.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al desactivar cliente. Id : {Id}", id);
                return Result<bool>.Failure(CodigosCliente.error_desconocido);
            }
        }

        public async Task<Result<ClienteDetalleResponse>> GetClienteDetallesAsync(int id)
        {
            try
            {
                var cliente = await _clienteRepository.GetClienteConHistorial(id);

                if (cliente is null)
                    return Result<ClienteDetalleResponse>.Failure(CodigosCliente.cliente_no_encontrado);

                var responseDTO = _mapper.Map<ClienteDetalleResponse>(cliente);

                // Construir resumen financiero desde el perfil actual
                var perfilActual = cliente.HistorialesScoring
                    .OrderByDescending(h => h.CalculadoEn)
                    .FirstOrDefault();

                if (perfilActual is not null)
                {
                    var builder = new ResumenFinancieroBuilder();
                    responseDTO.ResumenFinanciero = builder.Build(
                        perfilActual.FeaturesBcra,
                        perfilActual.SinEvidenciaCrediticia
                    );
                }

                return Result<ClienteDetalleResponse>.Success(responseDTO);

            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error al obtener detalle del cliente. Id: {Id}", id);
                return Result<ClienteDetalleResponse>.Failure(CodigosCliente.error_desconocido);
            }
        }

        public async Task<Result<PagedList<ClienteDTO>>> GetClientesPaginadosAsync(int page, int pageSize, bool estaActivo = true, string? searchTerm = "")
        {
            try
            {
                var query = _clienteRepository.GetClientesQueryable(estaActivo, searchTerm);

                var proyeccion = _mapper.ProjectTo<ClienteDTO>(query);

                var clientesPaginados = await PagedList<ClienteDTO>.CreateAsync(proyeccion, page, pageSize);

                return Result<PagedList<ClienteDTO>>.Success(clientesPaginados);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error inesperado en búsqueda de clientes: " + ex);
                return Result<PagedList<ClienteDTO>>.Failure(CodigosCliente.error_desconocido);
                throw;
            }
        }
    }
}