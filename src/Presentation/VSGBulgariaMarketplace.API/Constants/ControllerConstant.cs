namespace VSGBulgariaMarketplace.API.Constants
{
    internal static class ControllerConstant
    {
        #region ROUTES
        internal const string MARKETPLACE_ROUTE = "marketplace";
        internal const string INVENTORY_ROUTE = "inventory";
        internal const string GET_ITEM_BY_ID_ROUTE = "{id}";
        internal const string GET_CATEGORIES_ROUTE = "categories";
        internal const string CREATE_ITEM_ASYNC_ROUTE = "create";
        internal const string UPDATE_ITEM_ASYNC_ROUTE = "update/{code}";
        internal const string DELETE_ITEM_ASYNC_ROUTE = "delete/{code}";
        internal const string GET_USER_EMAILS_WITH_LEND_ITEMS_COUNT_ROUTE = "user-emails-with-lend-items-count";
        internal const string GET_USER_LENT_ITEMS_ROUTE = "user-lent-items/{email}";
        internal const string LEND_ITEMS_ROUTE = "lend-items/{itemId}";
        internal const string RETURN_LENT_ITEMS_ROUTE = "return-items/{id}";
        internal const string GET_LOCATIONS_ROUTE = "locations";
        internal const string GET_PENDING_ORDERS_ROUTE = "pending-orders";
        internal const string GET_USER_ORDERS_ROUTE = "user-orders";
        internal const string CREATE_ORDER_ROUTE = "create";
        internal const string FINISH_ORDER_ROUTE = "finish/{id}";
        internal const string DECLINE_ORDER_ROUTE = "decline/{id}";
        #endregion

        #region ROUTES SPARTAK
        internal const string GET_CATEGORIES_ROUTE_SPARTAK = "/getcategories";
        internal const string GET_ITEM_BY_ID_ROUTE_SPARTAK = "marketplace/{id}";
        internal const string CREATE_ITEM_ASYNC_ROUTE_SPARTAK = "~/inventory/addItem";
        internal const string UPDATE_ITEM_ASYNC_ROUTE_SPARTAK = "~/inventory/modify/{id}";
        internal const string DELETE_ITEM_ASYNC_ROUTE_SPARTAK = "/deleteItem/{id}";
        internal const string GET_USER_LENT_ITEMS_ROUTE_SPARTAK = "myloans/{email}";
        internal const string LEND_ITEMS_ROUTE_SPARTAK = "inventory/loan/{itemId}";
        internal const string RETURN_LENT_ITEMS_ROUTE_SPARTAK = "lentitems/return/{id}";
        internal const string GET_USER_EMAILS_WITH_LEND_ITEMS_COUNT_ROUTE_SPARTAK = "lentitems";
        internal const string GET_LOCATIONS_ROUTE_SPARTAK = "/getLocations";
        internal const string GET_PENDING_ORDERS_ROUTE_SPARTAK = "/pendingorders";
        internal const string GET_USER_ORDERS_ROUTE_SPARTAK = "/myorders";
        internal const string CREATE_ORDER_ROUTE_SPARTAK = "marketplace/buy";
        internal const string FINISH_ORDER_ROUTE_SPARTAK = "/pendingorders/complete/{code}";
        internal const string DECLINE_ORDER_ROUTE_SPARTAK = "myorders/deleteorder/{code}";
        #endregion

        #region SUCCESS MESSAGES
        internal const string ITEM_SUCCESSFULLY_CREATED_MESSAGE_TEMPLATE = "Item {0} has been successfully created!";
        internal const string ITEM_SUCCESSFULLY_UPDATED_MESSAGE_TEMPLATE = "Item {0} has been successfully updated!";
        internal const string ITEM_SUCCESSFULLY_DELETED_MESSAGE = "Item has been successfully deleted!";
        internal const string ITEMS_LENT_SUCCESSFULLY_MESSAGE = "Items were lent successfullly";
        internal const string ITEMS_RETURNED_SUCCESSFULLY_MESSAGE = "Items were returned successfully";
        internal const string ORDER_CREATED_SUCCESSFULLY_MESSAGE = "Order was created successfully!";
        internal const string ORDER_FINISHED_SUCCESSFULLY_MESSAGE = "Order was finished successfully!";
        internal const string ORDER_DECLINED_SUCCESSFULLY_MESSAGE = "Order was declined successfully!";
        #endregion
    }
}