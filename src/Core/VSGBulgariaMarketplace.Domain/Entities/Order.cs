namespace VSGBulgariaMarketplace.Domain.Entities;

using VSGBulgariaMarketplace.Domain.Enums;

public class Order : BaseEntity<int>
{
    public int ItemId { get; set; }

    public short Quantity { get; set; }

    public string Email { get; set; } = null!;

    public OrderStatus Status { get; set; }

    public virtual Item Item { get; set; } = null!;
}