using System.Text.Json.Serialization;
namespace Backend.Infrastructure.BcraGateway.DTOs.Responses.HistorialReponses
{
    public class HistorialResponse
    {
        [JsonPropertyName("status")]
        public int Status { get; set; }

        [JsonPropertyName("results")]
        public HistorialDeuda Results { get; set; }
    }

}