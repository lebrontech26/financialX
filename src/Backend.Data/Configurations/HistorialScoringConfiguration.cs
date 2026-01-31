using Backend.Domain.Entities.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Backend.Data.Configurations;

public class HistorialScoringConfiguration : IEntityTypeConfiguration<HistorialScoring>
{
    public void Configure(EntityTypeBuilder<HistorialScoring> entity)
    {
        entity.ToTable("HistorialesScoring");
        entity.HasKey(h => h.Id);

        // ────────────────────────────────────────────────────────────
        // RELACIÓN CON CLIENTE
        // ────────────────────────────────────────────────────────────
        entity.Property(h => h.ClienteId)
            .IsRequired();

        // ────────────────────────────────────────────────────────────
        // DATOS DEL SCORING
        // ────────────────────────────────────────────────────────────
        entity.Property(h => h.CalculadoEn)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .IsRequired();

        entity.Property(h => h.PuntajeBase)
            .IsRequired();

        entity.Property(h => h.PuntajeFinal)
            .IsRequired();

        // Enum se guarda como int en SQLite
        entity.Property(h => h.Categoria)
            .IsRequired()
            .HasConversion<int>();

        entity.Property(h => h.SinEvidenciaCrediticia)
            .IsRequired()
            .HasDefaultValue(false);

        // ─── Features del BCRA (Owned Entity) ───
        entity.OwnsOne(h => h.FeaturesBcra, fb =>
        {
            fb.Property(f => f.MaxSituacionActual)
                .HasColumnName("MaxSituacionActual")
                .HasDefaultValue(0);

            fb.Property(f => f.CantidadEntidadesActual)
                .HasColumnName("CantidadEntidadesActual")
                .HasDefaultValue(0);

            fb.Property(f => f.PeorSituacion24m)
                .HasColumnName("PeorSituacion24m")
                .HasDefaultValue(0);

            fb.Property(f => f.MesesMora24m)
                .HasColumnName("MesesMora24m")
                .HasDefaultValue(0);

            fb.Property(f => f.RecenciaMora)
                .HasColumnName("RecenciaMora")
                .HasDefaultValue(0);

            fb.Property(f => f.ChequesEventos12m)
                .HasColumnName("ChequesEventos12m")
                .HasDefaultValue(0);
        });

        // JSON snapshot del BCRA (puede ser muy largo)
        //entity.Property(h => h.BcraSnapshotRawJson)
        //   .HasColumnType("TEXT"); // SQLite: TEXT para strings largos

        // ────────────────────────────────────────────────────────────
        // RELACIONES CON AJUSTES Y ALERTAS (1-N)
        // ────────────────────────────────────────────────────────────
        entity.HasMany(h => h.Ajustes)
            .WithOne(a => a.HistorialScoring)
            .HasForeignKey(a => a.HistorialScoringId)
            .OnDelete(DeleteBehavior.Cascade); // Si se borra historial, borrar ajustes

        entity.HasMany(h => h.Alertas)
            .WithOne(a => a.HistorialScoring)
            .HasForeignKey(a => a.HistorialScoringId)
            .OnDelete(DeleteBehavior.Cascade);

        // ────────────────────────────────────────────────────────────
        // ÍNDICES
        // ────────────────────────────────────────────────────────────
        entity.HasIndex(h => h.ClienteId)
            .HasDatabaseName("IX_HistorialesScoring_ClienteId");

        entity.HasIndex(h => new { h.ClienteId, h.CalculadoEn })
            .HasDatabaseName("IX_HistorialesScoring_Cliente_Fecha");
    }
}