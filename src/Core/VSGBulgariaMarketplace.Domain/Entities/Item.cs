namespace VSGBulgariaMarketplace.Domain.Entities;

using VSGBulgariaMarketplace.Domain.Enums;

public class Item : BaseEntity
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? PicturePublicId { get; set; }

    public decimal Price { get; set; }

    public Category Category { get; set; }

    public short QuantityCombined { get; set; }

    public short? QuantityForSale { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime ModifiedAtUtc { get; set; }

    public DateTime? DeletedAtUtc { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}