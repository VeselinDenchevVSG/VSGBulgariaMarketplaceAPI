namespace VSGBulgariaMarketplace.Application.Models.Item.Interfaces
{
    using Microsoft.AspNetCore.Http;

    using VSGBulgariaMarketplace.Application.Models.Item.Dtos;

    public interface IItemService
    {
        public MarketplaceItemDto[] GetMarketplace();

        public InventoryItemDto[] GetInventory();

        public ItemDetailsDto GetByCode(int id);

        public Task CreateAsync(ManageItemDto createItemDto, IFormFile imageFile);

        public Task UpdateAsync(int code, ManageItemDto updateItemDto, IFormFile imageFile);

        public void Delete(int id);
    }
}
