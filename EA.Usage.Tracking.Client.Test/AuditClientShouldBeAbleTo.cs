using NUnit.Framework;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace EA.Usage.Tracking.Client.Test
{
    [TestFixture]
    public class AuditClientShouldBeAbleTo
    {
        [Test]
        public async Task GetAccessToken()
        {
            var services = new ServiceCollection();
            services.AddHttpClient();
            services.AddScoped<IAuthClient, CognitoClient>(provider =>
                new CognitoClient(provider.GetRequiredService<IHttpClientFactory>(), 
                    provider.GetRequiredService<IMemoryCache>(), 
                    provider.GetRequiredService<IOptions<CognitoSettings>>()));


            var servicesProvider = services.BuildServiceProvider();

            var auditClient = servicesProvider.GetRequiredService<IAuthClient>();

            var token = await auditClient.GetAccessToken();

        }

    }
}
