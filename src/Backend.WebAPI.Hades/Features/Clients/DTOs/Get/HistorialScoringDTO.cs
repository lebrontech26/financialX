using Backend.Domain.Entities.Client;

namespace Backend.WebAPI.Hades.Features.Clients.DTOs.Get;
public class HistorialScoringDto
{
    public int Id { get; set; }
    public DateTime CalculadoEn { get; set; }
    public int PuntajeBase { get; set; }
    public int PuntajeFinal { get; set; }
    public CategoriaRiesgo Categoria { get; set; }
    public bool SinEvidenciaCrediticia { get; set; }
    public List<AjusteScoringDto> Ajustes { get; set; } = new();
    public List<string> Alertas { get; set; } = new();
}
