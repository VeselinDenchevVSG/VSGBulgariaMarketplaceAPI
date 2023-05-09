namespace VSGBulgariaMarketplace.Domain.Entities;

using VSGBulgariaMarketplace.Domain.Enums;

public class Item : BaseEntity<int>
{
    public string Name { get; set; } = null!;

    public string? PicturePublicId { get; set; }

    public decimal Price { get; set; }

    public Category Category { get; set; }

    public short QuantityCombined { get; set; }

    public short? QuantityForSale { get; set; }

    public string? Description { get; set; }

    //public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}