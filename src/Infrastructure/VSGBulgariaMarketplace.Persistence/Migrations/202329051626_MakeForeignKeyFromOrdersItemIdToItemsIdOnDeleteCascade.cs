namespace VSGBulgariaMarketplace.Persistence.Migrations
{
    using FluentMigrator;
    using System.Data;

    [Migration(202329051626)]
    public class MakeForeignKeyFromOrdersItemIdToItemsIdOnDeleteSetNull : Migration
    {
        private const string ORDERS_TABLE_NAME = "Orders";
        private const string ITEMS_TABLE_NAME = "Items";

        private const string ID_COLUMN_NAME = "Id";
        private const string ITEM_ID_COLUMN_NAME = "ItemId";

        private const string FOREIGN_KEY_ORDERS_ITEM_ID_ITEMS_ID_NAME = "FK_Orders_ItemId_Items_Id";

        public override void Up()
        {
            Delete.ForeignKey(FOREIGN_KEY_ORDERS_ITEM_ID_ITEMS_ID_NAME).OnTable(ORDERS_TABLE_NAME);

            Create.ForeignKey().FromTable(ORDERS_TABLE_NAME).ForeignColumn(ITEM_ID_COLUMN_NAME)
                    .ToTable(ITEMS_TABLE_NAME).PrimaryColumn(ID_COLUMN_NAME)
                    .OnDelete(Rule.SetNull);
        }

        public override void Down()
        {
            Delete.ForeignKey(FOREIGN_KEY_ORDERS_ITEM_ID_ITEMS_ID_NAME).OnTable(ITEMS_TABLE_NAME);

            Create.ForeignKey(FOREIGN_KEY_ORDERS_ITEM_ID_ITEMS_ID_NAME).FromTable(ORDERS_TABLE_NAME).ForeignColumn(ITEM_ID_COLUMN_NAME)
                                                                            .ToTable(ITEMS_TABLE_NAME).PrimaryColumn(ID_COLUMN_NAME);
        }
    }
}