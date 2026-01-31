using Backend.Domain.Entities.Scoring;

namespace Backend.Data.Repositories.ScoringRepository
{
    public interface IScoringRulesRepository
    {
        Task<int> GetScoreBaseAsync(int situacion);
        Task<ReglaAjuste> GetReglaAjusteAsync(TipoAjuste tipoAjuste, int valor);
        Task<List<ReglaAjuste>> GetReglasAjustesAsync();
        Task<List<ReglaScoreBase>> GetAllReglasScoreBaseAsync();
    }
}