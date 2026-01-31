using Backend.Domain.Entities.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Data.Configurations
{
    public class AlertaScoringConfiguration : IEntityTypeConfiguration<AlertaScoring>
    {
        public void Configure(EntityTypeBuilder<AlertaScoring> entity)
        {
            entity.ToTable("AlertasScoring");
            entity.HasKey(a => a.Id);

            entity.Property(a => a.Texto)
                .IsRequired()
                .HasMaxLength(500);

            entity.HasIndex(a => a.HistorialScoringId)
                .HasDatabaseName("IX_AlertasScoring_HistorialScoringId");
        }
    }
}