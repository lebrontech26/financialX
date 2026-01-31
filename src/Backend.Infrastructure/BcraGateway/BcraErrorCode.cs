namespace Backend.Infrastructure.BcraGateway
{
    public enum BcraErrorCode
    {
        // Respuestas del BCRA
        SinEvidenciaCrediticia,
        ParametroErroneo,
        RespuestaVacia,
        RespuestaInvalida,

        // Errores de comunicación
        ErrorDeRed,
        Timeout,
        ServicioNoDisponible,

        // Errores de la API
        ErrorServidor,
        ErrorDesconocido,
        CuilInvalido
    }
}