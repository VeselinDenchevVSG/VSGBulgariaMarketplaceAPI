namespace VSGBulgariaMarketplace.Persistence.Repositories
{
    using Dapper;

    using VSGBulgariaMarketplace.Application.Models.ItemLoan.Interfaces;
    using VSGBulgariaMarketplace.Application.Models.UnitOfWork;
    using VSGBulgariaMarketplace.Domain.Entities;

    public class ItemLoanRepository : Repository<ItemLoan, string>, IItemLoanRepository
    {
        public ItemLoanRepository(IUnitOfWork unitOfWork) 
            : base(unitOfWork)
        {
            base.columnNamesString = "(Id, ItemId, Email, Quantity, EndDatetimeUtc, CreatedAtUtc, ModifiedAtUtc)";
            base.SetUpRepository();
        }

        public Dictionary<string, int> GetUserEmailWithLendItemsCount()
        {
            string sql = "SELECT Email, COUNT(Id) AS LoansCount FROM ItemLoans " +
                         "WHERE EndDatetimeUtc IS NULL " +
                         "GROUP BY Email";
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
            string sql =    "SELECT Id, ItemId, Email, Quantity, CreatedAtUtc, EndDatetimeUtc FROM ItemLoans " +
                            "WHERE Email = @Email";
            ItemLoan[] lendItemsByUser = base.DbConnection.Query<ItemLoan>(sql, new { Email = email }, transaction: base.Transaction).ToArray();

            return lendItemsByUser;
        }

        public ItemLoan GetItemLoanItemIdAndQuantity(string id)
        {
            string sql = "SELECT ItemId, Quantity FROM ItemLoans WHERE Id = @Id";
            ItemLoan itemLoan = base.DbConnection.QueryFirstOrDefault<ItemLoan>(sql, new { Id = id }, transaction: base.Transaction);

            return itemLoan;
        }

        public bool IsLoanWithItem(string itemId)
        {
            string sql = "SELECT TOP 1 1 FROM ItemLoans WHERE ItemId = @ItemId";
            int? result = base.DbConnection.QueryFirstOrDefault<int?>(sql, new { ItemId = itemId }, transaction: base.Transaction);

            return result.HasValue;
        }

        public short GetItemLoansTotalItemQuantityByItemId(string itemId)
        {
            string sql = "SELECT SUM(Quantity) FROM ItemLoans WHERE ItemId = @ItemId AND EndDatetimeUtc IS NOT NULL";
            short itemLoansTotalItemQuantity = base.DbConnection.ExecuteScalar<short>(sql, new { ItemId = itemId }, transaction: base.Transaction);

            return itemLoansTotalItemQuantity;
        }


        public void Return(string id)
        {
            string sql = $"UPDATE ItemLoans SET EndDatetimeUtc = GETUTCDATE(), ModifiedAtUtc = GETUTCDATE() WHERE Id = @Id";
            base.DbConnection.Execute(sql, new { Id = id }, transaction: base.Transaction);
        }

        public override void Create(ItemLoan itemLoan)
        {
            itemLoan.Id = Guid.NewGuid().ToString();

            base.Create(itemLoan);
        }
    }
}
