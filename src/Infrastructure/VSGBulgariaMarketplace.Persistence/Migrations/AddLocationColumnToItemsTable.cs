namespace VSGBulgariaMarketplace.Persistence.Migrations
{
    using FluentMigrator;

    [Migration(2023)]
    public class AddLocationColumnToItemsTable : Migration
    {
        private const string ITEMS_TABLE_NAME = "Items";
        private const string LOCATION_COLUMN_NAME = "Location";

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
