﻿namespace VSGBulgariaMarketplace.Domain.Entities
{
    public class CloudinaryImage : BaseEntity<string>
    {
        public string FileExtension { get; set; } = null!;

        public int Version { get; set; }
    }
}