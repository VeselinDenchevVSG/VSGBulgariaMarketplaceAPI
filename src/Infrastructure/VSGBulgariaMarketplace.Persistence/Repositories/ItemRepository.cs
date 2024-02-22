namespace VSGBulgariaMarketplace.Persistence.Repositories
{
    using Dapper;

    using Microsoft.Data.SqlClient;

    using VSGBulgariaMarketplace.Application.Models.Exceptions;
    using VSGBulgariaMarketplace.Application.Models.Item.Interfaces;
    using VSGBulgariaMarketplace.Application.Models.UnitOfWork;
    using VSGBulgariaMarketplace.Domain.Entities;

    using static VSGBulgariaMarketplace.Persistence.Constants.RepositoryConstant;

    public class ItemRepository : Repository<Item, string>, IItemRepository
    {
        public ItemRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
            base.columnNamesString = ITEM_REPOSITORY_COLUMN_NAMES_STRING;
            base.SetUpRepository();
        }

        public Item[] GetMarketplace()
        {
            string sql = GET_MARKETPLACE_SQL_QUERY;
            Item[] marketplace = base.DbConnection.Query<Item, CloudinaryImage, Item>(sql, (item, image) =>
            {
                item.ImagePublicId = image.Id;

                return item;
            }, splitOn: CLOUDINARY_IMAGE_ID_ALIAS, transaction: base.Transaction).ToArray();

            foreach (Item item in marketplace)
            {
                if (item.ImagePublicId is not null)
                {
                    item.ImagePublicId = item.ImagePublicId.Insert(0, CLOUDINARY_IMAGE_DIRECTORY);
                }
            }

            return marketplace;
        }

        public Item[] GetInventory()
        {
            string sql = GET_INVENTORY_SQL_QUERY;
            Item[] inventory = base.DbConnection.Query<Item>(sql, transaction: this.Transaction).ToArray();

            return inventory;
        }

        public Item GetById(string id)
        {
            string sql = GET_ITEM_BY_ID_SQL_QUERY;
            Item item = base.DbConnection.Query<Item, CloudinaryImage, Item>(sql, (item, image) =>
            {
                item.ImagePublicId = image.Id;

                return item;
            }, new { Id = id }, splitOn: CLOUDINARY_IMAGE_ID_ALIAS, transaction: base.Transaction).FirstOrDefault();

            if (item?.ImagePublicId is not null)
            {
                item.ImagePublicId = item.ImagePublicId.Insert(0, CLOUDINARY_IMAGE_DIRECTORY);
            }

            return item;
        }

        public Item GetOrderItemInfoById(string id)
        {
            string sql = GET_ORDER_ITEM_INFO_BY_ID_SQL_QUERY;
            Item item = base.DbConnection.QueryFirstOrDefault<Item>(sql, new { Id = id }, transaction: base.Transaction);

            return item;
        }

        public async Task<string> GetItemImagePublicIdAsync(string id, CancellationToken cancellationToken)
        {
            string sql = GET_ITEM_PICTURE_PUBLIC_ID_SQL_QUERY;
            string itemImagePublicId = await this.DbConnection.QueryFirstOrDefaultAsync<string>(new CommandDefinition(sql, new { Id = id }, base.Transaction,
                                                                                                                            cancellationToken: cancellationToken));
            if (itemImagePublicId is not null)
            {
                itemImagePublicId = CLOUDINARY_IMAGE_DIRECTORY + itemImagePublicId;
            }

            return itemImagePublicId;
        }

        public bool TryGetAvailableQuantity(string id, out int? avaiableQuantity)
        {
            string sql = TRY_GET_AVAILABLE_QUANTITY_SQL_QUERY;
            avaiableQuantity = base.DbConnection.ExecuteScalar<int?>(sql, new { Id = id }, transaction: base.Transaction);

            bool exists = avaiableQuantity != null;

            return exists;
        }

        public void RequestItemPurchase(string id, short quantityRequested)
        {
            string sql = REQUEST_ITEM_PURCHASE_SQL_QUERY;
            base.DbConnection.Execute(sql, new { Id = id, QuantityRequested = quantityRequested }, transaction: base.Transaction);
        }

        public void RequestItemLoan(string id, short quantityRequested)
        {
            string sql = REQUEST_ITEM_LOAN_SQL_QUERY;
            base.DbConnection.Execute(sql, new { Id = id, QuantityRequested = quantityRequested }, transaction: base.Transaction);
        }

        public void RestoreItemQuantitiesWhenOrderIsDeclined(string id, short quantity)
        {
            string sql = RESTORE_ITEM_QUANTITES_WHEN_ORDER_IS_DECLINED_SQL_QUERY;
            base.DbConnection.Execute(sql, new { Id = id, Quantity = quantity }, transaction: base.Transaction);
        }

        public void RestoreItemQuantitiesWhenReturningLendItems(string id, short quantity)
        {
            string sql = RESTORE_ITEM_QUANTITES_WHEN_RETURNING_LEND_ITEMS_SQL_QUERY;
            base.DbConnection.Execute(sql, new { Id = id, Quantity = quantity }, transaction: base.Transaction);
        }

        public void BuyItem(string id, short quantityRequested)
        {
            string sql = BUY_ITEM_SQL_QUERY;
            base.DbConnection.Execute(sql, new { Id = id, QuantitySold = quantityRequested }, transaction: base.Transaction);
        }

        public void Update(string id, Item item)
        {
            string sql = string.Format(UPDATE_ITEM_SQL_QUERY, this.parameterizedColumnsNamesUpdateString);

            try
            {
                base.DbConnection.Execute(sql, new
                {
                    Id = id,
                    Code = item.Code,
                    Name = item.Name,
                    ImagePublicId = item.ImagePublicId,
                    Price = item.Price,
                    Category = item.Category,
                    QuantityCombined = item.QuantityCombined,
                    QuantityForSale = item.QuantityForSale,
                    AvailableQuantity = item.AvailableQuantity,
                    Description = item.Description,
                    Location = item.Location,
                    ModifiedAtUtc = DateTime.UtcNow
                }, transaction: this.Transaction);
            }
            catch (SqlException se) when (se.Number == 2601)
            {
                ThrowEntityAlreadyExistsException(SIMILAR_ITEM_ALREADY_EXISTS_ERROR_MESSAGE);
            }
        }

        public async override Task CreateAsync(Item item, CancellationToken cancellationToken)
        {
            item.Id = Guid.NewGuid().ToString();

            try
            {
                await base.CreateAsync(item, cancellationToken);
            }
            catch (EntityAlreadyExistsException)
            {
                throw new EntityAlreadyExistsException(SIMILAR_ITEM_ALREADY_EXISTS_ERROR_MESSAGE);
            }

        }

        private static void ThrowEntityAlreadyExistsException(string message) 
        {
            string[] exceptionMessageTokens = message.Split(EXCEPTION_MESSAGE_SEPERATORS.ToCharArray());
            string duplicateColumn = exceptionMessageTokens[ENTITY_ALREADY_EXISTS_EXCEPTION_MESSAGE_TOKEN_DUPLICATE_COLUMN_INDEX];
            string duplicateValue = exceptionMessageTokens[ENTITY_ALREADY_EXISTS_EXCEPTION_MESSAGE_TOKEN_DUPLICATE_VALUE_INDEX];

            throw new EntityAlreadyExistsException(string.Format(ITEM_WITH_THE_SAME_PROPERTY_ALREADY_EXISTS_ERROR_MESSAGE, duplicateColumn, duplicateValue));
        }
    }
}