namespace VSGBulgariaMarketplace.Domain.Entities;

using VSGBulgariaMarketplace.Domain.Enums;

public class Order : BaseEntity<int>
{
    public int Id { get; set; }

    public int ItemId { get; set; }

    public short Quantity { get; set; }

    public string Email { get; set; } = null!;

    public DateTime OrderDatetime { get; set; }

    public OrderStatus Status { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime ModifiedAtUtc { get; set; }

    public DateTime? DeletedAtUtc { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Item Item { get; set; } = null!;
}