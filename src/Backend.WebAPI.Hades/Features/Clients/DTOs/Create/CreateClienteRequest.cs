
using System.ComponentModel.DataAnnotations;

namespace Backend.WebAPI.Hades.Features.Clients.DTOs.Create
{
    public class CreateClienteRequest
    {
        [Required(ErrorMessage = "El CUIL es requerido.")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "El CUIL debe tener 11 dígitos.")]
        public string Cuil { get; set; } = default!;

        [Required(ErrorMessage = "El nombre es requerido.")]
        public string Nombre { get; set; } = default!;

        [Required(ErrorMessage = "El apellido es requerido.")]
        public string Apellido { get; set; } = default!;

        [Required(ErrorMessage = "La fecha de nacimiento es requerida.")]
        [Range(typeof(DateTime), "1900-01-01", "2100-12-31", ErrorMessage = "La fecha de nacimiento es inválida.")]
        public DateTime FechaNacimiento { get; set; }

        [Required(ErrorMessage = "El teléfono es requerido.")]
        public string Telefono { get; set; } = default!;

        [Required(ErrorMessage = "La calle es requerida.")]
        public string Calle { get; set; } = default!;

        [Required(ErrorMessage = "La localidad es requerida.")]
        public string Localidad { get; set; } = default!;

        [Required(ErrorMessage = "La provincia es requerida.")]
        public string Provincia { get; set; } = default!;
    }
}