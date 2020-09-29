using Chambers.TechTest.Api.Controllers;
using Chambers.TechTest.Common.Interfaces;
using Chambers.TechTest.Common.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chambers.TechTest.Tests.Unit
{
    /// <summary>
    /// Unit tests for PdfsController
    /// </summary>
    [TestClass]
    public class PdfsControllerUnitTests
    {
        [TestMethod]
        public async Task GetPdfs_NoResults_ReturnsArrayOfStorageItems()
        {
            var mockLogger = new Mock<ILogger<PdfsController>>();
            var mockStorageClient = new Mock<IApiRepository>();
            mockStorageClient
                .Setup(c => c.GetAllItems(It.IsAny<string>()));
            var controller = new PdfsController(mockLogger.Object, mockStorageClient.Object);

            var result = await controller.GetPdfs();

            (result.Result as ObjectResult).Value.Should().BeOfType<StoredItem[]>();
        }

        [TestMethod]
        public async Task GetPdfs_OneResult_ReturnsArrayOfStorageItems()
        {
            var testResults = new List<StoredItem> {
                new StoredItem() { FileSize = 0, Location = "location", Name = "name" }
            };
            var mockLogger = new Mock<ILogger<PdfsController>>();
            var mockStorageClient = new Mock<IApiRepository>();
            mockStorageClient.Setup(c => c.GetAllItems(It.IsAny<string>())).Returns(Task.FromResult(testResults.AsEnumerable()));
            var controller = new PdfsController(mockLogger.Object, mockStorageClient.Object);

            var result = await controller.GetPdfs();

            (result.Result as ObjectResult).Value.Should().BeEquivalentTo(testResults);
        }
    }
}
