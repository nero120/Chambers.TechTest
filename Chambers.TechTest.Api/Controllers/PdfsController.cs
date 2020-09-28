using Chambers.TechTest.Api.Models;
using Chambers.TechTest.BlobStorage;
using Chambers.TechTest.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
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
        readonly int FILE_SIZE_LIMIT = 1024 * 1024 * 5;
        readonly ILogger _logger;
        readonly IApiStorageClient _storage;

        public PdfsController(ILogger<PdfsController> logger, IApiStorageClient storage)
        {
            _logger = logger;
            _storage = storage;
        }

        /// <summary>
        /// List all existing pdfs
        /// </summary>
        /// <returns>A list of pdf item metadata</returns>
        [HttpGet]
        public async Task<IActionResult> GetPdfs()
        {
            var blobs = await _storage.GetAllItems(Constants.PdfsContainerName);
            return Ok(blobs);
        }

        /// <summary>
        /// Retrieve a specific pdf
        /// </summary>
        /// <param name="id">The location guid of the pdf to retrieve</param>
        /// <returns>The requested pdf item metadata</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPdf(string id)
        {
            var notFoundError = new NotFoundApiErrorResponse { Message = "The requested item does not exist" };

            // If the id is not valid return 404
            Guid guid;
            if (!Guid.TryParse(id, out guid))
            {
                return NotFound(notFoundError);
            }

            // Retrieve the item from storage
            var blob = await _storage.GetItem(guid, Constants.PdfsContainerName);
            if (blob == null)
            {
                return NotFound(notFoundError);
            }

            return Ok(blob);
        }

        /// <summary>
        /// Downloads a specific pdf
        /// </summary>
        /// <param name="id">The location guid of the pdf to retrieve</param>
        /// <returns>The pdf binary data as an octet stream</returns>
        [HttpGet("{id}/download")]
        public async Task<IActionResult> DownloadPdf(string id)
        {
            var notFoundError = new NotFoundApiErrorResponse { Message = "The requested item does not exist" };

            // If the id is not valid return 404
            Guid guid;
            if (!Guid.TryParse(id, out guid))
            {
                return NotFound(notFoundError);
            }

            // Retrieve the item from storage
            var blob = await _storage.GetItem(guid, Constants.PdfsContainerName);
            if (blob == null)
            {
                return NotFound(notFoundError);
            }

            // Return the binary data to the client
            var bytes = await _storage.GetItemBinaryData(guid, Constants.PdfsContainerName);
            var result = new FileContentResult(bytes, "application/octet-stream");
            result.FileDownloadName = blob.Name;
            return result;
        }

        /// <summary>
        /// Add a pdf
        /// </summary>
        /// <param name="file">Pdf file to upload</param>
        /// <returns>The uploaded pdf item metadata</returns>
        [HttpPost]
        public async Task<IActionResult> UploadPdf(IFormFile file)
        {
            // Validate file
            if (!FileIsPdf(file))
            {
                return BadRequest(new InvalidFileTypeApiErrorResponse { Message = "File must be a PDF" });
            }
            if (FileSizeLimitExceeded(file))
            {
                return BadRequest(new FileSizeLimitExceededApiErrorResponse { Message = "File size must be less than 5MB" });
            }

            // Create a new unique file name and add the item to storage
            _logger.LogInformation($"Uploading {file.FileName} to container {Constants.PdfsContainerName}");
            var item = await _storage.AddItem(file, Constants.PdfsContainerName);
            return Created(item.Location, item);
        }

        protected bool FileIsPdf(IFormFile file)
        {
            return file.ContentType.Equals("application/pdf") && Path.GetExtension(file.FileName).ToUpper().Equals(".PDF");
        }

        protected bool FileSizeLimitExceeded(IFormFile file)
        {
            return file.Length > FILE_SIZE_LIMIT;
        }
    }
}
