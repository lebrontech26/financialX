using Backend.Infrastructure.BcraGateway.DTOs;
using Backend.Infrastructure.BcraGateway.Http;
using Backend.Shared.ResultPattern;
using Microsoft.Extensions.Logging;

namespace Backend.Infrastructure.BcraGateway.Services
{
    public class BcraDataService : IBcraDataService
    {
        private readonly ILogger<BcraDataService> _logger;
        private readonly IBcraApiClient _bcraClient;

        public BcraDataService(ILogger<BcraDataService> logger, IBcraApiClient bcraClient)
        {
            _logger = logger;
            _bcraClient = bcraClient;
        }

        public async Task<Result<BcraData>> GetBcraDataAsync(string cuil)
        {
            try
            {
                var deudasTask = _bcraClient.GetDeudasAsync(cuil);
                var historicoTask = _bcraClient.GetHistoricasAsync(cuil);
                var chequesTask = _bcraClient.GetChequesRechazadosAsync(cuil);

                await Task.WhenAll(deudasTask, historicoTask, chequesTask);

                var deudasResult = await deudasTask;
                var historicoResult = await historicoTask;
                var chequesResult = await chequesTask;

                _logger.LogInformation(
                    @"Resultados BCRA para {Cuil} - 
                    Deudas: {DeudasOk}, 
                    Histórico: {HistoricoOk}, 
                    Cheques: {ChequesOk}",
                    cuil,
                    deudasResult.IsSuccess,
                    historicoResult.IsSuccess,
                    chequesResult.IsSuccess);

                var sinEvidenciaCrediticia = !deudasResult.IsSuccess && !historicoResult.IsSuccess && !chequesResult.IsSuccess;

                if (sinEvidenciaCrediticia)
                {
                    _logger.LogWarning("No credit evidence found for CUIL {Cuil}", cuil);
                }

                var bcraData = new BcraData
                {
                    DeudasActuales = deudasResult.IsSuccess ? deudasResult.Value : null,
                    Historico = historicoResult.IsSuccess ? historicoResult.Value : null,
                    ChequesRechazados = chequesResult.IsSuccess ? chequesResult.Value : null,
                    SinEvidenciaCrediticia = sinEvidenciaCrediticia
                };

                return Result<BcraData>.Success(bcraData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error retrieving BCRA data for CUIL {Cuil}", cuil);
                return Result<BcraData>.Failure(BcraErrorCode.ErrorDesconocido);
            }
        }
    }
}