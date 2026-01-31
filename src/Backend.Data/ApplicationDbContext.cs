using Backend.Domain.Entities.Client;
using Backend.Domain.Entities.Scoring;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Cliente> Clientes => Set<Cliente>();
        public DbSet<HistorialScoring> HistorialesScoring => Set<HistorialScoring>();
        public DbSet<AjusteScoring> AjustesScoring => Set<AjusteScoring>();
        public DbSet<AlertaScoring> AlertasScoring => Set<AlertaScoring>();
        public DbSet<ReglaScoreBase> ReglasScoreBase => Set<ReglaScoreBase>();
        public DbSet<ReglaAjuste> ReglasAjustes => Set<ReglaAjuste>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }

    }
}