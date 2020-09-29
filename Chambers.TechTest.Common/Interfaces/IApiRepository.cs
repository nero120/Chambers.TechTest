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
    public interface IApiRepository
    {
        static IApiRepository Init(string connectionString) => throw new NotImplementedException();

        Task<IEnumerable<StoredItem>> GetAllItems(string containerName);

        Task<StoredItem> GetItem(Guid id, string containerName);

        Task<byte[]> GetItemBinaryData(Guid id, string containerName);

        Task<StoredItem> AddItem(IFormFile file, string containerName);
    }
}
