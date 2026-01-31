namespace Backend.Domain.Entities.Scoring
{
    /// <summary>
    /// Tabla de ajustes (penalizaciones) al score base
    /// </summary>
    public class ReglaAjuste
    {
        public int Id { get; set; }
        public TipoAjuste TipoAjuste { get; set; }
        public int ValorMinimo { get; set; }
        public int? ValorMaximo;
        public int Ajuste { get; set; }
        public string Codigo {get; set;}
        public string Descripcion {get; set;}
        public DateTime CreadoEn {get; set;} 
        public bool EstaActivo {get; set;}
    }
}