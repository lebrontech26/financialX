using System.Text.Json.Serialization;

namespace Backend.Infrastructure.BcraGateway.DTOs.Responses.ChequeRechazadoResponses
{
    public class Entidad
    {
        [JsonPropertyName("entidad")]
        public int Entidad1 { get; set; }

        [JsonPropertyName("detalle")]
        public List<Detalle> Detalle { get; set; }
    }


}