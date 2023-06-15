namespace VSGBulgariaMarketplace.Persistence.Migrations
{
    using FluentMigrator;

    using VSGBulgariaMarketplace.Persistence.Constants;

    [Migration(DatabaseConstant.ADD_LOCATION_COLUMN_TO_ITEMS_TABLE_MIGRATION_VERSION)]
    public class AddLocationColumnToItemsTable : Migration
    {
        public override void Up()
        {
            Create.Column(DatabaseConstant.LOCATION_COLUMN_NAME).OnTable(DatabaseConstant.ITEMS_TABLE_NAME).AsInt32().NotNullable();
        }

        public override void Down()
        {
            Delete.Column(DatabaseConstant.LOCATION_COLUMN_NAME).FromTable(DatabaseConstant.ITEMS_TABLE_NAME);
        }
    }
}
