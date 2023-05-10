namespace VSGBulgariaMarketplace.Application.Models.Item.Dtos
{
    public class InventoryItemDto
    {
        public int Code { get; set; }

        public string Name { get; set; }

        public string Category { get; set; }

        public short? QuantityForSale { get; set; }

        public short QuantityCombined { get; set; }
    }
}
