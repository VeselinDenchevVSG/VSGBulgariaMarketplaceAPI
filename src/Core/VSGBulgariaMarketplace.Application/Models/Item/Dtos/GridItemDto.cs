namespace VSGBulgariaMarketplace.Application.Models.Item.Dtos
{
    public class GridItemDto
    {
        public int Code { get; set; }

        public string PicturePublicId { get; set; }

        public decimal Price { get; set; }

        public string Category { get; set; }

        public short? QuantityForSale { get; set; }
    }
}
