using Chambers.TechTest.Common.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chambers.TechTest.Common.Interfaces
{
    /// <summary>
    /// Describes a data store that can be used by the Api
    /// </summary>
    public interface IApiStorageClient
    {
        static IApiStorageClient Init(string connectionString) => throw new NotImplementedException();

        Task<IEnumerable<StorageItem>> GetAllItems(string containerName);

        Task<StorageItem> GetItem(Guid id, string containerName);

        Task<byte[]> GetItemBinaryData(Guid id, string containerName);

        Task<StorageItem> AddItem(IFormFile file, string containerName);
    }
}
