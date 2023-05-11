namespace VSGBulgariaMarketplace.Application.Models.Order.Dtos
{
    public class CreateOrderDto
    {
        public int ItemCode { get; set; }

        public short Quantity { get; set; }
    }
}