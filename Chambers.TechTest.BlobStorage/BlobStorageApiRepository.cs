using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Chambers.TechTest.Common.Interfaces;
using Chambers.TechTest.Common.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Chambers.TechTest.BlobStorage
{
    /// <summary>
    /// Api repository for working with Azure blob storage
    /// </summary>
    public class BlobStorageApiRepository : IApiRepository
    {
        private BlobServiceClient _service;

        /// <summary>
        /// Initialises a new BlobStorageApiRepository
        /// </summary>
        /// <param name="connectionString">The connection string to use when connecting to azure blob storage</param>
        /// <returns>A new BlobStorageApiRepository object</returns>
        public static BlobStorageApiRepository Init(string connectionString)
        {
            var repository = new BlobStorageApiRepository();
            repository.Connect(connectionString);
            return repository;
        }

        /// <summary>
        /// Prepares the BlobServiceClient
        /// </summary>
        /// <param name="connectionString">The connection string to use when connecting to azure blob storage</param>
        public void Connect(string connectionString)
        {
            _service = new BlobServiceClient(connectionString);
        }

        /// <summary>
        /// Retrieves all items stored in a container
        /// </summary>
        /// <param name="containerName">The name of the blob container to inspect</param>
        /// <returns>A list of storage items</returns>
        public async Task<IEnumerable<StoredItem>> GetAllItems(string containerName)
        {
            var container = await GetContainer(containerName);
            var results = new List<StoredItem>();
            await foreach (var blob in container.GetBlobsAsync())
            {
                results.Add(new StoredItem
                {
                    Name = blob.Metadata[Constants.FileNameMetadataItemName],
                    Location = blob.Name,
                    FileSize = blob.Properties.ContentLength
                });
            }
            return results;
        }

        /// <summary>
        /// Retrieves an item stored in a container
        /// </summary>
        /// <param name="id">The id of the item to retrieve</param>
        /// <param name="containerName">The name of the blob container where the item is located</param>
        /// <returns>A storage item representing the requested item</returns>
        public async Task<StoredItem> GetItem(Guid id, string containerName)
        {
            var container = _service.GetBlobContainerClient(containerName);
            var blobClient = container.GetBlobClient(id.ToString());            
            if (!(await blobClient.ExistsAsync()))
            {
                return null;
            }

            var props = (await blobClient.GetPropertiesAsync()).Value;
            return new StoredItem
            {
                Name = props.Metadata[Constants.FileNameMetadataItemName],
                Location = blobClient.Name,
                FileSize = props.ContentLength
            };
        }

        /// <summary>
        /// Retrieves binary data of an item stored in a container
        /// </summary>
        /// <param name="id">The id of the item to retrieve</param>
        /// <param name="containerName">The name of the blob container where the item is located</param>
        /// <returns>A byte array containing the binary data of the requested item</returns>
        public async Task<byte[]> GetItemBinaryData(Guid id, string containerName)
        {
            var container = _service.GetBlobContainerClient(containerName);
            var blobClient = container.GetBlobClient(id.ToString());
            if (!(await blobClient.ExistsAsync()))
            {
                return null;
            }

            BlobDownloadInfo download = await blobClient.DownloadAsync();
            byte[] bytes;
            using (var memoryStream = new MemoryStream())
            {
                await download.Content.CopyToAsync(memoryStream);
                bytes = memoryStream.ToArray();
            }
            return bytes;
        }

        /// <summary>
        /// Adds a new item to the container
        /// </summary>
        /// <param name="file">File object that will be added</param>
        /// <returns>A storage item representing the added item</returns>
        public async Task<StoredItem> AddItem(IFormFile file, string containerName)
        {
            var container = await GetContainer(containerName);
            var blobClient = container.GetBlobClient(Guid.NewGuid().ToString());

            // Configure the content type and store the original file name as metadata
            BlobUploadOptions options = new BlobUploadOptions();
            options.HttpHeaders = new BlobHttpHeaders
            {
                ContentType = file.ContentType
            };
            options.Metadata = new Dictionary<string, string>()
            {
                { Constants.FileNameMetadataItemName, file.FileName }
            };

            // Upload the file
            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, options);
            }

            // Return uploaded file info as storage item
            return new StoredItem
            {
                Name = file.FileName,
                Location = blobClient.Name,
                FileSize = file.Length
            };
        }

        /// <summary>
        /// Retrieves a blob container from Azure blob storage, creating it if it does not already exist
        /// </summary>
        /// <param name="name">The name of the blob container to retrieve</param>
        /// <returns>The requested blob container</returns>
        protected async Task<BlobContainerClient> GetContainer(string name)
        {
            var container = _service.GetBlobContainerClient(name);
            await container.CreateIfNotExistsAsync();
            return container;
        }

        public async Task DeleteContainer(string name)
        {
            var container = _service.GetBlobContainerClient(name);
            if (await container.ExistsAsync())
            {
                await container.DeleteAsync();
            }
        }
    }
}
