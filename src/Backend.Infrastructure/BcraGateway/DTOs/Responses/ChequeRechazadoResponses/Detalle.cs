using System.Text.Json.Serialization;

namespace Backend.Infrastructure.BcraGateway.DTOs.Responses.ChequeRechazadoResponses
{
    public class Detalle
    {
        [JsonPropertyName("nroCheque")]
        public int NroCheque { get; set; }

        [JsonPropertyName("fechaRechazo")]
        public string FechaRechazo { get; set; }

        [JsonPropertyName("monto")]
        public int Monto { get; set; }

        [JsonPropertyName("fechaPago")]
        public string FechaPago { get; set; }

        [JsonPropertyName("fechaPagoMulta")]
        public string FechaPagoMulta { get; set; }

        [JsonPropertyName("estadoMulta")]
        public string EstadoMulta { get; set; }

        [JsonPropertyName("ctaPersonal")]
        public bool CtaPersonal { get; set; }

        [JsonPropertyName("denomJuridica")]
        public string DenomJuridica { get; set; }

        [JsonPropertyName("enRevision")]
        public bool EnRevision { get; set; }

        [JsonPropertyName("procesoJud")]
        public bool ProcesoJud { get; set; }
    }


}