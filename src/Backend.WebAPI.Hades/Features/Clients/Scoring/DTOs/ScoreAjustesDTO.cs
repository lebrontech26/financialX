namespace Backend.WebAPI.Hades.Features.Clients.Scoring.DTOs
{
    public class ScoreAjustesDTO
    {
        public string Codigo {get; init;} = default!;
        public string Descripcion {get; init;} = default!;
        public int Valor {get; init;}
    }
}