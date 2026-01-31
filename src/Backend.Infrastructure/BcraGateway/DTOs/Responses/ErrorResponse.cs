using System.Text.Json.Serialization;
namespace Backend.Infrastructure.BcraGateway.DTOs.Responses
{
    public class ErrorResponse
    {
        [JsonPropertyName("status")]
        public int Status { get; set; }

        [JsonPropertyName("errorMessages")]
        public List<string> ErrorMessages { get; set; }
    }
}