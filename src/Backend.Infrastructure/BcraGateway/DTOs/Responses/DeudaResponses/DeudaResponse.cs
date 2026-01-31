using System.Text.Json.Serialization;

namespace Backend.Infrastructure.BcraGateway.DTOs.Responses.DeudaResponses
{
    public class DeudaResponse
    {
        [JsonPropertyName("status")]
        public int Status { get; set; }

        [JsonPropertyName("results")]
        public Deuda Results { get; set; }
    }
}