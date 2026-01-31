using System.Text.Json.Serialization;

namespace Backend.Infrastructure.BcraGateway.DTOs.Responses.ChequeRechazadoResponses
{
    public class ChequeRechazado
    {
        [JsonPropertyName("identificacion")]
        public int Identificacion { get; set; }

        [JsonPropertyName("denominacion")]
        public string Denominacion { get; set; }

        [JsonPropertyName("causales")]
        public List<Causale> Causales { get; set; }
    }


}