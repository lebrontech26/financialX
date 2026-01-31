using Backend.Infrastructure.BcraGateway.DTOs.Responses.ChequeRechazadoResponses;
using Backend.Infrastructure.BcraGateway.DTOs.Responses.DeudaResponses;
using Backend.Infrastructure.BcraGateway.DTOs.Responses.HistorialReponses;

namespace Backend.Infrastructure.BcraGateway.DTOs
{
    public class BcraData
    {
        public DeudaResponse? DeudasActuales {get; init;}
        public HistorialResponse Historico {get; init;}
        public ChequeRechazadoResponse ChequesRechazados {get; init;}
        public bool SinEvidenciaCrediticia {get; init;}
    }
}

