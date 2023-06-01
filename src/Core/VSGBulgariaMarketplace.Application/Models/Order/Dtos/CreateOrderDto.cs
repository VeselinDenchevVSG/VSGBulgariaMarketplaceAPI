namespace VSGBulgariaMarketplace.Application.Models.Order.Dtos
{
    public class CreateOrderDto
    {
        public string ItemId { get; set; }

        public int Quantity { get; set; }
    }
}