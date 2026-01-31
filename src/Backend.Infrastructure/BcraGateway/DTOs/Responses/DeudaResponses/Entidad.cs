using System.Text.Json.Serialization;

namespace Backend.Infrastructure.BcraGateway.DTOs.Responses.DeudaResponses
{
    public class Entidad
    {
        [JsonPropertyName("entidad")]
        public string Entidad1 { get; set; }

        [JsonPropertyName("situacion")]
        public int Situacion { get; set; }

        [JsonPropertyName("fechaSit1")]
        public object FechaSit1 { get; set; }

        [JsonPropertyName("monto")]
        public double Monto { get; set; }

        [JsonPropertyName("diasAtrasoPago")]
        public int DiasAtrasoPago { get; set; }

        [JsonPropertyName("refinanciaciones")]
        public bool Refinanciaciones { get; set; }

        [JsonPropertyName("recategorizacionOblig")]
        public bool RecategorizacionOblig { get; set; }

        [JsonPropertyName("situacionJuridica")]
        public bool SituacionJuridica { get; set; }

        [JsonPropertyName("irrecDisposicionTecnica")]
        public bool IrrecDisposicionTecnica { get; set; }

        [JsonPropertyName("enRevision")]
        public bool EnRevision { get; set; }

        [JsonPropertyName("procesoJud")]
        public bool ProcesoJud { get; set; }
    }


}