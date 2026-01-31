using Backend.Infrastructure.BcraGateway.DTOs;
using Backend.Shared.ResultPattern;

namespace Backend.Infrastructure.BcraGateway.Services
{
    public interface IBcraDataService
    {
        Task<Result<BcraData>> GetBcraDataAsync(string cuil);
    }
}