using Backend.Domain.Entities.Scoring;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data.Repositories.ScoringRepository
{
    public class ScoringRulesRepository : IScoringRulesRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ScoringRulesRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<List<ReglaScoreBase>> GetAllReglasScoreBaseAsync()
        {
            return _dbContext.ReglasScoreBase
                             .AsNoTracking()
                             .Where(r => r.EstaActivo)
                             .OrderBy(r => r.Situacion)
                             .ToListAsync();
        }

        public Task<int> GetScoreBaseAsync(int situacion)
        {
            return _dbContext.ReglasScoreBase
                             .AsNoTracking()
                             .Where(r => r.Situacion == situacion && r.EstaActivo)
                             .Select(r => r.ScoreBase)
                             .FirstOrDefaultAsync();
        }

        public Task<ReglaAjuste> GetReglaAjusteAsync(TipoAjuste tipoAjuste, int valor)
        {
            return _dbContext.ReglasAjustes
                             .AsNoTracking()
                             .Where(r => r.TipoAjuste == tipoAjuste && r.EstaActivo &&
                                        valor >= r.ValorMinimo &&
                                        (r.ValorMaximo == null || valor <= r.ValorMaximo)
                                    )
                             .FirstOrDefaultAsync();
        }

        public Task<List<ReglaAjuste>> GetReglasAjustesAsync()
        {
            return _dbContext.ReglasAjustes
                             .AsNoTracking()
                             .Where(r => r.EstaActivo)
                             .OrderBy(r => r.TipoAjuste)
                             .ThenBy(r => r.ValorMinimo)
                             .ToListAsync();
        }
    }
}