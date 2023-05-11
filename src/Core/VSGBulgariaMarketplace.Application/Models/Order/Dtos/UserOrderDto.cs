namespace VSGBulgariaMarketplace.Application.Models.Order.Dtos
{
    public class UserOrderDto
    {
        public string ItemName { get; set; }

        public short Quantity { get; set; }

        public decimal Price { get; set; }

        public DateTime OrderDate { get; set; }

        public string Status { get; set; }
    }
}
