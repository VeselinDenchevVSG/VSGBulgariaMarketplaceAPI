namespace VSGBulgariaMarketplace.Application.Models.ItemLoan.Interfaces
{
    using VSGBulgariaMarketplace.Application.Models.Repositories;
    using VSGBulgariaMarketplace.Domain.Entities;

    public interface IItemLoanRepository : IRepository<ItemLoan, string>
    {
        Dictionary<string, int> GetUserEmailWithLendItemsCount();

        ItemLoan[] GetUserLendItems(string email);

        ItemLoan GetItemLoanItemIdQuantityAndEmail(string id);

        Task<bool> IsLoanWithItem(string itemId, CancellationToken cancellationToken);

        Task<short> GetItemLoansTotalQuantityForItem(string itemId, CancellationToken cancellationToken);

        void Return(string id);
    }
}
