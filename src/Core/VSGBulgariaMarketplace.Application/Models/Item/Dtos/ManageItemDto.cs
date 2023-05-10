namespace VSGBulgariaMarketplace.Application.Models.Item.Dtos
{
    public class ManageItemDto
    {
        public int Code { get; set; }

        public string PicturePublicId { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public string Category { get; set; }

        public short QuantityCombined { get; set; }

        public short? QuantityForSale { get; set; }

        public string? Description { get; set; }
    }
}
