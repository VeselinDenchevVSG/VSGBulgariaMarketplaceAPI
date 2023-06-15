namespace VSGBulgariaMarketplace.Application.Constants
{
    internal static class ServiceConstant
    {
        #region CLOUDINARY
        internal const string CLOUDINARY_CONFIGURATION_CLOUD = "Cloudinary:Cloud";
        internal const string CLOUDINARY_CONFIGURATION_API_KEY = "Cloudinary:ApiKey";
        internal const string CLOUDINARY_CONFIGURATION_API_SECRET = "Cloudinary:ApiSecret";
        internal const string CLOUDINARY_VSG_MARKETPLACE_IMAGES_FOLDER_NAME = "VSG_Marketplace";
        internal const string CLOUDINARY_IMAGE_URL_TEMPLATE = "v{0}/{1}/{2}.{3}";
        internal const string CLOUDINARY_DELETION_RESULT_NOT_FOUND = "not found";
        internal const string SLASH_URL_ENCODING = "%2F";
        #endregion

        #region CACHE KEYS
        internal const string USER_EMAILS_WITH_LEND_ITEMS_COUNT_CACHE_KEY = "user-emails-with-lend-items-count";
        internal const string USER_LEND_ITEMS_CACHE_KEY_TEMPLATE = "user-lend-items-{0}";
        internal const string MARKETPLACE_CACHE_KEY = "marketplace";
        internal const string INVENTORY_CACHE_KEY = "inventory";
        internal const string ITEM_CACHE_KEY_TEMPLATE = "item-{0}";
        internal const string PENDING_ORDERS_CACHE_KEY = "pending-orders";
        internal const string USER_ORDER_CACHE_KEY_TEMPLATE = "orders-user-{0}";
        #endregion

        #region ERROR MESSAGES
        internal const string NO_ENUM_WITH_DISPLAY_NAME_FOUND_ERROR_MESSAGE = "No {0} with display name {1} found";
        internal const string FAILED_TO_UPLOAD_FILE_ERROR_MESSAGE_TEMPLATE = "Failed to upload file: {0}";
        internal const string FAILED_TO_UPDATE_FILE_ERROR_MESSAGE_TEMPLATE = "Failed to update file: {0}";
        internal const string IMAGE_NOT_FOUND_ERROR_MESSAGE = "Image not found!";
        internal const string NOT_ENOUGH_AVAILABLE_QUANTITY_FOR_LENDING_IN_STOCK_ERROR_MESSAGE = "Not enough available quantity for lending in stock!";
        internal const string SUCH_ITEM_DOES_NOT_EXIST_ERROR_MESSAGE = "Such item doesn't exist!";
        internal const string SUCH_ITEM_LOAN_DOES_NOT_EXISTS_ERROR_MESSAGE = "Such item loan doesn't exist!";
        internal const string NOT_ENOUGH_QUANTITY_FOR_SALE_IN_ORDER_TO_COMPLETE_PENDING_ORDERS_WITH_THIS_ITEM_ERROR_MESSAGE =
                                                                                                "Not enough quantity for sale in order to complete pending orders with this item!";
        internal const string QUANTITY_COMBINED_MUST_NOT_BE_LOWER_THAN_THE_ACTIVE_LOANS_ITEM_QUANTITY_ERROR_MESSAGE =
                                                                                                            "Quantity combined mustn't be lower than the active loans item quantity!";
        internal const string CAN_NOT_DELETE_ITEM_BECAUSE_IT_IS_LENT_TO_SOMEONE_ERROR_MESSAGE = "Can't delete item because it is lent to someone!";
        internal const string NOT_ENOUGH_ITEM_QUANTITY_FOR_SALE_ERROR_MESSAGE = "Not enough item quantity for sale!";
        internal const string SUCH_ORDER_DOES_NOT_EXISTS_ERROR_MESSAGE = "Such order doesn't exist!";
        #endregion
    }
}