﻿namespace VSGBulgariaMarketplace.Domain.Entities;

using VSGBulgariaMarketplace.Domain.Enums;

public class Item : BaseEntity<string>
{
    public string Code { get; set; }

    public string Name { get; set; } = null!;

    public string? ImagePublicId { get; set; }

    public decimal? Price { get; set; }

    public Category Category { get; set; }

    public short QuantityCombined { get; set; }

    public short? QuantityForSale { get; set; }

    public short? AvailableQuantity { get; set; }

    public string? Description { get; set; }

    public Location Location { get; set; }
}