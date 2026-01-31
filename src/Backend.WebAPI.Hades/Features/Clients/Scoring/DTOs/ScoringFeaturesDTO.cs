namespace Backend.WebAPI.Hades.Features.Clients.Scoring.DTOs
{
    public class ScoringFeaturesDTO
    {
        public int MaxSituacionActual {get; set;}
        public int CantidadEntidadesActual {get; set;}
        public  int PeorSituacion24m {get; set;}
        public int MesesMora24m {get; set;}
        public int RecenciaMora {get; set;}
        public int ChequesEventos12m {get; set;}
    }
}