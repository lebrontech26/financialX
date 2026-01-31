using System.ComponentModel.DataAnnotations.Schema;
namespace Backend.Domain.Entities.Client;

public class Cliente
{
    public int Id { get; set; }
    public string Cuil { get;  set; } = default!;

    public string Nombre { get; set; } = default!;
    public string Apellido { get; set; } = default!;
    public DateTime FechaNacimiento { get; set; }
    public string? Telefono { get; set; }
    public Direccion Domicilio { get; set; } = new();

    public bool EstaActivo { get; set; } = true;
    public DateTime CreadoEn { get; set; } = DateTime.UtcNow;
    public DateTime ActualizadoEn { get; set; } = DateTime.UtcNow;

    public List<HistorialScoring> HistorialesScoring { get; set; } = new();

    [NotMapped]
    public HistorialScoring? PerfilActual =>
        HistorialesScoring
            .OrderByDescending(h => h.CalculadoEn)
            .FirstOrDefault();

    [NotMapped]
    public string NombreCompleto => $"{Nombre} {Apellido}";

    public Cliente() { }
    public static Cliente Crear(
        string cuil,
        string nombre,
        string apellido,
        DateTime fechaNacimiento,
        string? telefono,
        Direccion domicilio)
    {

        return new Cliente
        {
            Cuil = cuil,
            Nombre = nombre,
            Apellido = apellido,
            FechaNacimiento = fechaNacimiento,
            Telefono = telefono,
            Domicilio = domicilio,
            EstaActivo = true,
            CreadoEn = DateTime.UtcNow,
            ActualizadoEn = DateTime.UtcNow
        };
    }

    public void ActualizarDatosPersonales(
        string nombre,
        string apellido,
        DateTime fechaNacimiento,
        string? telefono,
        Direccion domicilio)
    {
        Nombre = nombre;
        Apellido = apellido;
        FechaNacimiento = fechaNacimiento;
        Telefono = telefono;
        Domicilio = domicilio;
        ActualizadoEn = DateTime.UtcNow;
    }

    public void ModificarEstado(bool estado = true)
    {
        EstaActivo = estado;
        ActualizadoEn = DateTime.UtcNow;
    }

    public void AgregarHistorialScoring(HistorialScoring historial)
    {
        if (historial.ClienteId != Id)
            throw new InvalidOperationException("El historial no pertenece a este cliente");

        HistorialesScoring.Add(historial);
        ActualizadoEn = DateTime.UtcNow;
    }

}
