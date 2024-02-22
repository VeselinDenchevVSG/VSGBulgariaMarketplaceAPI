namespace VSGBulgariaMarketplace.Persistence.Migrations
{
    using FluentMigrator;

    using static VSGBulgariaMarketplace.Persistence.Constants.DatabaseConstant;

    [Migration(ADD_LOCATION_COLUMN_TO_ITEMS_TABLE_MIGRATION_VERSION)]
    public class AddLocationColumnToItemsTable : Migration
    {
        public override void Up()
        {
            Create.Column(LOCATION_COLUMN_NAME).OnTable(ITEMS_TABLE_NAME).AsInt32().NotNullable();
        }

        public override void Down()
        {
            Delete.Column(LOCATION_COLUMN_NAME).FromTable(ITEMS_TABLE_NAME);
        }
    }
}
