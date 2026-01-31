using System.Text.Json.Serialization;

namespace Backend.Infrastructure.BcraGateway.DTOs.Responses.HistorialReponses
{
    public class HistorialDeuda
    {
        [JsonPropertyName("identificacion")]
        public long Identificacion { get; set; }

        [JsonPropertyName("denominacion")]
        public string Denominacion { get; set; }

        [JsonPropertyName("periodos")]
        public List<Periodo> Periodos { get; set; }
    }


}