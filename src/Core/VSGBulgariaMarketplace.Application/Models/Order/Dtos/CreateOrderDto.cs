namespace VSGBulgariaMarketplace.Application.Models.Order.Dtos
{
    public class CreateOrderDto
    {
        public int ItemCode { get; set; }

        public int Quantity { get; set; }
    }
}