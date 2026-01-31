namespace Backend.WebAPI.Hades.Features.Clients.DTOs.Get;

public class AjusteScoringDto
{
    public string TipoAjuste { get; set; } = default!;
    public string Descripcion { get; set; } = default!;
    public int Valor { get; set; }
}