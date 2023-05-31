namespace VSGBulgariaMarketplace.Application.Models.Item.Interfaces
{
    using VSGBulgariaMarketplace.Application.Models.Item.Dtos;

    public interface IItemService
    {
        public MarketplaceItemDto[] GetMarketplace();

        public InventoryItemDto[] GetInventory();

        public ItemDetailsDto GetByCode(int id);

        public Task CreateAsync(CreateItemDto createItemDto);

        public Task UpdateAsync(int code, UpdateItemDto updateItemDto);

        public Task Delete(int code);
    }
}
