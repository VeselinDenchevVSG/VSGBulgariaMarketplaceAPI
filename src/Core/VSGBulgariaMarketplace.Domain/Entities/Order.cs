namespace VSGBulgariaMarketplace.Domain.Entities;

using VSGBulgariaMarketplace.Domain.Enums;

public class Order : BaseEntity<string>
{
    public string ItemId { get; set; } = null!;

    public int ItemCode { get; set; }

    public string ItemName { get; set; } = null!;

    public decimal ItemPrice { get; set; }

    public short Quantity { get; set; }

    public string Email { get; set; } = null!;

    public OrderStatus Status { get; set; }

    public decimal Price => this.ItemPrice * this.Quantity;
}