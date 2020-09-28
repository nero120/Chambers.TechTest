using Chambers.TechTest.BlobStorage;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Threading.Tasks;

namespace Chambers.TechTest.Tests.Integration
{
    /// <summary>
    /// Integration tests for api pdfs resource
    /// </summary>
    [TestClass]
    public class PdfsControllerIntegrationTests : IntegrationTest
    {
        [ClassInitialize]
        public async static Task ClassInitialize(TestContext context)
        {
            // Remove the existing pdfs container before running tests
            var storage = BlobStorageClient.Init(Constants.BlobStorageConnectionString);
            await storage.DeleteContainer(BlobStorage.Constants.PdfsContainerName);
        }

        [TestMethod]
        public async Task GetPdfs_ReturnsStatusCode200()
        {
            const string API_URL = "api/pdfs";

            var response = await TestClient.GetAsync(API_URL);
            
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [TestMethod]
        public async Task GetPdfs_ReturnsEmptyArray()
        {
            const string API_URL = "api/pdfs";

            var response = await TestClient.GetAsync(API_URL);
            var body = await response.Content.ReadAsStringAsync();

            body.Should().Be("[]");
        }
    }
}
