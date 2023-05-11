namespace VSGBulgariaMarketplace.Application.Models.Order.Dtos
{
    public class PendingOrderDto
    {
        public int ItemCode { get; set; }

        public short Quantity { get; set; }

        public decimal Price { get; set; }

        public DateTime OrderDate { get; set; }
    }
}