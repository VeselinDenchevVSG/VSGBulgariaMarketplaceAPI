namespace VSGBulgariaMarketplace.Application.Models.Order.Dtos
{
    public class OrderDto
    {
        public int ItemId { get; set; }

        public short Quantity { get; set; }

        public string Email { get; set; } = null!;

        public string Status { get; set; }
    }
}
