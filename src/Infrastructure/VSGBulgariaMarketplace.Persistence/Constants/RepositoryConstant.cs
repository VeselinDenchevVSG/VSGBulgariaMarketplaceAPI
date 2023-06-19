namespace VSGBulgariaMarketplace.Persistence.Constants
{
    internal static class RepositoryConstant
    {
        #region COLUMN NAMES STRINGS
        internal const string ITEM_REPOSITORY_COLUMN_NAMES_STRING = "(Id, Code, Name, ImagePublicId, Price, Category, QuantityCombined, QuantityForSale, AvailableQuantity, " +
                                                            "Description, Location, CreatedAtUtc, ModifiedAtUtc)";
        internal const string IMAGE_REPOSITORY_COLUMN_NAMES_STRING = "(Id, FileExtension, Version, CreatedAtUtc, ModifiedAtUtc)";
        internal const string ORDER_REPOSITORY_COLUMN_NAMES_STRING = "(Id, ItemId, ItemCode, ItemName, ItemPrice, Quantity, Email, Status, CreatedAtUtc, ModifiedAtUtc)";
        internal const string ITEM_LOAN_REPOSITORY_COLUMN_NAMES_STRING = "(Id, ItemId, Email, Quantity, EndDatetimeUtc, CreatedAtUtc, ModifiedAtUtc)";
        #endregion

        #region GENERIC REPOSITORY SQL QUERIES
        internal const string SQL_QUERY_PARAMETER_TEMPLATE = "@{0}, ";
        internal const string SQL_QUERY_COLUMN_PARAMETER_TEMPLATE = "{0} = @{0}, ";
        internal const string CREATE_ENTITY_SQL_QUERY = "INSERT INTO {0} {1} VALUES {2}";
        internal const string DELETE_ENTITY_SQL_QUERY = "DELETE FROM {0} WHERE Id = @Id";
        #endregion

        #region ITEM REPOSITORY SQL QUERIES
        internal const string GET_MARKETPLACE_SQL_QUERY = "SELECT i.Id, i.Code, i.Price, i.Category, i.QuantityForSale, i.ImagePublicId, ci.Id AS CloudinaryImageId " +
                                                            "FROM Items AS i " +
                                                            "LEFT JOIN CloudinaryImages AS ci " +
                                                            "ON i.ImagePublicId = ci.Id " +
                                                            "WHERE i.QuantityForSale IS NOT NULL AND i.QuantityForSale > 0";

        internal const string GET_INVENTORY_SQL_QUERY = "SELECT Id, Code, Name, Description, Category, QuantityCombined, QuantityForSale, AvailableQuantity, Price, ImagePublicId, " +
                                                        "Location FROM Items";
        internal const string GET_ITEM_BY_ID_SQL_QUERY = "SELECT i.Name, i.Price, i.Category, i.QuantityForSale, i.Description, i.ImagePublicId, ci.Id AS CloudinaryImageId " +
                                                            "FROM Items AS i " +
                                                            "LEFT JOIN CloudinaryImages AS ci " +
                                                            "ON i.ImagePublicId = ci.Id " +
                                                            "WHERE i.Id = @Id AND i.QuantityForSale IS NOT NULL";
        internal const string GET_ORDER_ITEM_INFO_BY_ID_SQL_QUERY = "SELECT Id, Code, Name, QuantityForSale, Price FROM Items WHERE Id = @Id";
        internal const string GET_ITEM_PICTURE_PUBLIC_ID_SQL_QUERY = "SELECT ImagePublicId FROM Items WHERE Id = @Id";
        internal const string TRY_GET_AVAILABLE_QUANTITY_SQL_QUERY = "SELECT ISNULL(AvailableQuantity, 0) FROM Items WHERE Id = @Id";
        internal const string REQUEST_ITEM_PURCHASE_SQL_QUERY = "UPDATE Items SET QuantityForSale -= @QuantityRequested, ModifiedAtUtc = GETUTCDATE() WHERE Id = @Id";
        internal const string REQUEST_ITEM_LOAN_SQL_QUERY = "UPDATE Items SET AvailableQuantity -= @QuantityRequested, ModifiedAtUtc = GETUTCDATE() WHERE Id = @Id";
        internal const string RESTORE_ITEM_QUANTITES_WHEN_ORDER_IS_DECLINED_SQL_QUERY = $"UPDATE Items SET QuantityForSale += @Quantity, QuantityCombined += @Quantity, " +
                                                                                        $"ModifiedAtUtc = GETUTCDATE() WHERE Id = @Id";
        internal const string RESTORE_ITEM_QUANTITES_WHEN_RETURNING_LEND_ITEMS_SQL_QUERY = $"UPDATE Items SET AvailableQuantity += @Quantity, ModifiedAtUtc = GETUTCDATE() " +
                                                                                            $"WHERE Id = @Id";
        internal const string BUY_ITEM_SQL_QUERY = "UPDATE Items SET QuantityCombined -= @QuantitySold, ModifiedAtUtc = GETUTCDATE() WHERE Id = @Id";
        internal const string UPDATE_ITEM_SQL_QUERY = "UPDATE Items SET {0} WHERE Id = @Id";
        #endregion

        #region IMAGE REPOSITORY SQL QUERIES
        internal const string GET_IMAGE_BUILD_URL_INFO_BY_ITEM_ID_SQL_QUERY = "SELECT ci.FileExtension, ci.Version, ci.Id, i.ImagePublicId AS CloudinaryImageId " +
                                                                                "FROM CloudinaryImages AS ci " +
                                                                                "JOIN Items AS i " +
                                                                                "ON ci.Id = i.ImagePublicId " +
                                                                                "WHERE i.Id = @ItemId";
        internal const string UPDATE_IMAGE_FILE_INFO_SQL_QUERY = "UPDATE CloudinaryImages SET FileExtension = @FileExtension, Version = @Version, ModifiedAtUtc = GETUTCDATE() " +
                                                                    "WHERE Id = @PublicId";
        #endregion

        #region ORDERS REPOSITORY SQL QUERIES
        internal const string GET_PENDING_ORDERS_SQL_QUERY = "SELECT Id, ItemCode, Quantity, ItemPrice, Email, CreatedAtUtc FROM Orders WHERE Status = 0";
        internal const string GET_USER_ORDERS_SQL_QUERY = "SELECT Id, ItemCode, ItemName, ItemPrice, Quantity, CreatedAtUtc, Status FROM Orders WHERE Email = @Email";
        internal const string GET_ORDER_ITEM_ID_AND_QUANTITY = "SELECT ItemId, Quantity FROM Orders WHERE Id = @Id";
        internal const string FINISH_ORDER_SQL_QUERY = "UPDATE Orders SET Status = 1, ModifiedAtUtc = GETUTCDATE() WHERE Id = @id";
        internal const string DECLINE_ALL_PENDING_ORDERS_WITH_DELETED_ITEM_SQL_QUERY = "SELECT o.Id, o.ItemId AS ItemId, i.Id FROM Orders AS o " +
                                                                                "JOIN Items AS i " +
                                                                                "ON o.ItemId = i.Id " +
                                                                                "WHERE o.Status = 0 AND i.Id = @ItemId";
        internal const string GET_PENDING_ORDERS_TOTAL_ITEM_QUANTITY_BY_ITEM_ID_SQL_QUERY = "SELECT SUM(Quantity) FROM Orders WHERE ItemId = @ItemId AND Status = 0";
        #endregion

        #region ITEM LOANS REPOSITORY SQL QUERIES
        internal const string GET_USER_EMAILS_WITH_LEND_ITEMS_COUNT_SQL_QUERY = "SELECT Email, COUNT(Id) AS LoansCount FROM ItemLoans GROUP BY Email";
        internal const string GET_USER_LEND_ITEMS_SQL_QUERY = "SELECT Id, ItemId, Email, Quantity, CreatedAtUtc, EndDatetimeUtc FROM ItemLoans WHERE Email = @Email";
        internal const string GET_ITEM_LOAN_ITEM_ID_AND_QUANTITY_SQL_QUERY = "SELECT ItemId, Quantity, Email FROM ItemLoans WHERE Id = @Id";
        internal const string IS_LOAN_WITH_ITEM_SQL_QUERY = "SELECT TOP 1 1 FROM ItemLoans WHERE ItemId = @ItemId";
        internal const string GET_ITEM_LOANS_TOTAL_QUANTITY_FOR_ITEM_SQL_QUERY = "SELECT SUM(Quantity) FROM ItemLoans WHERE ItemId = @ItemId AND EndDatetimeUtc IS NULL";
        internal const string RETURN_LEND_ITEM_SQL_QUERY = "UPDATE ItemLoans SET EndDatetimeUtc = GETUTCDATE(), ModifiedAtUtc = GETUTCDATE() WHERE Id = @Id";
        #endregion

        #region ALIASES
        internal const string CLOUDINARY_IMAGE_ID_ALIAS = "CloudinaryImageId";
        internal const string ITEM_ID_ALIAS = "ItemId";
        #endregion

        #region ERROR MESSAGES
        internal const string ENTITY_ALREADY_EXISTS_ERROR_MESSAGE = "{0} already exists!";
        internal const string ENTITY_DOES_NOT_EXIST_ERROR_MESSAGE = "{0} doesn't exist!";
        internal const string ITEM_DOES_NOT_EXIST_ERROR_MESSAGE = "Item doesn't exist!";
        internal const string SIMILAR_ITEM_ALREADY_EXISTS_ERROR_MESSAGE = "Item with the same name and location or code and location already exists!";
        internal const string ITEM_WITH_THE_SAME_PROPERTY_ALREADY_EXISTS_ERROR_MESSAGE = "Item with {0} {1} already exists!";
        #endregion

        #region EXCEPTION MESSAGES
        internal const int ENTITY_ALREADY_EXISTS_EXCEPTION_MESSAGE_TOKEN_DUPLICATE_COLUMN_INDEX = 17;
        internal const int ENTITY_ALREADY_EXISTS_EXCEPTION_MESSAGE_TOKEN_DUPLICATE_VALUE_INDEX = 26;
        internal const string EXCEPTION_MESSAGE_SEPERATORS = " _()'.";
        #endregion

        internal const string CLOUDINARY_IMAGE_DIRECTORY = "VSG_Marketplace/";
    }
}