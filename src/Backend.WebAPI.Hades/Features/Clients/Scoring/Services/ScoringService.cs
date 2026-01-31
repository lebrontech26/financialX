using Backend.Data.Repositories.ScoringRepository;
using Backend.Domain.Entities.Client;
using Backend.Domain.Entities.Scoring;
using Backend.Infrastructure.BcraGateway.DTOs;
using Backend.Shared.ResultPattern;
using Backend.WebAPI.Hades.Features.Clients.Scoring.DTOs;

namespace Backend.WebAPI.Hades.Features.Clients.Scoring.Services
{
    public class ScoringService : IScoringService
    {
        private readonly ILogger<ScoringService> _logger;
        private readonly IScoringRulesRepository _scoringRulesRepository;

        public ScoringService(IScoringRulesRepository scoringRulesRepository, ILogger<ScoringService> logger)
        {
            _scoringRulesRepository = scoringRulesRepository;
            _logger = logger;
        }


        private List<string> GenerarAlertas(ScoringFeaturesDTO features)
        {
            var alertas = new List<string>();

            // Alerta por situación severa actual
            if (features.MaxSituacionActual >= 4)
            {
                alertas.Add("Situación crediticia severa detectada");
            }

            // Alerta por historial severo
            if (features.PeorSituacion24m >= 5)
            {
                alertas.Add("Historial crediticio con situaciones críticas");
            }

            // Alerta por mora reciente
            if (features.RecenciaMora <= 2 && features.MesesMora24m > 0)
            {
                alertas.Add("Mora reciente detectada");
            }

            // Alerta por mora recurrente
            if (features.MesesMora24m >= 6)
            {
                alertas.Add("Mora recurrente en el historial");
            }

            // Alerta por cheques rechazados
            if (features.ChequesEventos12m >= 1)
            {
                alertas.Add($"Cheques rechazados: {features.ChequesEventos12m} evento(s)");
            }

            // Alerta por múltiples entidades
            if (features.CantidadEntidadesActual >= 4)
            {
                alertas.Add("Múltiples entidades financieras con deuda");
            }

            return alertas;
        }

        private ScoringFeaturesDTO ExtractScoringFeatures(BcraData bcraData)
        {
            var features = new ScoringFeaturesDTO
            {
                MaxSituacionActual = 0,
                CantidadEntidadesActual = 0,
                PeorSituacion24m = 0,
                MesesMora24m = 0,
                RecenciaMora = int.MaxValue,
                ChequesEventos12m = 0
            };

            // Extraer de DeudasActuales (último período)
            if (bcraData.DeudasActuales?.Results?.Periodos != null && bcraData.DeudasActuales.Results.Periodos.Count > 0)
            {
                var periodoActual = bcraData.DeudasActuales.Results.Periodos
                                    .OrderByDescending(p => p.Periodo1)
                                    .FirstOrDefault();

                if (periodoActual?.Entidades != null && periodoActual.Entidades.Any())
                {
                    var entidades = periodoActual.Entidades;
                    features.MaxSituacionActual = entidades.Max(e => e.Situacion);
                    features.CantidadEntidadesActual = entidades.Count;
                }
            }

            // Extraer de Histórico (últimos 24 meses)
            if (bcraData.Historico?.Results?.Periodos != null && bcraData.Historico.Results.Periodos.Count > 0)
            {
                var periodos = bcraData.Historico.Results.Periodos
                                .OrderByDescending(p => p.Periodo1)
                                .Take(24)
                                .ToList();


                if (periodos.Any())
                {
                    var todasLasSituaciones = periodos
                        .Where(p => p.Entidades != null && p.Entidades.Count > 0)
                        .SelectMany(p => p.Entidades)
                        .Select(e => e.Situacion)
                        .ToList();

                    features.PeorSituacion24m = todasLasSituaciones.Any() ? todasLasSituaciones.Max() : 0;

                    features.MesesMora24m = periodos
                        .Count(p => p.Entidades != null &&
                                    p.Entidades.Any(e => e.Situacion >= 2));

                    if (features.MesesMora24m > 0)
                    {
                        var indicePrimerMoraReciente = periodos
                            .Select((p, index) => new
                            {
                                Periodo = p,
                                Index = index,
                                TieneMora = p.Entidades != null && p.Entidades.Any(e => e.Situacion >= 2)
                            })
                            .Where(x => x.TieneMora)
                            .Select(x => x.Index)
                            .FirstOrDefault();

                        features.RecenciaMora = indicePrimerMoraReciente; // +1 porque los índices son 0-based
                    }
                }

            }

            // Extraer de Cheques (últimos 12 meses)
            if (bcraData.ChequesRechazados?.Results?.Causales != null && bcraData.ChequesRechazados.Results.Causales.Count > 0)
            {
                features.ChequesEventos12m = bcraData.ChequesRechazados.Results.Causales
                    .Where(c => c.Entidades != null)
                    .SelectMany(c => c.Entidades)
                    .Where(e => e.Detalle != null)
                    .SelectMany(e => e.Detalle)
                    .Count();
            }

            return features;
        }

        private CategoriaRiesgo AsignarCategoria(int scoreFinal)
        {
            return scoreFinal switch
            {
                >= 80 => CategoriaRiesgo.Bajo,
                >= 65 => CategoriaRiesgo.MedioBajo,
                >= 50 => CategoriaRiesgo.Medio,
                >= 35 => CategoriaRiesgo.Alto,
                _ => CategoriaRiesgo.Critico,
            };
        }

        public async Task<Result<ScoringResult>> CalcularScoringAsync(BcraData bcraData)
        {
            try
            {
                //CASO ESPECIAL: SIN EVIDENCIA CREDITICIA
                if (bcraData.SinEvidenciaCrediticia)
                {
                    var scoringResult = new ScoringResult
                    {
                        PuntajeBase = 65,  // Score base para sin evidencia
                        PuntajeFinal = 65,
                        Categoria = CategoriaRiesgo.MedioBajo,
                        SinEvidenciaCrediticia = true,
                        Ajustes = new List<ScoreAjustesDTO>(),
                        Alertas = new List<string> { "Cliente sin evidencia crediticia en BCRA" },
                        ScoringFeaturesDTO = new ScoringFeaturesDTO()
                    };

                    return Result<ScoringResult>.Success(scoringResult);
                }

                //PASO 1: EXTRAER FEATURES (Variables agregadas)
                var features = ExtractScoringFeatures(bcraData);

                // PASO 2: OBTENER SCORE BASE
                var scoreBase = await _scoringRulesRepository.GetScoreBaseAsync(features.MaxSituacionActual);

                if (scoreBase == 0 && features.MaxSituacionActual == 0)
                {
                    scoreBase = 65; // Asignar score base para sin evidencia
                }

                // PASO 3: APLICAR AJUSTES (Penalizaciones)
                var ajustes = new List<ScoreAjustesDTO>();
                var scoreFinal = scoreBase;


                //Peor Situación 24m
                var ajustesPeorSituacion24m = await _scoringRulesRepository.GetReglaAjusteAsync(
                                                TipoAjuste.PeorSituacion24m, 
                                                features.PeorSituacion24m);
                
                if (ajustesPeorSituacion24m is not null && ajustesPeorSituacion24m.Ajuste != 0)
                {
                    ajustes.Add(new ScoreAjustesDTO
                    {
                        Codigo = ajustesPeorSituacion24m.Codigo,
                        Descripcion = ajustesPeorSituacion24m.Descripcion,
                        Valor = ajustesPeorSituacion24m.Ajuste
                    });
                    scoreFinal += ajustesPeorSituacion24m.Ajuste;
                }

                //Meses en Mora 24m
                var ajustesMesesMora24m = await _scoringRulesRepository.GetReglaAjusteAsync(TipoAjuste.MesesMora24m, features.MesesMora24m);
                if (ajustesMesesMora24m is not null && ajustesMesesMora24m.Ajuste != 0)
                {
                    ajustes.Add(new ScoreAjustesDTO
                    {
                        Codigo = ajustesMesesMora24m.Codigo,
                        Descripcion = ajustesMesesMora24m.Descripcion,
                        Valor = ajustesMesesMora24m.Ajuste
                    });
                    scoreFinal += ajustesMesesMora24m.Ajuste;
                }

                //Recencia de Mora - Solo aplicar si hay mora
                if (features.MesesMora24m > 0)
                {
                    var ajustesRecenciaMora = await _scoringRulesRepository.GetReglaAjusteAsync(TipoAjuste.RecenciaMora, features.RecenciaMora);
                    if (ajustesRecenciaMora is not null && ajustesRecenciaMora.Ajuste != 0)
                    {
                        ajustes.Add(new ScoreAjustesDTO
                        {
                            Codigo = ajustesRecenciaMora.Codigo,
                            Descripcion = ajustesRecenciaMora.Descripcion,
                            Valor = ajustesRecenciaMora.Ajuste
                        });
                        scoreFinal += ajustesRecenciaMora.Ajuste;
                    }
                }

                //Cantidad de entidades
                var ajustesEntidades = await _scoringRulesRepository.GetReglaAjusteAsync(TipoAjuste.CantidadEntidades, features.CantidadEntidadesActual);
                if (ajustesEntidades is not null && ajustesEntidades.Ajuste != 0)
                {
                    ajustes.Add(new ScoreAjustesDTO
                    {
                        Codigo = ajustesEntidades.Codigo,
                        Descripcion = ajustesEntidades.Descripcion,
                        Valor = ajustesEntidades.Ajuste
                    });
                    scoreFinal += ajustesEntidades.Ajuste;
                }

                //Cheques Rechazados
                var ajustesCheques = await _scoringRulesRepository.GetReglaAjusteAsync(TipoAjuste.ChequesRechazados, features.ChequesEventos12m);
                if (ajustesCheques is not null && ajustesCheques.Ajuste != 0)
                {
                    ajustes.Add(new ScoreAjustesDTO
                    {
                        Codigo = ajustesCheques.Codigo,
                        Descripcion = ajustesCheques.Descripcion,
                        Valor = ajustesCheques.Ajuste
                    });
                    scoreFinal += ajustesCheques.Ajuste;
                }

                scoreFinal = Math.Clamp(scoreFinal, 0, 100);
                var categoria = AsignarCategoria(scoreFinal);
                var alertas = GenerarAlertas(features);

                var resultadoScoring = new ScoringResult
                {
                    PuntajeBase = scoreBase,
                    PuntajeFinal = scoreFinal,
                    Categoria = categoria,
                    SinEvidenciaCrediticia = false,
                    Ajustes = ajustes,
                    Alertas = alertas,
                    ScoringFeaturesDTO = features
                };
                return Result<ScoringResult>.Success(resultadoScoring);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al calcular el scoring");
                return Result<ScoringResult>.Failure(ScoringErrorCode.ErrorCalculoScoring);
            }
        }
    }
}
