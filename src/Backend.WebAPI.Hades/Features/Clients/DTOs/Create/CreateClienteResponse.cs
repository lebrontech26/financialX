using Backend.Domain.Entities.Client;

namespace Backend.WebAPI.Hades.Features.Clients.DTOs.Create
{
    public class CreateClienteResponse
    {
        public int Id { get; set; }
        public string Cuil { get; set; } = default!;
        public string Nombre { get; set; } = default!;
        public string Apellido { get; set; } = default!;
        public int ScoreFinal { get; set; }
        public CategoriaRiesgo Categoria { get; set; }
        public bool SinEvidenciaCrediticia { get; set; }
    }
}