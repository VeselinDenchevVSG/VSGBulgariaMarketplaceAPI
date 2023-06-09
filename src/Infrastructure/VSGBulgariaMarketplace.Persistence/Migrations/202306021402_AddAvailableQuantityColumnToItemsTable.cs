namespace VSGBulgariaMarketplace.Persistence.Migrations
{
    using FluentMigrator;

    [Migration(202306021402)]
    public class AddAvailableQuantityColumnToItemsTable : Migration
    {
        private const string ITEMS_TABLE_NAME = "Items";
        private const string AVAILABLE_QUANTITY_COLUMN_NAME = "AvailableQuantity";
        private const string AVAILABLE_QUANTITY_CHECK_CONSTRAINT_NAME = "CK_Item_AvailableQuantity";

        public override void Up()
        {
            Alter.Table(ITEMS_TABLE_NAME).AddColumn(AVAILABLE_QUANTITY_COLUMN_NAME).AsInt16().Nullable();

            Execute.Sql($"ALTER TABLE {ITEMS_TABLE_NAME} ADD CONSTRAINT {AVAILABLE_QUANTITY_CHECK_CONSTRAINT_NAME} CHECK ({AVAILABLE_QUANTITY_COLUMN_NAME} >= 0)");
        }

        public override void Down()
        {
            Execute.Sql($"ALTER TABLE {ITEMS_TABLE_NAME} DROP CONSTRAINT {AVAILABLE_QUANTITY_CHECK_CONSTRAINT_NAME}");

            Delete.Column(AVAILABLE_QUANTITY_COLUMN_NAME).FromTable(ITEMS_TABLE_NAME);
        }
    }
}