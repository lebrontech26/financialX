namespace Backend.WebAPI.Hades.Features.Clients.Codes
{
    public enum CodigosCliente
    {
        cuil_nulo_o_vacio,
        cuil_contiene_letras,
        cuil_longitud_error,
        cuil_existente,
        cliente_no_encontrado,
        cliente_ya_activo,
        cliente_ya_inactivo,
        error_desconocido,
    }
}