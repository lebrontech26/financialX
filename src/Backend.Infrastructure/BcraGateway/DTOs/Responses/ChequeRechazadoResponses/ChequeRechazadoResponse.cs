using System.Text.Json.Serialization;
namespace Backend.Infrastructure.BcraGateway.DTOs.Responses.ChequeRechazadoResponses
{
    public class ChequeRechazadoResponse
    {
        [JsonPropertyName("status")]
        public int Status { get; set; }

        [JsonPropertyName("results")]
        public ChequeRechazado Results { get; set; }
    }
}