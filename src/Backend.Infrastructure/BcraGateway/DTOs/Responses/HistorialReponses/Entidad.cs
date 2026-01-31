using System.Text.Json.Serialization;

namespace Backend.Infrastructure.BcraGateway.DTOs.Responses.HistorialReponses
{
    public class Entidad
    {
        [JsonPropertyName("entidad")]
        public string Entidad1 { get; set; }

        [JsonPropertyName("situacion")]
        public int Situacion { get; set; }

        [JsonPropertyName("monto")]
        public double Monto { get; set; }

        [JsonPropertyName("enRevision")]
        public bool EnRevision { get; set; }

        [JsonPropertyName("procesoJud")]
        public bool ProcesoJud { get; set; }
    }


}