
# Cliente
Entidad principal que representa un cliente del sistema.
Reglas de negocio:
- CUIL único e inmutable (RB-01) - 11 dígitos numéricos, único, inmutable.
- Borrado lógico obligatorio (RB-02)
- El perfil actual se obtiene del historial más reciente

Direccion : Dirección como Owned Entity (se mapea en la misma tabla)

────────────────────────────────────────────────────────────
    ESTADO Y AUDITORÍA
────────────────────────────────────────────────────────────
    Borrado lógico. False = cliente inactivo/eliminado.


────────────────────────────────────────────────────────────
    RELACIONES
────────────────────────────────────────────────────────────
    Historial completo de scoring (append-only, nunca se borra)


────────────────────────────────────────────────────────────
    PROPIEDADES CALCULADAS (NO MAPEADAS A DB)
────────────────────────────────────────────────────────────
    Perfil de riesgo actual (calculado dinámicamente desde el historial más reciente).
    No se persiste en DB para evitar duplicación y mantener single source of truth.

────────────────────────────────────────────────────────────
    MÉTODOS
────────────────────────────────────────────────────────────
* Constructor vacio : Constructor para Entity Framework (requiere constructor sin parámetros)
* Factory method para crear cliente (garantiza CUIL inmutable desde construcción)
    public static Cliente Crear(...)
* Actualizar datos personales (CUIL nunca se puede modificar)
    public void ActualizarDatosPersonales(...)
* Baja lógica del cliente
    public void Desactivar()
* Agregar registro de scoring al historial
    public void AgregarHistorialScoring(HistorialScoring historial)

---
# HistoralScoring:

Registro de cálculo de scoring en un punto en el tiempo.
Append-only: nunca se modifica ni se elimina.

    public int Id { get; set; }

    ────────────────────────────────────────────────────────────
    RELACIÓN CON CLIENTE
    ────────────────────────────────────────────────────────────
    public int ClienteId { get; set; }
    public Cliente Cliente { get; set; } = default!;

    ────────────────────────────────────────────────────────────
     DATOS DEL SCORING
    ────────────────────────────────────────────────────────────
    public DateTime CalculadoEn { get; set; } = DateTime.UtcNow;

   
* Score base antes de ajustes (según tablas del BCRA)
    public int PuntajeBase { get; set; }

* Score final después de aplicar ajustes (clamped 0-100)
    public int PuntajeFinal { get; set; }


* Categoría de riesgo asignada según el score final
    public CategoriaRiesgo Categoria { get; set; }

  
* Indica si el cliente NO tenía evidencia crediticia en BCRA
    public bool SinEvidenciaCrediticia { get; set; }

   
* Snapshot del JSON raw del BCRA (para auditoría)
    public string? BcraSnapshotRawJson { get; set; }

────────────────────────────────────────────────────────────
     COLECCIONES NORMALIZADAS (1-N)
────────────────────────────────────────────────────────────
   
* Ajustes (penalizaciones/bonificaciones) aplicados al score base
    public List<AjusteScoring> Ajustes { get; set; } = new();


* Alertas generadas durante el cálculo
    public List<AlertaScoring> Alertas { get; set; } = new();

# CategoriaRiesgo

Categorías de riesgo crediticio según rangos de scoring.
Los valores numéricos corresponden al orden de severidad.

Score 80-100: Riesgo Bajo
    Bajo = 1,


Score 65-79: Riesgo Medio-Bajo
    MedioBajo = 2,

Score 50-64: Riesgo Medio
    Medio = 3,

  
Score 35-49: Riesgo Alto
    Alto = 4,

   
Score 0-34: Riesgo Crítico
    Critico = 5


* Métodos de extensión para CategoriaRiesgo
    public static class CategoriaRiesgoExtensions(...)

* Determina la categoría de riesgo según el score final
    public static CategoriaRiesgo DesdePuntaje(int puntajeFinal)

# AlertaScoring
Alerta generada durante el cálculo de scoring

# AjusteScoring
Ajuste (penalización o bonificación) aplicado al score base

    Id { get; set; }
    int HistorialScoringId { get; set; }
    HistorialScoring HistorialScoring { get; set; } = default!;


* Código del ajuste (ej: CHEQUE_RECHAZADO, MORA_RECIENTE)

    public string Codigo { get; set; } = default!;

 
* Descripción legible del ajuste
 
    public string Descripcion { get; set; } = default!;

    
* Valor del ajuste (negativo = penalización, positivo = bonificación)
  
    public int Valor { get; set; }
