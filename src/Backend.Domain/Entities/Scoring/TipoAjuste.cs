namespace Backend.Domain.Entities.Scoring
{
    /// <summary>
    /// Tipos de ajustes que se aplican al score base
    /// </summary>
    public enum TipoAjuste
    {
        PeorSituacion24m = 1,
        MesesMora24m = 2,
        RecenciaMora = 3,
        CantidadEntidades = 4,
        ChequesRechazados = 5
    }
}