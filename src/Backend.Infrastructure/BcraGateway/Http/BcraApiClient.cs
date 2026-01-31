using System.Net;
using System.Text.Json;
using Backend.Infrastructure.BcraGateway.DTOs.Responses.ChequeRechazadoResponses;
using Backend.Infrastructure.BcraGateway.DTOs.Responses.DeudaResponses;
using Backend.Infrastructure.BcraGateway.DTOs.Responses.HistorialReponses;
using Backend.Shared.ResultPattern;
using Microsoft.Extensions.Logging;


namespace Backend.Infrastructure.BcraGateway.Http
{
    /// <summary>
    /// Antes de hacer el consumo de API del BCRA se hara una validacion 
    /// del cuil mediante los servicios de ARCA (ex AFIP)
    /// </summary>
    public class BcraApiClient : IBcraApiClient
    {
        private readonly ILogger<BcraApiClient> _logger;
        private readonly HttpClient _httpClient;
        private string endpoint;
        public BcraApiClient(HttpClient httpClient, ILogger<BcraApiClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        private Result<T> DeserializeResponse<T>(string content) where T : class
        {
            try
            {
                var result = JsonSerializer.Deserialize<T>(content);

                if (result is null) return Result<T>.Failure(BcraErrorCode.RespuestaVacia);

                return Result<T>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deserializing response for type {Type}. Content preview: {Content}", typeof(T).Name, content[..Math.Min(200, content.Length)]);
                return Result<T>.Failure(BcraErrorCode.RespuestaInvalida);
            }

        }
        private async Task<Result<T>> ExecuteRequestAsync<T>(string endpoint) where T : class
        {
            try
            {
                var response = await _httpClient.GetAsync(endpoint);
                var content = await response.Content.ReadAsStringAsync();

                return response.StatusCode switch
                {
                    HttpStatusCode.OK => DeserializeResponse<T>(content),

                    //404 = CUIL valido pero sin historial crediticio en BCRA.
                    HttpStatusCode.NotFound => Result<T>.Failure(BcraErrorCode.SinEvidenciaCrediticia),

                    //400 = parametro no valido. (e.g : cuil imcompleto, etc)
                    HttpStatusCode.BadRequest => Result<T>.Failure(BcraErrorCode.ParametroErroneo),
                    HttpStatusCode.ServiceUnavailable => Result<T>.Failure(BcraErrorCode.ServicioNoDisponible),
                    HttpStatusCode.InternalServerError => Result<T>.Failure(BcraErrorCode.ErrorServidor),
                    _ => Result<T>.Failure(BcraErrorCode.ErrorDesconocido),
                };
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network error calling BCRA endpoint {Endpoint}. InnerException: {InnerMessage}", endpoint, ex.InnerException?.Message);
                return Result<T>.Failure(BcraErrorCode.ErrorDeRed);
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Request timeout for BCRA endpoint {Endpoint}. Timeout details: {Details}", endpoint, ex.Message);
                return Result<T>.Failure(BcraErrorCode.Timeout);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON parsing error from BCRA endpoint {Endpoint}. Error at Line {Line}", endpoint, ex.LineNumber);
                return Result<T>.Failure(BcraErrorCode.Timeout);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error calling BCRA endpoint {Endpoint}. Exception type: {ExceptionType}, Message: {Message}", endpoint, ex.GetType().Name, ex.Message);
                return Result<T>.Failure(BcraErrorCode.ErrorServidor);
            }
        }

        //https://api.bcra.gob.ar/centraldedeudores/v1.0/Deudas/29431621539
        public async Task<Result<DeudaResponse>> GetDeudasAsync(string cuil)
        {
            endpoint = $"centraldedeudores/v1.0/Deudas/{cuil}";
            return await ExecuteRequestAsync<DeudaResponse>(endpoint);
        }

        public async Task<Result<HistorialResponse>> GetHistoricasAsync(string cuil)
        {
            endpoint = $"centraldedeudores/v1.0/Deudas/Historicas/{cuil}";
            return await ExecuteRequestAsync<HistorialResponse>(endpoint);
        }

        public async Task<Result<ChequeRechazadoResponse>> GetChequesRechazadosAsync(string cuil)
        {
            endpoint = $"centraldedeudores/v1.0/Deudas/ChequesRechazados/{cuil}";
            return await ExecuteRequestAsync<ChequeRechazadoResponse>(endpoint);
        }
    }
}