using Chambers.TechTest.Api.Models;
using Chambers.TechTest.BlobStorage;
using Chambers.TechTest.Common.Interfaces;
using Chambers.TechTest.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chambers.TechTest.Api.Controllers
{
    /// <summary>
    /// API operations for working with PDFs.
    /// </summary>
    [Route("api/pdfs")]
    [ApiController]
    public class PdfsController : ControllerBase
    {
        readonly ILogger _logger;
        readonly IApiRepository _storage;

        public PdfsController(ILogger<PdfsController> logger, IApiRepository storage)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
        }

        /// <summary>
        /// List all existing pdfs
        /// </summary>
        /// <returns>A list of pdf item metadata</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StoredApiItem>>> GetPdfs()
        {
            var blobs = await _storage.GetAllItems(Constants.PdfsContainerName);
            return Ok(blobs);
        }

        /// <summary>
        /// Retrieve a specific pdf
        /// </summary>
        /// <param name="id">The location guid of the pdf to retrieve</param>
        /// <returns>The requested pdf item metadata</returns>
        [HttpGet("{id}", Name = "GetPdf")]
        public async Task<ActionResult<StoredApiItem>> GetPdf(string id)
        {
            // If the id is not valid return 404
            Guid guid;
            if (!Guid.TryParse(id, out guid))
            {
                return NotFound();
            }

            // Retrieve the item from storage
            var blob = await _storage.GetItem(guid, Constants.PdfsContainerName);
            if (blob == null)
            {
                return NotFound();
            }

            return Ok(blob);
        }

        /// <summary>
        /// Downloads a specific pdf
        /// </summary>
        /// <param name="id">The location guid of the pdf to retrieve</param>
        /// <returns>The pdf binary data as an octet stream</returns>
        [HttpGet("{id}/download")]
        public async Task<ActionResult<FileContentResult>> DownloadPdf(string id)
        {
            // If the id is not valid return 404
            Guid guid;
            if (!Guid.TryParse(id, out guid))
            {
                return NotFound();
            }

            // Retrieve the item from storage
            var blob = await _storage.GetItem(guid, Constants.PdfsContainerName);
            if (blob == null)
            {
                return NotFound();
            }

            // Return the binary data to the client
            var bytes = await _storage.GetItemBinaryData(guid, Constants.PdfsContainerName);
            var result = new FileContentResult(bytes, "application/octet-stream");
            result.FileDownloadName = blob.Name;
            return result;
        }

        /// <summary>
        /// Add a pdf (max file size 5MB)
        /// </summary>
        /// <param name="uploadData">Pdf file to upload</param>
        /// <returns>The uploaded pdf item metadata</returns>
        [HttpPost]
        public async Task<ActionResult<StoredApiItem>> UploadPdf([FromForm]UploadPdfModel uploadData)
        {
            // Create a new unique file name and add the item to storage
            _logger.LogInformation($"Uploading {uploadData.File.FileName} to container {Constants.PdfsContainerName}");
            var item = await _storage.AddItem(uploadData.File, Constants.PdfsContainerName);
            return CreatedAtRoute("GetPdf", new { id = item.Location }, item);
        }
    }
}
