using Backend.Shared.ResultPattern; 
using Backend.Infrastructure.BcraGateway.DTOs.Responses.ChequeRechazadoResponses;
using Backend.Infrastructure.BcraGateway.DTOs.Responses.DeudaResponses;
using Backend.Infrastructure.BcraGateway.DTOs.Responses.HistorialReponses;

namespace Backend.Infrastructure.BcraGateway.Http
{
    public interface IBcraApiClient
    {
        Task<Result<DeudaResponse>> GetDeudasAsync(string cuil);
        Task<Result<HistorialResponse>> GetHistoricasAsync(string cuil);
        Task<Result<ChequeRechazadoResponse>> GetChequesRechazadosAsync(string cuil);
    }
}



