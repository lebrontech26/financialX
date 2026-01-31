namespace Backend.Domain.Entities.Scoring
{
    /// <summary>
    /// Tabla de score base según situación crediticia actual
    /// </summary>
    public class ReglaScoreBase
    {
        public int Id { get; set; }
        public int Situacion { get; set; }
        public int ScoreBase { get; set; }
        public DateTime CreadoEn { get; set; }
        public bool EstaActivo { get; set; } = true;    
    }
}