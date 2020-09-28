﻿namespace Chambers.TechTest.Common.Models
{
    /// <summary>
    /// An item contained in Api storage
    /// </summary>
    public class StorageItem
    {
        public string Name { get; set; }

        public string Location { get; set; }

        public long? FileSize { get; set; }
    }
}
