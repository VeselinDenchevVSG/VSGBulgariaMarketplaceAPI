namespace VSGBulgariaMarketplace.Application.Models.Item.Dtos
{
    public class ItemDetailsDto
    {
        public string ImageUrl { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public string Category { get; set; }

        public short? QuantityForSale { get; set; }

        public string? Description { get; set; }
    }
}
