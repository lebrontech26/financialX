namespace Backend.WebAPI.Hades.Features.Clients.DTOs.Get
{
    public class ResumenFinancieroDTO
    {
        public List<ResumenFinancieroItemDTO> Items {get; set;} = [];
    }

    public class ResumenFinancieroItemDTO
    {
        public string Titulo {get; set;} = string.Empty;
        public string Texto {get; set;} = string.Empty;
        public string? Icono {get; set;}
    }
}