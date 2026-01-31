using System.Text.Json.Serialization;

namespace Backend.Infrastructure.BcraGateway.DTOs.Responses.DeudaResponses
{
    public class Periodo
    {
        [JsonPropertyName("periodo")]
        public string Periodo1 { get; set; }

        [JsonPropertyName("entidades")]
        public List<Entidad> Entidades { get; set; }
    }


}