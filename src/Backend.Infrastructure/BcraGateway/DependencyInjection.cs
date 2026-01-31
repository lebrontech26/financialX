using Backend.Infrastructure.BcraGateway.Http;
using Backend.Infrastructure.BcraGateway.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Backend.Infrastructure.BcraGateway
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBcraGateway(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<BcraOptions>(
                configuration.GetSection(BcraOptions.SectionName)
            );

            services.AddHttpClient<IBcraApiClient, BcraApiClient>((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<BcraOptions>>().Value;
                client.BaseAddress = new Uri(options.BaseUrl);
                client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            return services;
        }
    }

}