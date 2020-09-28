using Chambers.TechTest.Api;
using Chambers.TechTest.BlobStorage;
using Chambers.TechTest.Common.Interfaces;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Net.Http;

namespace Chambers.TechTest.Tests.Integration
{
    /// <summary>
    /// Base class for defining api integration tests
    /// </summary>
    public class IntegrationTest
    {
        protected readonly HttpClient TestClient;

        protected IntegrationTest()
        {
            // Create an in memory api client to send test requests to
            var appFactory = new WebApplicationFactory<Startup>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        // Replace storage client service with one we can run tests against
                        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IApiRepository));
                        if (descriptor != null)
                        {
                            services.Remove(descriptor);
                        }
                        services.Add(new ServiceDescriptor(typeof(IApiRepository),
                            BlobStorageApiRepository.Init(Constants.BlobStorageConnectionString)));
                    });
                });

            TestClient = appFactory.CreateClient();
        }
    }
}
