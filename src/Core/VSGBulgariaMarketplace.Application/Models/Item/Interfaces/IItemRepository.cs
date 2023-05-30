namespace VSGBulgariaMarketplace.Application.Models.Item.Interfaces
{
    using VSGBulgariaMarketplace.Application.Models.Repositories;
    using VSGBulgariaMarketplace.Domain.Entities;

    public interface IItemRepository : IRepository<Item, string>
    {
        public Item[] GetMarketplace();

        public Item[] GetInventory();

        public Item GetByCode(int code);

        public Item GetOrderItemInfoByCode(int code);

        public void Update(int id, Item item);

        public void DeleteByCode(int code);

        public void BuyItem(int code, short quantity);

        public string GetItemPicturePublicId(int code);
    }
}