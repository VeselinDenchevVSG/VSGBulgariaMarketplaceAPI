namespace VSGBulgariaMarketplace.Persistence.Migrations
{
    using FluentMigrator;

    [Migration(202306021402)]
    public class AddAvailableQuantityColumnToItemsTable : Migration
    {
        private const string ITEMS_TABLE_NAME = "Items";
        private const string AVAILABLE_QUANTITY_COLUMN_NAME = "AvailableQuantity";

        public override void Up()
        {
            Alter.Table(ITEMS_TABLE_NAME).AddColumn(AVAILABLE_QUANTITY_COLUMN_NAME).AsInt16().Nullable();

            Execute.Sql($"ALTER TABLE {ITEMS_TABLE_NAME} ADD CONSTRAINT CK_Item_AvailableQuantity CHECK (AvailableQuantity >= 0)");
        }

        public override void Down()
        {
            Execute.Sql($"ALTER TABLE Persons DROP CONSTRAINT CK_Item_AvailableQuantity");

            Delete.Column(AVAILABLE_QUANTITY_COLUMN_NAME).FromTable(ITEMS_TABLE_NAME);
        }
    }
}