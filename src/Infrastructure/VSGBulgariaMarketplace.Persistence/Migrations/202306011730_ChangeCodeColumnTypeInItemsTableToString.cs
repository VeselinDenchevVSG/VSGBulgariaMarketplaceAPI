namespace VSGBulgariaMarketplace.Persistence.Migrations
{
    using FluentMigrator;

    [Migration(202306011730)]
    public class ChangeCodeColumnTypeInItemsTableToString : Migration
    {
        private const string ITEMS_TABLE_NAME = "Items";
        private const string ORDERS_TABLE_NAME = "Orders";

        private const string NAME_COLUMN_NAME = "Name";
        private const string CODE_COLUMN_NAME = "Code";
        private const string LOCATION_COLUMN_NAME = "Location";

        private const string ITEM_CODE_COLUMN_NAME = "ItemCode";

        public override void Up()
        {
            Delete.UniqueConstraint().FromTable(ITEMS_TABLE_NAME).Columns(CODE_COLUMN_NAME, LOCATION_COLUMN_NAME);
            Delete.UniqueConstraint().FromTable(ITEMS_TABLE_NAME).Columns(NAME_COLUMN_NAME, CODE_COLUMN_NAME, LOCATION_COLUMN_NAME);

            Alter.Table(ITEMS_TABLE_NAME).AlterColumn(CODE_COLUMN_NAME).AsString(50).NotNullable();

            Create.UniqueConstraint().OnTable(ITEMS_TABLE_NAME).Columns(CODE_COLUMN_NAME, LOCATION_COLUMN_NAME);
            Create.UniqueConstraint().OnTable(ITEMS_TABLE_NAME).Columns(NAME_COLUMN_NAME, CODE_COLUMN_NAME, LOCATION_COLUMN_NAME);

            Alter.Table(ORDERS_TABLE_NAME).AlterColumn(ITEM_CODE_COLUMN_NAME).AsString(50).NotNullable();
        }

        public override void Down()
        {
            Alter.Table(ORDERS_TABLE_NAME).AlterColumn(ITEM_CODE_COLUMN_NAME).AsInt32().NotNullable();

            Delete.UniqueConstraint().FromTable(ITEMS_TABLE_NAME).Columns(CODE_COLUMN_NAME, LOCATION_COLUMN_NAME);
            Delete.UniqueConstraint().FromTable(ITEMS_TABLE_NAME).Columns(NAME_COLUMN_NAME, CODE_COLUMN_NAME, LOCATION_COLUMN_NAME);

            Alter.Table(ITEMS_TABLE_NAME).AlterColumn(CODE_COLUMN_NAME).AsInt32().Unique().NotNullable();

            Create.UniqueConstraint().OnTable(ITEMS_TABLE_NAME).Columns(CODE_COLUMN_NAME, LOCATION_COLUMN_NAME);
            Create.UniqueConstraint().OnTable(ITEMS_TABLE_NAME).Columns(NAME_COLUMN_NAME, CODE_COLUMN_NAME, LOCATION_COLUMN_NAME);
        }
    }
}