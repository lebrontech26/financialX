using System.Text.Json.Serialization;
namespace Backend.Infrastructure.BcraGateway.DTOs.Responses.ChequeRechazadoResponses
{
    public class Causale
    {
        [JsonPropertyName("causal")]
        public string Causal { get; set; }

        [JsonPropertyName("entidades")]
        public List<Entidad> Entidades { get; set; }
    }
}