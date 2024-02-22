namespace VSGBulgariaMarketplace.Persistence.Migrations
{
    using FluentMigrator;

    using static VSGBulgariaMarketplace.Persistence.Constants.DatabaseConstant;

    [Migration(CHANGE_CODE_COLUMN_TYPE_IN_ITEMS_TABLE_TO_STRING_MIGRATION_VERSION)]
    public class ChangeCodeColumnTypeInItemsTableToString : Migration
    {
        public override void Up()
        {
            Delete.UniqueConstraint().FromTable(ITEMS_TABLE_NAME).Columns(CODE_COLUMN_NAME, LOCATION_COLUMN_NAME);
            Delete.UniqueConstraint().FromTable(ITEMS_TABLE_NAME).Columns(NAME_COLUMN_NAME, CODE_COLUMN_NAME, LOCATION_COLUMN_NAME);

            Alter.Table(ITEMS_TABLE_NAME).AlterColumn(CODE_COLUMN_NAME).AsString(ITEM_CODE_COLUMN_SIZE).NotNullable();

            Create.UniqueConstraint().OnTable(ITEMS_TABLE_NAME).Columns(CODE_COLUMN_NAME, LOCATION_COLUMN_NAME);
            Create.UniqueConstraint().OnTable(ITEMS_TABLE_NAME).Columns(NAME_COLUMN_NAME, CODE_COLUMN_NAME, LOCATION_COLUMN_NAME);

            Alter.Table(ORDERS_TABLE_NAME).AlterColumn(ITEM_CODE_COLUMN_NAME).AsString(ITEM_CODE_COLUMN_SIZE).NotNullable();
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