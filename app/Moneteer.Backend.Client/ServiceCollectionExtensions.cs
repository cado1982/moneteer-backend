using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;

namespace Moneteer.Backend.Client
{
    public static class ServiceCollectionExtensions
    {
        public static void AddMoneteerApiClient(this IServiceCollection services, string baseApiUrl)
        {
            var httpClient = new HttpClient();
            services.AddTransient(f => new MoneteerApiClient(baseApiUrl, httpClient));
        }
    }
}
