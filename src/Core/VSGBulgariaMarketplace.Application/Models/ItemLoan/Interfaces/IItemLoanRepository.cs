namespace VSGBulgariaMarketplace.Application.Models.ItemLoan.Interfaces
{
    using VSGBulgariaMarketplace.Application.Models.Repositories;
    using VSGBulgariaMarketplace.Domain.Entities;

    public interface IItemLoanRepository : IRepository<ItemLoan, string>
    {
        Dictionary<string, int> GetUserEmailWithLendItemsCount();

        ItemLoan[] GetUserLendItems(string email);

        ItemLoan GetItemLoanItemIdAndQuantity(string id);

        bool IsLoanWithItem(string itemId);

        short GetItemLoansTotalQuantityForItem(string itemId);

        void Return(string id);
    }
}
