using Backend.Domain.Entities.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Backend.Data.Configurations;
public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> entity)
    {
        entity.ToTable("Clientes");
        entity.HasKey(c => c.Id);

        // ────────────────────────────────────────────────────────────
        // CUIL: ÚNICO E INMUTABLE (RB-01)
        // ────────────────────────────────────────────────────────────
        entity.HasIndex(c => c.Cuil)
            .IsUnique()
            .HasDatabaseName("IX_Clientes_Cuil_Unique");

        entity.Property(c => c.Cuil)
            .IsRequired()
            .HasMaxLength(11);

        // ────────────────────────────────────────────────────────────
        // DATOS PERSONALES
        // ────────────────────────────────────────────────────────────
        entity.Property(c => c.Nombre)
            .IsRequired()
            .HasMaxLength(100);

        entity.Property(c => c.Apellido)
            .IsRequired()
            .HasMaxLength(100);

        entity.Property(c => c.FechaNacimiento)
            .IsRequired();

        entity.Property(c => c.Telefono)
            .HasMaxLength(20);

        // ────────────────────────────────────────────────────────────
        // DIRECCIÓN COMO OWNED ENTITY (se mapea en la misma tabla)
        // ────────────────────────────────────────────────────────────
        entity.OwnsOne(c => c.Domicilio, domicilio =>
        {
            domicilio.Property(d => d.Calle)
                .HasColumnName("Domicilio_Calle")
                .IsRequired()
                .HasMaxLength(200);

            domicilio.Property(d => d.Localidad)
                .HasColumnName("Domicilio_Localidad")
                .IsRequired()
                .HasMaxLength(100);

            domicilio.Property(d => d.Provincia)
                .HasColumnName("Domicilio_Provincia")
                .IsRequired()
                .HasMaxLength(100);
        });

        // ────────────────────────────────────────────────────────────
        // ESTADO Y AUDITORÍA
        // ────────────────────────────────────────────────────────────
        entity.Property(c => c.EstaActivo)
            .IsRequired()
            .HasDefaultValue(true);

        entity.Property(c => c.CreadoEn)
            .IsRequired();

        entity.Property(c => c.ActualizadoEn)
            .IsRequired();

        // ────────────────────────────────────────────────────────────
        // ÍNDICES PARA OPTIMIZACIÓN DE QUERIES
        // ────────────────────────────────────────────────────────────

        // Para filtrar clientes activos en el listado principal
        entity.HasIndex(c => c.EstaActivo)
            .HasDatabaseName("IX_Clientes_EstaActivo");

        // Para búsquedas por nombre/apellido (RF-BE-01: búsqueda server-side)
        entity.HasIndex(c => new { c.Nombre, c.Apellido })
            .HasDatabaseName("IX_Clientes_NombreApellido");

        // Para filtrar activos + ordenar
        entity.HasIndex(c => new { c.EstaActivo, c.CreadoEn })
            .HasDatabaseName("IX_Clientes_EstaActivo_CreadoEn");

        // ────────────────────────────────────────────────────────────
        // RELACIÓN CON HISTORIAL (1-N)
        // ────────────────────────────────────────────────────────────
        entity.HasMany(c => c.HistorialesScoring)
            .WithOne(h => h.Cliente)
            .HasForeignKey(h => h.ClienteId)
            .OnDelete(DeleteBehavior.Restrict); // No permitir borrar cliente si tiene historial

        entity.Navigation(c => c.Domicilio).IsRequired();
    }
}
