namespace VSGBulgariaMarketplace.Persistence.Constants
{
    public static class DatabaseConstant
    {
        #region TABLE NAMES
        public const string LOGS_TABLE_NAME = "Logs";
        public const string CLOUDINARY_IMAGES_TABLE_NAME = "CloudinaryImages";
        public const string ITEMS_TABLE_NAME = "Items";
        public const string ORDERS_TABLE_NAME = "Orders";
        public const string ITEM_LOANS_TABLE_NAME = "ItemLoans";
        #endregion

        #region TABLE COLUMNS
        public const string ID_COLUMN_NAME = "Id";
        public const string CREATED_AT_UTC_COLUMN_NAME = "CreatedAtUtc";
        public const string MODIFIED_AT_UTC_COLUMN_NAME = "ModifiedAtUtc";
        public const string DELETED_AT_UTC_COLUMN_NAME = "DeletedAtUtc";
        public const string IS_DELETED_COLUMN_NAME = "IsDeleted";
        public const string LEVEL_COLUMN_NAME = "Level";
        public const string CALL_SITE_COLUMN_NAME = "CallSite";
        public const string TYPE_COLUMN_NAME = "Type";
        public const string MESSAGE_COLUMN_NAME = "Message";
        public const string STACK_TRACE_COLUMN_NAME = "StackTrace";
        public const string INNER_EXCEPTION_COLUMN_NAME = "InnerException";
        public const string ADDITIONAL_INFO_COLUMN_NAME = "AdditionalInfo";
        public const string LOGGED_ON_DATETIME_UTC_COLUMN_NAME = "LoggedOnDatetimeUtc";
        public const string SECURE_URL_COLUMN_NAME = "SecureUrl";
        public const string FILE_EXTENSION_COLUMN_NAME = "FileExtension";
        public const string VERSION_COLUMN_NAME = "Version";
        public const string NAME_COLUMN_NAME = "Name";
        public const string IMAGE_PUBLIC_ID_COLUMN_NAME = "ImagePublicId";
        public const string PRICE_COLUMN_NAME = "Price";
        public const string CATEGORY_COLUMN_NAME = "Category";
        public const string QUANTITY_COMBINED_COLUMN_NAME = "QuantityCombined";
        public const string QUANTITY_FOR_SALE_COLUMN_NAME = "QuantityForSale";
        public const string AVAILABLE_QUANTITY_COLUMN_NAME = "AvailableQuantity";
        public const string DESCRIPTION_COLUMN_NAME = "Description";
        public const string LOCATION_COLUMN_NAME = "Location";
        public const string ITEM_ID_COLUMN_NAME = "ItemId";
        public const string CODE_COLUMN_NAME = "Code";
        public const string QUANTITY_COLUMN_NAME = "Quantity";
        public const string EMAIL_COLUMN_NAME = "Email";
        public const string STATUS_COLUMN_NAME = "Status";
        public const string ITEM_CODE_COLUMN_NAME = "ItemCode";
        public const string ITEM_NAME_COLUMN_NAME = "ItemName";
        public const string ITEM_PRICE_COLUMN_NAME = "ItemPrice";
        public const string END_DATE_TIME_UTC_COLUMN_NAME = "EndDatetimeUtc";
        #endregion

        #region DATA TYPES
        public const string DATETIME2_DATA_TYPE_TEMPLATE = "DATETIME2({0})";
        public const string NVARCHAR_DATA_TYPE_TEMPLATE = "NVARCHAR({0})";
        public const string MONEY_DATA_TYPE = "MONEY";
        #endregion

        #region COLUMN SIZES
        public const byte LEVEL_COLUMN_SIZE = 11;
        public const byte LOGGED_ON_DATETIME_UTC_COLUMN_SIZE = 7;
        public const byte INT_ID_COLUMN_SIZE = 20;
        public const byte STRING_ID_COLUMN_SIZE = 36;
        public const byte SECURE_URL_COLUMN_SIZE = 150;
        public const byte FILE_EXTENSION_COLUMN_SIZE = 5;
        public const byte ITEM_NAME_COLUMN_SIZE = 150;
        public const short DESCRIPTION_COLUMN_SIZE = 1_000;
        public const byte EMAIL_COLUMN_SIZE = 30;
        public const byte ITEM_CODE_COLUMN_SIZE = 50;
        #endregion

        #region CHECK CONSTRAINTS
        internal const string ADD_CONSTRAINT_SQL_QUERY_TEMPLATE = "ALTER TABLE {0} ADD CONSTRAINT {1} CHECK {2}";
        internal const string DROP_CONSTRAINT_SQL_QUERY_TEMPLATE = "ALTER TABLE {0} DROP CONSTRAINT {1}";
        public const string CHECK_ITEM_QUANTITY_COMBINED_CONSTRAINT_NAME = "CK_Item_QuantityCombined";
        public const string CHECK_ITEM_QUANTITY_FOR_SALE_CONSTRAINT_NAME = "CK_Item_QuantityForSale";
        public const string CHECK_ORDER_QUANTITY_CONSTRAINT_NAME = "CK_Order_Quantity";
        public const string CHECK_AVAILABLE_QUANTITY_CONSTRAINT_NAME = "CK_Item_AvailableQuantity";
        #endregion

        #region PRIMARY KEYS
        public const string PRIMARY_KEY_ITEMS = "PK_Items";
        public const string PRIMARY_KEY_ORDERS = "PK_Orders";
        public const string PRIMARY_KEY_ITEM_LOANS= "PK_ItemLoans";
        #endregion

        #region FOREIGN KEYS
        public const string FOREIGN_KEY_ORDERS_ITEM_ID_ITEMS_ID = "FK_Orders_ItemId_Items_Id";
        public const string FOREIGN_KEY_ITEMS_IMAGE_PUBLIC_ID_CLOUDINARY_IMAGES_ID_NAME = "FK_Items_ImagePublicId_CloudinaryImages_Id";
        public const string FOREIGN_KEY_ORDERS_ITEM_ID_ITEMS_ID_NAME = "FK_Orders_ItemId_Items_Id";
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
        internal const long MAKE_PRICE_IN_ITEMS_TABLE_NULLABLE_MIGRATION_VERSION = 202306161526;
        #endregion
    }
}