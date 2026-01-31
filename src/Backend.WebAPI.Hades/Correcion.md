 # Plan de Modificaciones - Financial Summary para Frontend                                               
              
  El frontend espera un objeto `financialSummary` en la respuesta de `GET /api/Client/{id}` para mostrar   
  las tarjetas de información financiera. Actualmente el backend ya obtiene esta información del BCRA pero no la está mapeando al DTO de respuesta.

  ---

  ## Datos Disponibles vs Requeridos

  | Card del Frontend | Dato del BCRA | Variable en ScoringFeaturesDTO | Estado |
  |-------------------|---------------|-------------------------------|--------|
  | Situación crediticia actual | `Deuda.Entidad.Situacion` | `MaxSituacionActual` | ✅ Extraído, no       
  mapeado |
  | Historial crediticio (24 meses) | `Historicas.Periodos[].Situacion` | `PeorSituacion24m`,
  `MesesMora24m` | ✅ Extraído, no mapeado |
  | Recencia de la mora | Índice del período con mora | `RecenciaMora` | ✅ Extraído, no mapeado |
  | Relación con entidades | Cantidad de entidades | `CantidadEntidadesActual` | ✅ Extraído, no mapeado | 
  | Cheques rechazados | Cantidad de eventos | `ChequesEventos12m` | ✅ Extraído, no mapeado |

  ---

  ## Archivos a Modificar

  ### 1. Backend.WebAPI.Hades - DTOs

  **Archivo:** `src/Backend.WebAPI.Hades/DTOs/Clientes/ClienteDetalleResponse.cs`

  **Acción:** Agregar propiedad `FinancialSummary`

  ```csharp
  // Agregar al DTO existente
  public FinancialSummaryDTO? FinancialSummary { get; set; }

  ---
  2. Backend.WebAPI.Hades - Nuevo DTO

  Archivo a crear: src/Backend.WebAPI.Hades/DTOs/Clientes/FinancialSummaryDTO.cs

  Contenido:

  namespace Backend.WebAPI.Hades.DTOs.Clientes;

  public class FinancialSummaryDTO
  {
      public List<FinancialSummaryItemDTO> Items { get; set; } = new();
  }

  public class FinancialSummaryItemDTO
  {
      public string Title { get; set; } = string.Empty;
      public string Text { get; set; } = string.Empty;
      public string? Badge { get; set; }
  }

  ---
  3. Domain - Nuevo Servicio de Interpretación

  Archivo a crear: src/Backend.Domain/Services/FinancialSummaryBuilder.cs

  Propósito: Convertir los valores numéricos del scoring en textos interpretados

  Métodos a implementar:

  public class FinancialSummaryBuilder
  {
      // Construir resumen completo
      public FinancialSummary Build(ScoringFeaturesDTO features, bool sinEvidencia);

      // Interpretar situación crediticia (1-6) → texto
      private string InterpretarSituacionActual(int maxSituacion);

      // Interpretar historial 24 meses → texto
      private string InterpretarHistorial24Meses(int peorSituacion, int mesesMora);

      // Interpretar recencia de mora → texto
      private string InterpretarRecenciaMora(int recencia, int mesesMora);

      // Interpretar cantidad de entidades → texto
      private string InterpretarEntidades(int cantidad);

      // Interpretar cheques rechazados → texto
      private string InterpretarCheques(int cantidad);
  }

  Lógica de interpretación sugerida:
  ┌────────────────┬─────────────────────┬────────────────────────────────────────────────────────────────┐
  │     Campo      │        Valor        │                       Texto Interpretado                       │
  ├────────────────┼─────────────────────┼────────────────────────────────────────────────────────────────┤
  │ Situación      │ 1                   │ "Situación normal, sin incidencias relevantes."                │
  │ Actual         │                     │                                                                │
  ├────────────────┼─────────────────────┼────────────────────────────────────────────────────────────────┤
  │                │ 2                   │ "Riesgo bajo con señales menores en el comportamiento          │
  │                │                     │ crediticio."                                                   │
  ├────────────────┼─────────────────────┼────────────────────────────────────────────────────────────────┤
  │                │ 3                   │ "Dificultades parciales detectadas en el historial reciente."  │
  ├────────────────┼─────────────────────┼────────────────────────────────────────────────────────────────┤
  │                │ 4+                  │ "Compromiso alto, se observan incidencias significativas."     │
  ├────────────────┼─────────────────────┼────────────────────────────────────────────────────────────────┤
  │                │ 0/null              │ "Sin deudas registradas en el sistema financiero."             │
  ├────────────────┼─────────────────────┼────────────────────────────────────────────────────────────────┤
  │ Historial 24m  │ peor=1, meses=0     │ "Historial limpio sin incidencias en los últimos 24 meses."    │
  ├────────────────┼─────────────────────┼────────────────────────────────────────────────────────────────┤
  │                │ peor<=2, meses<=3   │ "Historial con incidencias menores, comportamiento mayormente  │
  │                │                     │ estable."                                                      │
  ├────────────────┼─────────────────────┼────────────────────────────────────────────────────────────────┤
  │                │ peor<=4, meses<=6   │ "Se detectan dificultades parciales en el historial reciente." │
  ├────────────────┼─────────────────────┼────────────────────────────────────────────────────────────────┤
  │                │ peor>=5 o meses>6   │ "Historial con situaciones críticas o mora recurrente."        │
  ├────────────────┼─────────────────────┼────────────────────────────────────────────────────────────────┤
  │ Recencia Mora  │ recencia=0 (sin     │ "No se registra mora en el historial reciente."                │
  │                │ mora)               │                                                                │
  ├────────────────┼─────────────────────┼────────────────────────────────────────────────────────────────┤
  │                │ recencia<=2         │ "Mora detectada en los últimos 2 meses."                       │
  ├────────────────┼─────────────────────┼────────────────────────────────────────────────────────────────┤
  │                │ recencia<=6         │ "Última mora registrada hace {recencia} meses."                │
  ├────────────────┼─────────────────────┼────────────────────────────────────────────────────────────────┤
  │                │ recencia>6          │ "Última mora registrada hace más de 6 meses."                  │
  ├────────────────┼─────────────────────┼────────────────────────────────────────────────────────────────┤
  │ Entidades      │ 0                   │ "Sin relación con entidades financieras registradas."          │
  ├────────────────┼─────────────────────┼────────────────────────────────────────────────────────────────┤
  │                │ 1                   │ "Relación con 1 entidad financiera."                           │
  ├────────────────┼─────────────────────┼────────────────────────────────────────────────────────────────┤
  │                │ 2-3                 │ "Relación con {n} entidades financieras."                      │
  ├────────────────┼─────────────────────┼────────────────────────────────────────────────────────────────┤
  │                │ 4+                  │ "Relación con múltiples entidades ({n}), exposición            │
  │                │                     │ diversificada."                                                │
  ├────────────────┼─────────────────────┼────────────────────────────────────────────────────────────────┤
  │ Cheques        │ 0                   │ "No se registran cheques rechazados."                          │
  ├────────────────┼─────────────────────┼────────────────────────────────────────────────────────────────┤
  │                │ 1-2                 │ "Se registran {n} cheques rechazados en los últimos 12 meses." │
  ├────────────────┼─────────────────────┼────────────────────────────────────────────────────────────────┤
  │                │ 3+                  │ "Se registran {n} cheques rechazados, revisar situación        │
  │                │                     │ bancaria."                                                     │
  └────────────────┴─────────────────────┴────────────────────────────────────────────────────────────────┘
  ---
  4. Domain - Persistir ScoringFeatures

  Problema: Actualmente ScoringFeaturesDTO se usa solo para calcular el score y luego se descarta. No se   
  persiste en la BD.

  Opción A (Recomendada): Persistir features en HistorialScoring

  Archivo: src/Backend.Domain/Entities/HistorialScoring.cs

  Agregar propiedades:

  // Métricas del BCRA (para reconstruir financial summary)
  public int MaxSituacionActual { get; set; }
  public int CantidadEntidadesActual { get; set; }
  public int PeorSituacion24m { get; set; }
  public int MesesMora24m { get; set; }
  public int RecenciaMora { get; set; }
  public int ChequesEventos12m { get; set; }

  Archivo: src/Backend.Data/Configurations/HistorialScoringConfiguration.cs

  Agregar mappings EF:

  builder.Property(h => h.MaxSituacionActual);
  builder.Property(h => h.CantidadEntidadesActual);
  builder.Property(h => h.PeorSituacion24m);
  builder.Property(h => h.MesesMora24m);
  builder.Property(h => h.RecenciaMora);
  builder.Property(h => h.ChequesEventos12m);

  ---
  Opción B (Alternativa): Recalcular desde ajustes

  Si no quieres agregar columnas a la BD, podrías inferir algunos valores desde los ajustes existentes:    
  ┌─────────────────┬──────────────────────────────────┐
  │ Ajuste Guardado │         Valor Inferible          │
  ├─────────────────┼──────────────────────────────────┤
  │ PEOR_SIT_24M_*  │ PeorSituacion24m (parcial)       │
  ├─────────────────┼──────────────────────────────────┤
  │ MESES_MORA_*    │ MesesMora24m (rangos)            │
  ├─────────────────┼──────────────────────────────────┤
  │ MORA_*_RECIENTE │ RecenciaMora (parcial)           │
  ├─────────────────┼──────────────────────────────────┤
  │ ENTIDADES_*     │ CantidadEntidadesActual (rangos) │
  └─────────────────┴──────────────────────────────────┘
  ⚠️ Limitación: Los ajustes guardan rangos, no valores exactos. Perderías precisión.

  ---
  5. Backend.Domain - Modificar ScoringService

  Archivo: src/Backend.Domain/Services/ScoringService.cs

  Modificar CalcularScoringAsync para incluir features en el resultado:

  // En ScoringResult, agregar:
  public ScoringFeaturesDTO Features { get; set; }

  // En el método, antes de retornar:
  result.Features = features;

  ---
  6. Backend.Domain - Modificar ClienteService

  Archivo: src/Backend.Domain/Services/ClienteService.cs

  Modificar CrearClienteAsync para guardar features:

  // Al crear HistorialScoring, agregar:
  var historial = new HistorialScoring
  {
      // ... propiedades existentes ...
      MaxSituacionActual = scoringResult.Features.MaxSituacionActual,
      CantidadEntidadesActual = scoringResult.Features.CantidadEntidadesActual,
      PeorSituacion24m = scoringResult.Features.PeorSituacion24m,
      MesesMora24m = scoringResult.Features.MesesMora24m,
      RecenciaMora = scoringResult.Features.RecenciaMora,
      ChequesEventos12m = scoringResult.Features.ChequesEventos12m
  };

  ---
  7. Backend.WebAPI.Hades - Modificar Controller/Mapper

  Archivo: src/Backend.WebAPI.Hades/Controllers/ClientController.cs o donde mapees la respuesta

  En el método GetClienteConDetalles:

  // Al construir ClienteDetalleResponse:
  var summaryBuilder = new FinancialSummaryBuilder();
  var perfilActual = cliente.HistorialesScoring.OrderByDescending(h => h.CalculadoEn).FirstOrDefault();    

  if (perfilActual != null)
  {
      var features = new ScoringFeaturesDTO
      {
          MaxSituacionActual = perfilActual.MaxSituacionActual,
          CantidadEntidadesActual = perfilActual.CantidadEntidadesActual,
          PeorSituacion24m = perfilActual.PeorSituacion24m,
          MesesMora24m = perfilActual.MesesMora24m,
          RecenciaMora = perfilActual.RecenciaMora,
          ChequesEventos12m = perfilActual.ChequesEventos12m
      };

      response.FinancialSummary = summaryBuilder.Build(features, perfilActual.SinEvidenciaCrediticia);     
  }

  ---
  Migración de BD Requerida

  Si eliges la Opción A (persistir features), necesitarás una migración:

  dotnet ef migrations add AddScoringFeaturesToHistorial -p src/Backend.Data -s src/Backend.WebAPI.Hades   
  dotnet ef database update -p src/Backend.Data -s src/Backend.WebAPI.Hades

  Columnas nuevas en HistorialScoring:
  - MaxSituacionActual (int)
  - CantidadEntidadesActual (int)
  - PeorSituacion24m (int)
  - MesesMora24m (int)
  - RecenciaMora (int)
  - ChequesEventos12m (int)

  ---
  Respuesta API Resultante

  {
    "id": 3,
    "cuil": "20397278973",
    "nombre": "Enzo",
    "apellido": "Peraalta",
    "perfilActual": {
      "puntajeFinal": 0,
      "categoria": 5,
      "sinEvidenciaCrediticia": false,
      "calculadoEn": "2026-01-22T03:55:49"
    },
    "financialSummary": {
      "items": [
        {
          "title": "Situacion crediticia actual",
          "text": "Compromiso alto, se observan incidencias significativas.",
          "badge": null
        },
        {
          "title": "Historial crediticio (ultimos 24 meses)",
          "text": "Historial con situaciones críticas o mora recurrente.",
          "badge": null
        },
        {
          "title": "Recencia de la mora",
          "text": "Mora detectada en los últimos 2 meses.",
          "badge": null
        },
        {
          "title": "Relacion con entidades financieras",
          "text": "Relación con 2-3 entidades financieras.",
          "badge": null
        },
        {
          "title": "Cheques rechazados",
          "text": "No se registran cheques rechazados.",
          "badge": null
        }
      ]
    },
    "historialScoring": [...]
  }

  ---
  Resumen de Cambios
  ┌────────┬─────────────────────────────────────────────────┬───────────────────────────────┐
  │  Capa  │                     Archivo                     │            Acción             │
  ├────────┼─────────────────────────────────────────────────┼───────────────────────────────┤
  │ WebAPI │ DTOs/Clientes/FinancialSummaryDTO.cs            │ Crear                         │
  ├────────┼─────────────────────────────────────────────────┼───────────────────────────────┤
  │ WebAPI │ DTOs/Clientes/ClienteDetalleResponse.cs         │ Agregar propiedad             │
  ├────────┼─────────────────────────────────────────────────┼───────────────────────────────┤
  │ WebAPI │ Controllers/ClientController.cs                 │ Construir summary             │
  ├────────┼─────────────────────────────────────────────────┼───────────────────────────────┤
  │ Domain │ Services/FinancialSummaryBuilder.cs             │ Crear                         │
  ├────────┼─────────────────────────────────────────────────┼───────────────────────────────┤
  │ Domain │ Services/ScoringService.cs                      │ Exponer features en resultado │
  ├────────┼─────────────────────────────────────────────────┼───────────────────────────────┤
  │ Domain │ Services/ClienteService.cs                      │ Guardar features en historial │
  ├────────┼─────────────────────────────────────────────────┼───────────────────────────────┤
  │ Domain │ Entities/HistorialScoring.cs                    │ Agregar 6 propiedades         │
  ├────────┼─────────────────────────────────────────────────┼───────────────────────────────┤
  │ Data   │ Configurations/HistorialScoringConfiguration.cs │ Mapear columnas               │
  ├────────┼─────────────────────────────────────────────────┼───────────────────────────────┤
  │ Data   │ Migración EF                                    │ Ejecutar                      │
  └────────┴─────────────────────────────────────────────────┴───────────────────────────────┘
  ---
  Nota sobre Clientes Existentes

  Los clientes creados antes de esta modificación tendrán FinancialSummary = null porque no tienen las     
  features guardadas. Opciones:

  1. Aceptar null: El frontend ya maneja el fallback con "Sin datos suficientes"
  2. Recalcular: Crear un script que vuelva a consultar BCRA para clientes existentes
  3. Inferir: Intentar reconstruir features desde los ajustes (impreciso)

  La opción 1 es la más simple y el frontend ya la soporta.