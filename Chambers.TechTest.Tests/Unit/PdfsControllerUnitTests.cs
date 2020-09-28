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
            var mockStorageClient = new Mock<IApiStorageClient>();
            mockStorageClient
                .Setup(c => c.GetAllItems(It.IsAny<string>()));
            var controller = new PdfsController(mockLogger.Object, mockStorageClient.Object);

            var result = (await controller.GetPdfs()) as ObjectResult;

            result.Value.Should().BeOfType<StorageItem[]>();
        }

        [TestMethod]
        public async Task GetPdfs_OneResult_ReturnsArrayOfStorageItems()
        {
            var testResults = new List<StorageItem> {
                new StorageItem() { FileSize = 0, Location = "location", Name = "name" }
            };
            var mockLogger = new Mock<ILogger<PdfsController>>();
            var mockStorageClient = new Mock<IApiStorageClient>();
            mockStorageClient.Setup(c => c.GetAllItems(It.IsAny<string>())).Returns(Task.FromResult(testResults.AsEnumerable()));
            var controller = new PdfsController(mockLogger.Object, mockStorageClient.Object);

            var result = (await controller.GetPdfs()) as ObjectResult;

            result.Value.Should().Be(testResults);
        }
    }
}
