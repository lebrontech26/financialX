using Backend.Domain.Entities.Client;

namespace Backend.WebAPI.Hades.Features.Clients.DTOs.Get;

public class PerfilRiesgoDto
{
    public int PuntajeFinal { get; set; }
    public CategoriaRiesgo Categoria { get; set; }
    public bool SinEvidenciaCrediticia { get; set; }
    public DateTime CalculadoEn { get; set; }
}
