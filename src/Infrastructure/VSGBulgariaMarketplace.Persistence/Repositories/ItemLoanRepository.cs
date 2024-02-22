namespace VSGBulgariaMarketplace.Persistence.Repositories
{
    using Dapper;

    using VSGBulgariaMarketplace.Application.Models.ItemLoan.Interfaces;
    using VSGBulgariaMarketplace.Application.Models.UnitOfWork;
    using VSGBulgariaMarketplace.Domain.Entities;

    using static VSGBulgariaMarketplace.Persistence.Constants.RepositoryConstant;

    public class ItemLoanRepository : Repository<ItemLoan, string>, IItemLoanRepository
    {
        public ItemLoanRepository(IUnitOfWork unitOfWork) 
            : base(unitOfWork)
        {
            base.columnNamesString = ITEM_LOAN_REPOSITORY_COLUMN_NAMES_STRING;
            base.SetUpRepository();
        }

        public Dictionary<string, int> GetUserEmailWithLendItemsCount()
        {
            string sql = GET_USER_EMAILS_WITH_LEND_ITEMS_COUNT_SQL_QUERY;
            var emailsWithLendItemsCountRows = base.DbConnection.Query(sql, transaction: this.Transaction).ToArray();

            Dictionary<string, int> emailsWithLendItemsCount = new Dictionary<string, int>();

            foreach (var row in emailsWithLendItemsCountRows)
            {
                emailsWithLendItemsCount[row.Email] = row.LoansCount;
            }

            return emailsWithLendItemsCount;
        }

        public ItemLoan[] GetUserLendItems(string email)
        {
            string sql = GET_USER_LEND_ITEMS_SQL_QUERY;
            ItemLoan[] lendItemsByUser = base.DbConnection.Query<ItemLoan>(sql, new { Email = email }, transaction: base.Transaction).ToArray();

            return lendItemsByUser;
        }

        public ItemLoan GetItemLoanItemIdQuantityAndEmail(string id)
        {
            string sql = GET_ITEM_LOAN_ITEM_ID_AND_QUANTITY_SQL_QUERY;
            ItemLoan itemLoan = base.DbConnection.QueryFirstOrDefault<ItemLoan>(sql, new { Id = id }, transaction: base.Transaction);

            return itemLoan;
        }

        public async Task<bool> IsLoanWithItemAsync(string itemId, CancellationToken cancellationToken)
        {
            string sql = IS_LOAN_WITH_ITEM_SQL_QUERY;
            int? result = await base.DbConnection.QueryFirstOrDefaultAsync<int?>(new CommandDefinition(sql, new { ItemId = itemId }, 
                transaction: base.Transaction, cancellationToken: cancellationToken));

            return result.HasValue;
        }

        public async Task<short> GetItemLoansTotalQuantityForItemAsync(string itemId, CancellationToken cancellationToken)
        {
            string sql = GET_ITEM_LOANS_TOTAL_QUANTITY_FOR_ITEM_SQL_QUERY;
            short itemLoansTotalItemQuantity = await base.DbConnection.ExecuteScalarAsync<short>(new CommandDefinition(sql, new { ItemId = itemId }, 
                                                                                                    transaction: base.Transaction, cancellationToken: cancellationToken));

            return itemLoansTotalItemQuantity;
        }


        public void Return(string id)
        {
            string sql = RETURN_LEND_ITEM_SQL_QUERY;
            base.DbConnection.Execute(sql, new { Id = id }, transaction: base.Transaction);
        }

        public override void Create(ItemLoan itemLoan)
        {
            itemLoan.Id = Guid.NewGuid().ToString();

            base.Create(itemLoan);
        }
    }
}
