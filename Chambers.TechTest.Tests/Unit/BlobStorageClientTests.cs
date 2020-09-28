using Chambers.TechTest.BlobStorage;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chambers.TechTest.Tests
{
    /// <summary>
    /// Unit tests for BlobStorageClient
    /// </summary>
    [TestClass]
    public class BlobStorageClientTests
    {
        [TestMethod]
        public void InitReturnsNewBlobStorageClient()
        {
            var client = BlobStorageClient.Init(Constants.BlobStorageConnectionString);
            client.Should().BeOfType<BlobStorageClient>().And.NotBeNull();
        }
    }
}
