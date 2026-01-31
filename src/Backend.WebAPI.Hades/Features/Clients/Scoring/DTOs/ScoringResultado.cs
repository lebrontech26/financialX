

using Backend.Domain.Entities.Client;

namespace Backend.WebAPI.Hades.Features.Clients.Scoring.DTOs
{
    public class ScoringResult
    {
        public int PuntajeBase {get; init;}
        public int PuntajeFinal {get; init;}
        public CategoriaRiesgo Categoria {get; init;}
        public bool SinEvidenciaCrediticia {get; init;}
        public List<ScoreAjustesDTO> Ajustes {get; init;} = [];
        public List<string> Alertas {get; init;} = [];
        public ScoringFeaturesDTO ScoringFeaturesDTO {get; init;} = default!;
    }
}