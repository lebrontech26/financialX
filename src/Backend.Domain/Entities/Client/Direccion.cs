namespace Backend.Domain.Entities.Client;

public class Direccion
{
    public string Calle { get; set; }
    public string Localidad { get; set; }
    public string Provincia { get; set; }

    public Direccion() { }

    public Direccion(string calle, string localidad, string provincia)
    {
        Calle = calle;
        Localidad = localidad;
        Provincia = provincia;
    }

    public override string ToString()
    {
        var partes = new[] { Calle, Localidad, Provincia }
            .Where(p => !string.IsNullOrWhiteSpace(p));

        return string.Join(", ", partes);
    }
}

