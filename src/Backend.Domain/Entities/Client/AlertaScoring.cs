namespace Backend.Domain.Entities.Client;

public class AlertaScoring
{
    public int Id { get; set; }
    public int HistorialScoringId { get; set; }
    public HistorialScoring HistorialScoring { get; set; } = default!;
    public string Texto { get; set; } = default!;
}