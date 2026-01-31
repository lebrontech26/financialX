using Backend.WebAPI.Hades.Features.Clients.Codes;

namespace Backend.WebAPI.Hades.Features.Clients.FriendlyMessages
{
    public static class ClienteDictionary
    {
        public static readonly Dictionary<CodigosCliente, string> Messages = new()
        {
            { CodigosCliente.cuil_nulo_o_vacio, "El CUIL es obligatorio." },
            { CodigosCliente.cuil_contiene_letras, "El CUIL debe contener solo números." },
            { CodigosCliente.cuil_longitud_error, "El CUIL debe tener 11 dígitos." },
            { CodigosCliente.cuil_existente, "Ya existe un cliente registrado con ese CUIL." },
            { CodigosCliente.cliente_no_encontrado, "El cliente no existe"},
            { CodigosCliente.cliente_ya_inactivo, "El cliente ya se encuentra inactivo." },
            { CodigosCliente.cliente_ya_activo, "El cliente ya se encuentra activo." },
            { CodigosCliente.error_desconocido, "Ocurrió un error inesperado. Por favor, intente nuevamente." }
        };
    }
}