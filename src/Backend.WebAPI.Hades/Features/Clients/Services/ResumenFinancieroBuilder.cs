using Backend.Domain.Entities.Client;
using Backend.WebAPI.Hades.Features.Clients.DTOs.Get;

namespace Backend.WebAPI.Hades.Features.Clients.Services;

public class ResumenFinancieroBuilder
{
    public ResumenFinancieroDTO Build(FeaturesBcra features, bool sinEvidenciaCrediticia)
    {
        var items = new List<ResumenFinancieroItemDTO>();

        // Si no hay evidencia crediticia, retornar mensaje especial
        if (sinEvidenciaCrediticia)
        {
            items.Add(new ResumenFinancieroItemDTO
            {
                Titulo = "Situacion crediticia actual",
                Texto = "Sin evidencia crediticia registrada en el sistema financiero.",
                Icono = "info"
            });
            return new ResumenFinancieroDTO { Items = items };
        }

        // 1. Situacion crediticia actual
        items.Add(new ResumenFinancieroItemDTO
        {
            Titulo = "Situacion crediticia actual",
            Texto = InterpretarSituacionActual(features.MaxSituacionActual),
            Icono = ObtenerIconoSituacion(features.MaxSituacionActual)
        });

        // 2. Historial crediticio (24 meses)
        items.Add(new ResumenFinancieroItemDTO
        {
            Titulo = "Historial crediticio (ultimos 24 meses)",
            Texto = InterpretarHistorial24Meses(features.PeorSituacion24m, features.MesesMora24m),
            Icono = ObtenerIconoHistorial(features.PeorSituacion24m, features.MesesMora24m)
        });

        // 3. Recencia de mora
        items.Add(new ResumenFinancieroItemDTO
        {
            Titulo = "Recencia de la mora",
            Texto = InterpretarRecenciaMora(features.RecenciaMora, features.MesesMora24m),
            Icono = ObtenerIconoRecencia(features.RecenciaMora, features.MesesMora24m)
        });

        // 4. Relacion con entidades financieras
        items.Add(new ResumenFinancieroItemDTO
        {
            Titulo = "Relacion con entidades financieras",
            Texto = InterpretarEntidades(features.CantidadEntidadesActual),
            Icono = ObtenerIconoEntidades(features.CantidadEntidadesActual)
        });

        // 5. Cheques rechazados
        items.Add(new ResumenFinancieroItemDTO
        {
            Titulo = "Cheques rechazados",
            Texto = InterpretarCheques(features.ChequesEventos12m),
            Icono = ObtenerIconoCheques(features.ChequesEventos12m)
        });

        return new ResumenFinancieroDTO { Items = items };
    }

    private static string InterpretarSituacionActual(int maxSituacion) => maxSituacion switch
    {
        0 => "Sin deudas registradas en el sistema financiero.",
        1 => "Situacion normal, sin incidencias relevantes.",
        2 => "Riesgo bajo con senales menores en el comportamiento crediticio.",
        3 => "Dificultades parciales detectadas en el historial reciente.",
        _ => "Compromiso alto, se observan incidencias significativas."
    };

    private static string InterpretarHistorial24Meses(int peorSituacion, int mesesMora)
    {
        if (peorSituacion <= 1 && mesesMora == 0)
            return "Historial limpio sin incidencias en los ultimos 24 meses.";

        if (peorSituacion <= 2 && mesesMora <= 3)
            return "Historial con incidencias menores, comportamiento mayormente estable.";

        if (peorSituacion <= 4 && mesesMora <= 6)
            return "Se detectan dificultades parciales en el historial reciente.";

        return "Historial con situaciones criticas o mora recurrente.";
    }

    private static string InterpretarRecenciaMora(int recencia, int mesesMora)
    {
        if (mesesMora == 0)
            return "No se registra mora en el historial reciente.";

        if (recencia <= 2)
            return "Mora detectada en los ultimos 2 meses.";

        if (recencia <= 6)
            return $"Ultima mora registrada hace {recencia} meses.";

        return "Ultima mora registrada hace mas de 6 meses.";
    }

    private static string InterpretarEntidades(int cantidad) => cantidad switch
    {
        0 => "Sin relacion con entidades financieras registradas.",
        1 => "Relacion con 1 entidad financiera.",
        2 or 3 => $"Relacion con {cantidad} entidades financieras.",
        _ => $"Relacion con multiples entidades ({cantidad}), exposicion diversificada."
    };

    private static string InterpretarCheques(int cantidad) => cantidad switch
    {
        0 => "No se registran cheques rechazados.",
        1 or 2 => $"Se registran {cantidad} cheques rechazados en los ultimos 12 meses.",
        _ => $"Se registran {cantidad} cheques rechazados, revisar situacion bancaria."
    };

    // Iconos opcionales para el frontend
    private static string ObtenerIconoSituacion(int maxSituacion) => maxSituacion switch
    {
        0 or 1 => "success",
        2 => "info",
        3 => "warning",
        _ => "danger"
    };

    private static string ObtenerIconoHistorial(int peorSituacion, int mesesMora)
    {
        if (peorSituacion <= 1 && mesesMora == 0) return "success";
        if (peorSituacion <= 2 && mesesMora <= 3) return "info";
        if (peorSituacion <= 4 && mesesMora <= 6) return "warning";
        return "danger";
    }

    private static string ObtenerIconoRecencia(int recencia, int mesesMora)
    {
        if (mesesMora == 0) return "success";
        if (recencia <= 2) return "danger";
        if (recencia <= 6) return "warning";
        return "info";
    }

    private static string ObtenerIconoEntidades(int cantidad) => cantidad switch
    {
        0 or 1 => "success",
        2 or 3 => "info",
        _ => "warning"
    };

    private static string ObtenerIconoCheques(int cantidad) => cantidad switch
    {
        0 => "success",
        1 or 2 => "warning",
        _ => "danger"
    };
}
