namespace Backend.Domain.Entities.Client;
public class AjusteScoring
{
    public int Id { get; set; }
    public int HistorialScoringId { get; set; }
    public HistorialScoring HistorialScoring { get; set; } = default!;
    public string Codigo { get; set; } = default!;
    public string Descripcion { get; set; } = default!;
    public int Valor { get; set; }
}
