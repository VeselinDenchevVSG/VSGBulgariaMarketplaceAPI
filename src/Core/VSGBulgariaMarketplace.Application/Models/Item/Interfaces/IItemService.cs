namespace VSGBulgariaMarketplace.Application.Models.Item.Interfaces
{
    using VSGBulgariaMarketplace.Application.Models.Item.Dtos;

    public interface IItemService
    {
        MarketplaceItemDto[] GetMarketplace();

        InventoryItemDto[] GetInventory();

        ItemDetailsDto GetById(string id);

        Task CreateAsync(CreateItemDto createItemDto, CancellationToken cancellationToken);

        Task UpdateAsync(string id, UpdateItemDto updateItemDto, CancellationToken cancellationToken);

        Task DeleteAsync(string id, CancellationToken cancellationToken);
    }
}
