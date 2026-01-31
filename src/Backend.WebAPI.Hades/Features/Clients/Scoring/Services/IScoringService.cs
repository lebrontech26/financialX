using Backend.Infrastructure.BcraGateway.DTOs;
using Backend.Shared.ResultPattern;
using Backend.WebAPI.Hades.Features.Clients.Scoring.DTOs;

namespace Backend.WebAPI.Hades.Features.Clients.Scoring.Services
{
    public interface IScoringService
    {
        Task<Result<ScoringResult>> CalcularScoringAsync(BcraData bcraData);
        
    }

}