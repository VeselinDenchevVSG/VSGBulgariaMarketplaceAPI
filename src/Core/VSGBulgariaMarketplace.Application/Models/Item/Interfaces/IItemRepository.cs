namespace VSGBulgariaMarketplace.Application.Models.Item.Interfaces
{
    using VSGBulgariaMarketplace.Application.Models.Repositories;
    using VSGBulgariaMarketplace.Domain.Entities;

    public interface IItemRepository : IRepository<Item, int>
    {
        public Item[] GetMarketplace();

        public Item[] GetInventory();

        public Item GetByCode(int code);

        public Item GetQuantityForSaleAndPriceByCode(int code);

        public void Update(int id, Item item);

        public void BuyItem(int id, short quantity);
    }
}