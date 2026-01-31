using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Domain.Entities.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Data.Configurations
{
    public class AjusteScoringConfiguration : IEntityTypeConfiguration<AjusteScoring>
    {
        public void Configure(EntityTypeBuilder<AjusteScoring> entity)
        {
            entity.ToTable("AjustesScoring");
            entity.HasKey(a => a.Id);

            entity.Property(a => a.Codigo)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(a => a.Descripcion)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(a => a.Valor)
                .IsRequired();

            entity.HasIndex(a => a.HistorialScoringId)
                .HasDatabaseName("IX_AjustesScoring_HistorialScoringId");
        }
    }
}