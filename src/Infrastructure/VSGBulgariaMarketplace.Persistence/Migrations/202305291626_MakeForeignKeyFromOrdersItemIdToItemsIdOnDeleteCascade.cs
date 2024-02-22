namespace VSGBulgariaMarketplace.Persistence.Migrations
{
    using FluentMigrator;
    using System.Data;

    using static VSGBulgariaMarketplace.Persistence.Constants.DatabaseConstant;

    [Migration(MAKE_FOREIGN_KEY_FROM_ORDERS_ITEM_ID_TO_ITEMS_ID_ON_DELETE_SET_NULL_MIGRATION_VERSION)]
    public class MakeForeignKeyFromOrdersItemIdToItemsIdOnDeleteSetNull : Migration
    {
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
                                                                       .ToTable(ORDERS_TABLE_NAME).PrimaryColumn(ID_COLUMN_NAME);
        }
    }
}