namespace Backend.Domain.Entities.Client;


public class HistorialScoring
{
    public int Id { get; set; }

    public int ClienteId { get; set; }
    public Cliente Cliente { get; set; } = default!;

    public DateTime CalculadoEn { get; set; } = DateTime.UtcNow;
    public int PuntajeBase { get; set; }
    public int PuntajeFinal { get; set; }

    public CategoriaRiesgo Categoria { get; set; }

    public bool SinEvidenciaCrediticia { get; set; }

    //public string? BcraSnapshotRawJson { get; set; }

    // ─── Features del BCRA (Value Object) ───
    public FeaturesBcra FeaturesBcra { get; set; } = new();

    public List<AjusteScoring> Ajustes { get; set; } = new();
    public List<AlertaScoring> Alertas { get; set; } = new();

    public static HistorialScoring Crear(
        int clienteId,
        int puntajeBase,
        int puntajeFinal,
        CategoriaRiesgo categoria,
        bool sinEvidenciaCrediticia
        /*,string? bcraSnapshot = null*/)
    {
        // Validar que el score esté en rango
        if (puntajeFinal < 0 || puntajeFinal > 100)
            throw new ArgumentException("El puntaje final debe estar entre 0 y 100", nameof(puntajeFinal));

        return new HistorialScoring
        {
            ClienteId = clienteId,
            CalculadoEn = DateTime.UtcNow,
            PuntajeBase = puntajeBase,
            PuntajeFinal = puntajeFinal,
            Categoria = categoria,
            SinEvidenciaCrediticia = sinEvidenciaCrediticia,
            //BcraSnapshotRawJson = bcraSnapshot
        };
    }
    
}

