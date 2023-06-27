namespace VSGBulgariaMarketplace.Application.Models.Item.Interfaces
{
    using VSGBulgariaMarketplace.Application.Models.Repositories;
    using VSGBulgariaMarketplace.Domain.Entities;

    public interface IItemRepository : IRepository<Item, string>
    {
        Item[] GetMarketplace();

        Item[] GetInventory();

        Item GetById(string id);

        Item GetOrderItemInfoById(string id);

        Task<string> GetItemImagePublicIdAsync(string id, CancellationToken cancellationToken);

        bool TryGetAvailableQuantity(string id, out int? avaiableQuantity);

        void Update(string id, Item item);

        void RequestItemPurchase(string id, short quantityRequested);

        void RequestItemLoan(string id, short quantityRequested);

        void RestoreItemQuantitiesWhenOrderIsDeclined(string id, short quantity);

        void RestoreItemQuantitiesWhenReturningLendItems(string id, short quantity);

        void BuyItem(string id, short quantitySold);
    }
}