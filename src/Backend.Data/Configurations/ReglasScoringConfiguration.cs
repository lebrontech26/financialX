using Backend.Domain.Entities.Client;
using Backend.Domain.Entities.Scoring;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Backend.Data.Configurations;

public class ReglasScoringConfiguration : IEntityTypeConfiguration<ReglaAjuste>, IEntityTypeConfiguration<ReglaScoreBase>
{
    public void Configure(EntityTypeBuilder<ReglaScoreBase> entity)
    {
        entity.ToTable("ReglasScoreBase");
        entity.HasKey(r => r.Id);

        entity.Property(r => r.Situacion)
            .IsRequired();

        entity.Property(r => r.ScoreBase)
            .IsRequired();

        entity.Property(r => r.EstaActivo)
            .IsRequired()
            .HasDefaultValue(true);

        entity.Property(r => r.CreadoEn)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Índice único: solo una regla activa por situación
        entity.HasIndex(r => new { r.Situacion, r.EstaActivo })
            .IsUnique()
            .HasFilter("EstaActivo = 1")
            .HasDatabaseName("IX_ReglasScoreBase_Situacion_Activa");

        entity.HasData(
            new ReglaScoreBase { Id = 1, Situacion = 1, ScoreBase = 80 },
                new ReglaScoreBase { Id = 2, Situacion = 2, ScoreBase = 55 },
                new ReglaScoreBase { Id = 3, Situacion = 3, ScoreBase = 35 },
                new ReglaScoreBase { Id = 4, Situacion = 4, ScoreBase = 15 },
                new ReglaScoreBase { Id = 5, Situacion = 5, ScoreBase = 0 },
                new ReglaScoreBase { Id = 6, Situacion = 6, ScoreBase = 0 }
        );
    }
    public void Configure(EntityTypeBuilder<ReglaAjuste> entity)
    {
        entity.ToTable("ReglasAjustes");
        entity.HasKey(r => r.Id);

        entity.Property(r => r.TipoAjuste)
            .IsRequired();

        entity.Property(r => r.ValorMinimo)
            .IsRequired();

        entity.Property(r => r.ValorMaximo)
            .IsRequired(false); // Nullable - sin límite superior

        entity.Property(r => r.Ajuste)
            .IsRequired();

        entity.Property(r => r.Codigo)
            .IsRequired()
            .HasMaxLength(100);

        entity.Property(r => r.Descripcion)
            .IsRequired()
            .HasMaxLength(500);

        entity.Property(r => r.EstaActivo)
            .IsRequired()
            .HasDefaultValue(true);

        entity.Property(r => r.CreadoEn)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Índice por tipo de ajuste
        entity.HasIndex(r => new { r.TipoAjuste, r.EstaActivo })
            .HasDatabaseName("IX_ReglasAjustes_Tipo_Activa");

        // Índice compuesto para búsquedas de reglas por valor
        entity.HasIndex(r => new { r.TipoAjuste, r.ValorMinimo, r.ValorMaximo })
            .HasDatabaseName("IX_ReglasAjustes_Tipo_Rango");

        // ════════════════════════════════════════════════════════════════════
        // SEED DATA - Datos iniciales según documentación
        // ════════════════════════════════════════════════════════════════════
        SeedReglasScoring(entity);

    }
    private static void SeedReglasScoring(EntityTypeBuilder<ReglaAjuste> entity)
    {
        entity.HasData(
            // Peor Situación 24m
            new ReglaAjuste { Id = 1, TipoAjuste = TipoAjuste.PeorSituacion24m, ValorMinimo = 1, ValorMaximo = 1, Ajuste = 0, Codigo = "PEOR_SIT_24M_NORMAL", Descripcion = "Sin deterioro histórico", EstaActivo = true },
            new ReglaAjuste { Id = 2, TipoAjuste = TipoAjuste.PeorSituacion24m, ValorMinimo = 2, ValorMaximo = 2, Ajuste = -5, Codigo = "PEOR_SIT_24M_NIVEL_2", Descripcion = "Riesgo bajo histórico detectado", EstaActivo = true },
            new ReglaAjuste { Id = 3, TipoAjuste = TipoAjuste.PeorSituacion24m, ValorMinimo = 3, ValorMaximo = 3, Ajuste = -12, Codigo = "PEOR_SIT_24M_NIVEL_3", Descripcion = "Dificultades históricas detectadas", EstaActivo = true },
            new ReglaAjuste { Id = 4, TipoAjuste = TipoAjuste.PeorSituacion24m, ValorMinimo = 4, ValorMaximo = 4, Ajuste = -20, Codigo = "PEOR_SIT_24M_NIVEL_4", Descripcion = "Situación severa histórica", EstaActivo = true },
            new ReglaAjuste { Id = 5, TipoAjuste = TipoAjuste.PeorSituacion24m, ValorMinimo = 5, ValorMaximo = null, Ajuste = -35, Codigo = "PEOR_SIT_24M_CRITICA", Descripcion = "Situación crítica en historial", EstaActivo = true },

            // Meses en Mora 24m
            new ReglaAjuste { Id = 6, TipoAjuste = TipoAjuste.MesesMora24m, ValorMinimo = 0, ValorMaximo = 0, Ajuste = 0, Codigo = "MESES_MORA_NINGUNO", Descripcion = "Sin períodos de mora", EstaActivo = true },
            new ReglaAjuste { Id = 7, TipoAjuste = TipoAjuste.MesesMora24m, ValorMinimo = 1, ValorMaximo = 2, Ajuste = -5, Codigo = "MESES_MORA_OCASIONAL", Descripcion = "1-2 meses en mora", EstaActivo = true },
            new ReglaAjuste { Id = 8, TipoAjuste = TipoAjuste.MesesMora24m, ValorMinimo = 3, ValorMaximo = 5, Ajuste = -12, Codigo = "MESES_MORA_FRECUENTE", Descripcion = "3-5 meses en mora", EstaActivo = true },
            new ReglaAjuste { Id = 9, TipoAjuste = TipoAjuste.MesesMora24m, ValorMinimo = 6, ValorMaximo = 10, Ajuste = -20, Codigo = "MESES_MORA_PERSISTENTE", Descripcion = "6-10 meses en mora", EstaActivo = true },
            new ReglaAjuste { Id = 10, TipoAjuste = TipoAjuste.MesesMora24m, ValorMinimo = 11, ValorMaximo = null, Ajuste = -30, Codigo = "MESES_MORA_CRONICA", Descripcion = "Más de 10 meses en mora", EstaActivo = true },

            // Recencia de Mora
            new ReglaAjuste { Id = 11, TipoAjuste = TipoAjuste.RecenciaMora, ValorMinimo = 0, ValorMaximo = 2, Ajuste = -20, Codigo = "MORA_MUY_RECIENTE", Descripcion = "Mora en los últimos 2 meses", EstaActivo = true },
            new ReglaAjuste { Id = 12, TipoAjuste = TipoAjuste.RecenciaMora, ValorMinimo = 3, ValorMaximo = 5, Ajuste = -12, Codigo = "MORA_RECIENTE", Descripcion = "Mora hace 3-5 meses", EstaActivo = true },
            new ReglaAjuste { Id = 13, TipoAjuste = TipoAjuste.RecenciaMora, ValorMinimo = 6, ValorMaximo = 11, Ajuste = -6, Codigo = "MORA_MODERADA", Descripcion = "Mora hace 6-11 meses", EstaActivo = true },
            new ReglaAjuste { Id = 14, TipoAjuste = TipoAjuste.RecenciaMora, ValorMinimo = 12, ValorMaximo = null, Ajuste = 0, Codigo = "MORA_ANTIGUA", Descripcion = "Mora hace más de 12 meses", EstaActivo = true },

            // Cantidad de Entidades
            new ReglaAjuste { Id = 15, TipoAjuste = TipoAjuste.CantidadEntidades, ValorMinimo = 0, ValorMaximo = 1, Ajuste = 0, Codigo = "ENTIDADES_BAJA", Descripcion = "0-1 entidades", EstaActivo = true },
            new ReglaAjuste { Id = 16, TipoAjuste = TipoAjuste.CantidadEntidades, ValorMinimo = 2, ValorMaximo = 3, Ajuste = -4, Codigo = "ENTIDADES_MEDIA", Descripcion = "2-3 entidades", EstaActivo = true },
            new ReglaAjuste { Id = 17, TipoAjuste = TipoAjuste.CantidadEntidades, ValorMinimo = 4, ValorMaximo = 6, Ajuste = -8, Codigo = "ENTIDADES_ALTA", Descripcion = "4-6 entidades", EstaActivo = true },
            new ReglaAjuste { Id = 18, TipoAjuste = TipoAjuste.CantidadEntidades, ValorMinimo = 7, ValorMaximo = null, Ajuste = -12, Codigo = "ENTIDADES_MUY_ALTA", Descripcion = "Más de 6 entidades", EstaActivo = true },

            // Cheques Rechazados
            new ReglaAjuste { Id = 19, TipoAjuste = TipoAjuste.ChequesRechazados, ValorMinimo = 0, ValorMaximo = 0, Ajuste = 0, Codigo = "CHEQUES_NINGUNO", Descripcion = "Sin cheques rechazados", EstaActivo = true },
            new ReglaAjuste { Id = 20, TipoAjuste = TipoAjuste.ChequesRechazados, ValorMinimo = 1, ValorMaximo = 1, Ajuste = -8, Codigo = "CHEQUES_UNO", Descripcion = "1 cheque rechazado", EstaActivo = true },
            new ReglaAjuste { Id = 21, TipoAjuste = TipoAjuste.ChequesRechazados, ValorMinimo = 2, ValorMaximo = 2, Ajuste = -15, Codigo = "CHEQUES_DOS", Descripcion = "2 cheques rechazados", EstaActivo = true },
            new ReglaAjuste { Id = 22, TipoAjuste = TipoAjuste.ChequesRechazados, ValorMinimo = 3, ValorMaximo = null, Ajuste = -30, Codigo = "CHEQUES_MULTIPLES", Descripcion = "3 o más cheques rechazados", EstaActivo = true }
        );
    }
}
