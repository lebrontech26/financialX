namespace Backend.Domain.Entities.Client;
public enum CategoriaRiesgo
{
    Bajo = 1,
    MedioBajo = 2,
    Medio = 3,
    Alto = 4,
    Critico = 5
}


public static class CategoriaRiesgoExtensions
{
    public static string ObtenerDescripcion(this CategoriaRiesgo categoria)
    {
        return categoria switch
        {
            CategoriaRiesgo.Bajo => "Riesgo Bajo",
            CategoriaRiesgo.MedioBajo => "Riesgo Medio-Bajo",
            CategoriaRiesgo.Medio => "Riesgo Medio",
            CategoriaRiesgo.Alto => "Riesgo Alto",
            CategoriaRiesgo.Critico => "Riesgo Crítico",
            _ => "Desconocido"
        };
    }

    public static string ObtenerTextoInterpretativo(this CategoriaRiesgo categoria)
    {
        return categoria switch
        {
            CategoriaRiesgo.Bajo =>
                "El cliente presenta un perfil de riesgo bajo según su comportamiento crediticio histórico.",

            CategoriaRiesgo.MedioBajo =>
                "El cliente presenta un perfil de riesgo medio-bajo, con señales moderadas en su historial crediticio.",

            CategoriaRiesgo.Medio =>
                "El cliente presenta un perfil de riesgo medio. Se recomienda revisar alertas y antecedentes antes de avanzar.",

            CategoriaRiesgo.Alto =>
                "El cliente presenta un perfil de riesgo alto según su historial crediticio. Se recomienda cautela operativa.",

            CategoriaRiesgo.Critico =>
                "El cliente presenta un perfil de riesgo crítico. Se recomienda una validación reforzada antes de operar.",

            _ => "Sin información suficiente para evaluar el riesgo."
        };
    }

    public static CategoriaRiesgo DesdePuntaje(int puntajeFinal)
    {
        return puntajeFinal switch
        {
            >= 80 => CategoriaRiesgo.Bajo,
            >= 65 => CategoriaRiesgo.MedioBajo,
            >= 50 => CategoriaRiesgo.Medio,
            >= 35 => CategoriaRiesgo.Alto,
            _ => CategoriaRiesgo.Critico
        };
    }
}