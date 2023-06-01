namespace VSGBulgariaMarketplace.Application.Models.Item.Interfaces
{
    using VSGBulgariaMarketplace.Application.Models.Repositories;
    using VSGBulgariaMarketplace.Domain.Entities;

    public interface IItemRepository : IRepository<Item, string>
    {
        public Item[] GetMarketplace();

        public Item[] GetInventory();

        public Item GetById(string id);

        public Item GetOrderItemInfoById(string id);

        public void Update(string id, Item item);

        public void Delete(string id);

        public void RequestItemPurchase(string id, short quantityRequested);

        public void RestoreItemQuantities(string id, short quantity);

        public void BuyItem(string id, short quantitySold);

        public string GetItemPicturePublicId(string id);
    }
}