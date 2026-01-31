namespace Backend.Domain.Entities.Client;

/// <summary>
/// Value Object que agrupa las features extraidas del BCRA
/// para construir el resumen financiero del cliente.
/// </summary>
public class FeaturesBcra
{
    public int MaxSituacionActual { get; set; }
    public int CantidadEntidadesActual { get; set; }
    public int PeorSituacion24m { get; set; }
    public int MesesMora24m { get; set; }
    public int RecenciaMora { get; set; }
    public int ChequesEventos12m { get; set; }
}
