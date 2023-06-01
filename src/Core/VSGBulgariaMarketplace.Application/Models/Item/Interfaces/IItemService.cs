namespace VSGBulgariaMarketplace.Application.Models.Item.Interfaces
{
    using VSGBulgariaMarketplace.Application.Models.Item.Dtos;

    public interface IItemService
    {
        public MarketplaceItemDto[] GetMarketplace();

        public InventoryItemDto[] GetInventory();

        public ItemDetailsDto GetById(string id);

        public Task CreateAsync(CreateItemDto createItemDto);

        public Task UpdateAsync(string id, UpdateItemDto updateItemDto);

        public Task Delete(string id);
    }
}
