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
    public class PdfsControllerTests : IntegrationTest
    {
        [ClassInitialize]
        public async static Task ClassInitialize(TestContext context)
        {
            // Remove the existing pdfs container before running tests
            var storage = BlobStorageClient.Init(Constants.BlobStorageConnectionString);
            await storage.DeleteContainer(BlobStorage.Constants.PdfsContainerName);
        }
        
        [TestMethod]
        public async Task GetPdfsReturns200()
        {
            var response = await TestClient.GetAsync("api/pdfs");
            var body = await response.Content.ReadAsStringAsync();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            body.Should().Be("[]");
        }
    }
}
