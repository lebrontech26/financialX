namespace Backend.WebAPI.Hades.Features.Clients.DTOs.Get
{
    public class ClienteDetalleResponse
    {
        public int Id { get; set; }
        public string Cuil { get; set; } = default!;
        public string Nombre { get; set; } = default!;
        public string Apellido { get; set; } = default!;
        public DateTime FechaNacimiento { get; set; }
        public string? Telefono { get; set; }
        public string Calle { get; set; } = default!;
        public string Localidad { get; set; } = default!;
        public string Provincia { get; set; } = default!;
        //public bool EstaActivo { get; set; }
        public DateTime CreadoEn { get; set; }
        //public DateTime ActualizadoEn { get; set; }
        public PerfilRiesgoDto? PerfilActual { get; set; }
        public List<HistorialScoringDto> HistorialScoring { get; set; } = new();
        public ResumenFinancieroDTO? ResumenFinanciero { get; set; }
    }
}