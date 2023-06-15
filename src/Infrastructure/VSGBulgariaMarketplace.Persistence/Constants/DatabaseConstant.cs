namespace VSGBulgariaMarketplace.Persistence.Constants
{
    internal static class DatabaseConstant
    {
        #region TABLE NAMES
        internal const string LOGS_TABLE_NAME = "Logs";
        internal const string CLOUDINARY_IMAGES_TABLE_NAME = "CloudinaryImages";
        internal const string ITEMS_TABLE_NAME = "Items";
        internal const string ORDERS_TABLE_NAME = "Orders";
        internal const string ITEM_LOANS_TABLE_NAME = "ItemLoans";
        #endregion

        #region TABLE COLUMNS
        internal const string ID_COLUMN_NAME = "Id";
        internal const string CREATED_AT_UTC_COLUMN_NAME = "CreatedAtUtc";
        internal const string MODIFIED_AT_UTC_COLUMN_NAME = "ModifiedAtUtc";
        internal const string DELETED_AT_UTC_COLUMN_NAME = "DeletedAtUtc";
        internal const string IS_DELETED_COLUMN_NAME = "IsDeleted";
        internal const string LEVEL_COLUMN_NAME = "Level";
        internal const string CALL_SITE_COLUMN_NAME = "CallSite";
        internal const string TYPE_COLUMN_NAME = "Type";
        internal const string MESSAGE_COLUMN_NAME = "Message";
        internal const string STACK_TRACE_COLUMN_NAME = "StackTrace";
        internal const string INNER_EXCEPTION_COLUMN_NAME = "InnerException";
        internal const string ADDITIONAL_INFO_COLUMN_NAME = "AdditionalInfo";
        internal const string LOGGED_ON_DATETIME_UTC_COLUMN_NAME = "LoggedOnDatetimeUtc";
        internal const string SECURE_URL_COLUMN_NAME = "SecureUrl";
        internal const string FILE_EXTENSION_COLUMN_NAME = "FileExtension";
        internal const string VERSION_COLUMN_NAME = "Version";
        internal const string NAME_COLUMN_NAME = "Name";
        internal const string IMAGE_PUBLIC_ID_COLUMN_NAME = "ImagePublicId";
        internal const string PRICE_COLUMN_NAME = "Price";
        internal const string CATEGORY_COLUMN_NAME = "Category";
        internal const string QUANTITY_COMBINED_COLUMN_NAME = "QuantityCombined";
        internal const string QUANTITY_FOR_SALE_COLUMN_NAME = "QuantityForSale";
        internal const string AVAILABLE_QUANTITY_COLUMN_NAME = "AvailableQuantity";
        internal const string DESCRIPTION_COLUMN_NAME = "Description";
        internal const string LOCATION_COLUMN_NAME = "Location";
        internal const string ITEM_ID_COLUMN_NAME = "ItemId";
        internal const string CODE_COLUMN_NAME = "Code";
        internal const string QUANTITY_COLUMN_NAME = "Quantity";
        internal const string EMAIL_COLUMN_NAME = "Email";
        internal const string STATUS_COLUMN_NAME = "Status";
        internal const string ITEM_CODE_COLUMN_NAME = "ItemCode";
        internal const string ITEM_NAME_COLUMN_NAME = "ItemName";
        internal const string ITEM_PRICE_COLUMN_NAME = "ItemPrice";
        internal const string END_DATE_TIME_UTC_COLUMN_NAME = "EndDatetimeUtc";
        #endregion

        #region DATA TYPES
        internal const string DATETIME2_DATA_TYPE_TEMPLATE = "DATETIME2({0})";
        internal const string NVARCHAR_DATA_TYPE_TEMPLATE = "NVARCHAR({0})";
        internal const string MONEY_DATA_TYPE = "MONEY";
        #endregion

        #region COLUMN SIZES
        internal const byte LEVEL_COLUMN_SIZE = 11;
        internal const byte LOGGED_ON_DATETIME_UTC_COLUMN_SIZE = 7;
        internal const byte INT_ID_COLUMN_SIZE = 20;
        internal const byte STRING_ID_COLUMN_SIZE = 36;
        internal const byte SECURE_URL_COLUMN_SIZE = 150;
        internal const byte FILE_EXTENSION_COLUMN_SIZE = 5;
        internal const byte ITEM_NAME_COLUMN_SIZE = 150;
        internal const short DESCRIPTION_COLUMN_SIZE = 1_000;
        internal const byte EMAIL_COLUMN_SIZE = 30;
        internal const byte ITEM_CODE_COLUMN_SIZE = 50;
        #endregion

        #region CHECK CONSTRAINTS
        internal const string ADD_CONSTRAINT_SQL_QUERY_TEMPLATE = "ALTER TABLE {0} ADD CONSTRAINT {1} CHECK {2}"; // items
        internal const string DROP_CONSTRAINT_SQL_QUERY_TEMPLATE = "ALTER TABLE {0} DROP CONSTRAINT {1}";
        internal const string CHECK_ITEM_QUANTITY_COMBINED_CONSTRAINT_NAME = "CK_Item_QuantityCombined";
        internal const string CHECK_ITEM_QUANTITY_FOR_SALE_CONSTRAINT_NAME = "CK_Item_QuantityForSale";
        internal const string CHECK_ORDER_QUANTITY_CONSTRAINT_NAME = "CK_Order_Quantity";
        internal const string CHECK_AVAILABLE_QUANTITY_CONSTRAINT_NAME = "CK_Item_AvailableQuantity";
        #endregion

        #region PRIMARY KEYS
        internal const string PRIMARY_KEY_ITEMS = "PK_Items";
        internal const string PRIMARY_KEY_ORDERS = "PK_Orders";
        #endregion

        #region FOREIGN KEYS
        internal const string FOREIGN_KEY_ORDERS_ITEM_ID_ITEMS_ID = "FK_Orders_ItemId_Items_Id";
        internal const string FOREIGN_KEY_ITEMS_IMAGE_PUBLIC_ID_CLOUDINARY_IMAGES_ID_NAME = "FK_Items_ImagePublicId_CloudinaryImages_Id";
        internal const string FOREIGN_KEY_ORDERS_ITEM_ID_ITEMS_ID_NAME = "FK_Orders_ItemId_Items_Id";
        #endregion

        #region MIGRATION VERSIONS
        internal const long CREATE_TABLE_LOGS_MIGRATION_VERSION = 202305041729;
        internal const long CREATE_TABLE_CLOUDINARY_IMAGES_MIGRATION_VERSION = 202305171120;
        internal const long CREATE_TABLE_ITEMS_MIGRATION_VERSION = 202305171123;
        internal const long CREATE_TABLE_ORDERS_MIGRATION_VERSION = 202305171124;
        internal const long UPDATE_TABLE_CLOUDINARY_IMAGES_MIGRATION_VERSION = 202305261012;
        internal const long UPDATE_TABLE_ITEMS_MIGRATION_VERSION = 202305261018;
        internal const long UPDATE_TABLE_ORDERS_MIGRATION_VERSION = 202305261035;
        internal const long MAKE_FOREIGN_KEY_FROM_ITEMS_IMAGE_PUBLIC_ID_TO_CLOUDINARY_IMAGES_ID_ON_DELETE_SET_NULL_MIGRATION_VERSION = 202305291615;
        internal const long MAKE_FOREIGN_KEY_FROM_ORDERS_ITEM_ID_TO_ITEMS_ID_ON_DELETE_SET_NULL_MIGRATION_VERSION = 202305291626;
        internal const long ADD_VERSION_COLUMN_TO_CLOUDINARY_IMAGES_TABLE_MIGRATION_VERSION = 202305311556;
        internal const long ADD_LOCATION_COLUMN_TO_ITEMS_TABLE_MIGRATION_VERSION = 202306010846;
        internal const long CHANGE_UNIQUE_CONSTRAINTS_INT_ITEMS_TABLE_MIGRATION_VERSION = 202306011030;
        internal const long CHANGE_CODE_COLUMN_TYPE_IN_ITEMS_TABLE_TO_STRING_MIGRATION_VERSION = 202306011730;
        internal const long DELETE_UNIQUE_CONSTRAINT_CODE_NAME_LOCATION_IN_TABLE_ITEMS_MIGRATION_VERSION = 202306021001;
        internal const long ADD_AVAILABLE_QUANTITY_COLUMN_TO_ITEMS_TABLE_MIGRATION_VERSION = 202306021402;
        internal const long CREATE_TABLE_ITEM_LOANS_MIGRATION_VERSION = 202306021421;
        internal const long ADD_QUANTITY_COMBINED_GREATER_THAN_OR_EQUAL_TO_AVAILABLE_QUANTITY_FOR_LOAN_CONSTRAINT_IN_ITEM_TABLE_MIGRATION_VERSION = 202306091021;
        #endregion
    }
}