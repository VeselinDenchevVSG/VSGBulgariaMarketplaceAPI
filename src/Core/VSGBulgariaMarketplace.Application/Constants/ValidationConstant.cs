namespace VSGBulgariaMarketplace.Application.Constants
{
    internal static class ValidationConstant
    {
        #region VALIDATION BOUNDARIES
        internal const int ITEM_CODE_MAX_STRING_LENGTH = 50;
        internal const int ITEM_NAME_MAX_STRING_LENGTH = 120;
        internal const int ITEM_DESCRIPTION_MAX_STRING_LENGTH = 1_000;
        internal const short LEND_ITEMS_MIN_QUANTITY = 1;
        internal const short LEND_ITEMS_MAX_QUANTITY = short.MaxValue;
        internal const decimal ITEM_PRICE_MIN_VALUE = 0m;
        internal const decimal ITEM_PRICE_MAX_VALUE = 100_000;
        internal const int ITEM_PRICE_PRECISION = 28;
        internal const int ITEM_PRICE_SCALE = 2;
        internal const short ITEM_QUANTITY_MIN_VALUE = 0;
        internal const short ITEM_QUANTITY_MAX_VALUE = short.MaxValue;
        internal const short ORDER_QUANTITY_MIN_VALUE = 1;
        internal const short ORDER_QUANTITY_MAX_VALUE = short.MaxValue;
        #endregion

        #region ITEM PROPERTIES
        internal const string ITEM_CODE = "code";
        internal const string ITEM_NAME = "name";
        internal const string ITEM_PRICE = "price";
        internal const string ITEM_QUANTITY_COMBINED = "quantity combined";
        internal const string ITEM_QUANTITY_FOR_SALE = "quantity for sale";
        internal const string ITEM_AVAILABLE_QUANTITY = "available quantity";
        internal const string ITEM_DESCRIPTION = "description";
        internal const string ITEM_LENT_QUANTITY = "lent quantity";
        #endregion

        #region FILE CONSTANTS
        internal const int BYTES_IN_MB = 1024 * 1024;
        internal const int MAX_FILE_SIZE_IN_MB = 5;
        internal const string JPG_FILE_EXTENSION = ".jpg";
        internal const string JPEG_FILE_EXTENSION = ".jpeg";
        internal const string PNG_FILE_EXTENSION = ".png";
        internal const string GIF_FILE_EXTENSION = ".gif";
        internal const string SVG_FILE_EXTENSION = ".svg";
        internal const string WEBP_FILE_EXTENSION = ".webp";
        internal const string APNG_FILE_EXTENSION = ".apng";
        internal const string AVIF_FILE_EXTENSION = ".avif";
        #endregion

        #region ERROR MESSAGES
        internal const string NOT_EMPTY_ERROR_MESSAGE = "{PropertyName.ToLower()} can't be empty!";
        internal const string ITEM_PROPERTY_CAN_NOT_BE_LONGER_THAN_ERROR_MESSAGE_TEMPLATE = "Item {0} can't be longer than {1} characters!";
        internal const string INVALID_EMAIL_FORMAT_ERROR_MESSAGE = "Invalid email format!";
        internal const string ITEM_PROPERTY_MUST_BE_BETWEEN_MIN_AND_MAX_VALUE_ERROR_MESSAGE_TEMPLATE = "Item {0} must be between {1} and {2}!";
        internal const string ITEM_PRECISION_SCALE_ERROR_MESSAGE = "Item price must have no more than two digits after the decimal point and mustn't be longer than 28 digits in " +
                                                            "total!";
        internal const string INVALID_ITEM_CATEGORY_ERROR_MESSAGE = $"Item category must be in the specified ones!";
        internal const string ITEM_QUANTITY_MUST_BE_LESS_THAN_OR_EQUAL_TO_QUANTITY_COMBINED_ERROR_MESSAGE_TEMPLATE = "Item {0} must be less than or equal to quantity combined!";
        internal const string ITEM_QUANTITY_COMBINED_MUST_BE_GREATER_THAN_OR_EQUAL_TO_THE_SUM_OF_QUANTITY_FOR_SALE_AND_AVAILABLE_QUANTITY_ERROR_MESSAGE =
                                                                    "Item quantity combined must be greater than or equal to the sum of quantity for sale and available quantity!";
        internal const string ITEM_QUANTITY_COMBINED_MUST_BE_GREATER_THAN_OR_EQUAL_TO_QUANTITY_ERROR_MESSAGE_TEMPLATE =
                                                                                                        "Item quantity combined must be greater than or equal to {0}!";
        internal const string ITEM_LOCATION_MUST_BE_IN_THE_SPECIFIED_ONES_ERROR_MESSAAGE = "Item location must be in the specified ones!";

        internal const string FILE_CAN_NOT_BE_EMPTY_ERROR_MESSAGE = "File cannot be empty";
        internal const string FILE_MUST_BE_LESS_THAN_MAX_FILE_SIZE_ERROR_MESSAGE = "File size must be less than {0} MB";
        internal const string FILE_MUST_HAVE_ALLOWED_EXTENSION_ERROR_MESSAGE = "File must have one of the following extensions: {0}";

        internal const string ORDER_QUANTITY_MUST_BE_BETWEEN_MIN_AND_MAX_VALUE_ERROR_MESSAGE = "Order quantity must be between {0} and {1}!";
        #endregion

        internal const string VSG_EMAIL_REGEX_PATTERN = "[\\w]+@vsgbg\\.com$";
    }
}
