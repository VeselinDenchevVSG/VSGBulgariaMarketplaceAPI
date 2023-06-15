namespace VSGBulgariaMarketplace.Application.Constants
{
    internal static class JsonConstant
    {
        #region ITEM DTOS
        internal const string INVENTORY_ITEM_QUANTITY_COMBINED_JSON_PROPERTY_NAME = "quantity";
        internal const string ITEM_IMAGE_URL_JSON_PROPERTY_NAME = "imageURL";
        #endregion

        #region ITEM LOAN DTOS
        internal const string EMAIL_WITH_LEND_ITEMS_COUNT_LEND_ITEMS_COUNT_JSON_PROPERTY_NAME = "count";
        internal const string LEND_ITEMS_EMAIL_JSON_PROPERTY_NAME = "orderedBy";
        internal const string USER_LEND_ITEMS_START_DATE_JSON_PROPERTY_NAME = "loanStartDate";
        internal const string USER_LEND_ITEMS_END_DATE_JSON_PROPERTY_NAME = "loanEndDate";
        #endregion

        #region ORDER DTOS
        internal const string PENDING_ORDER_PRICE_JSON_PROPERTY_NAME = "orderPrice";
        internal const string USER_ORDER_ITEM_NAME_JSON_PROPERTY_NAME = "name";
        #endregion
    }
}
