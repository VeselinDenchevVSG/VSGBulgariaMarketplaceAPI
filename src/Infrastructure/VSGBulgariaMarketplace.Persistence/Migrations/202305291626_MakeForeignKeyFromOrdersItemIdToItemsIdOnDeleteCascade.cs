namespace VSGBulgariaMarketplace.Persistence.Migrations
{
    using FluentMigrator;
    using System.Data;

    using VSGBulgariaMarketplace.Persistence.Constants;

    [Migration(DatabaseConstant.MAKE_FOREIGN_KEY_FROM_ORDERS_ITEM_ID_TO_ITEMS_ID_ON_DELETE_SET_NULL_MIGRATION_VERSION)]
    public class MakeForeignKeyFromOrdersItemIdToItemsIdOnDeleteSetNull : Migration
    {
        public override void Up()
        {
            Delete.ForeignKey(DatabaseConstant.FOREIGN_KEY_ORDERS_ITEM_ID_ITEMS_ID_NAME).OnTable(DatabaseConstant.ORDERS_TABLE_NAME);

            Create.ForeignKey().FromTable(DatabaseConstant.ORDERS_TABLE_NAME).ForeignColumn(DatabaseConstant.ITEM_ID_COLUMN_NAME)
                    .ToTable(DatabaseConstant.ITEMS_TABLE_NAME).PrimaryColumn(DatabaseConstant.ID_COLUMN_NAME)
                    .OnDelete(Rule.SetNull);
        }

        public override void Down()
        {
            Delete.ForeignKey(DatabaseConstant.FOREIGN_KEY_ORDERS_ITEM_ID_ITEMS_ID_NAME).OnTable(DatabaseConstant.ITEMS_TABLE_NAME);

            Create.ForeignKey(DatabaseConstant.FOREIGN_KEY_ORDERS_ITEM_ID_ITEMS_ID_NAME).FromTable(DatabaseConstant.ORDERS_TABLE_NAME)
                                                                            .ForeignColumn(DatabaseConstant.ITEM_ID_COLUMN_NAME)
                                                                            .ToTable(DatabaseConstant.ORDERS_TABLE_NAME).PrimaryColumn(DatabaseConstant.ID_COLUMN_NAME);
        }
    }
}