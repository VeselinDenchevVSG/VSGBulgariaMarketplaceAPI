namespace VSGBulgariaMarketplace.Persistence.Migrations
{
    using FluentMigrator;

    using VSGBulgariaMarketplace.Persistence.Constants;

    [Migration(DatabaseConstant.CHANGE_CODE_COLUMN_TYPE_IN_ITEMS_TABLE_TO_STRING_MIGRATION_VERSION)]
    public class ChangeCodeColumnTypeInItemsTableToString : Migration
    {
        public override void Up()
        {
            Delete.UniqueConstraint().FromTable(DatabaseConstant.ITEMS_TABLE_NAME)
                                        .Columns(DatabaseConstant.CODE_COLUMN_NAME, DatabaseConstant.LOCATION_COLUMN_NAME);
            Delete.UniqueConstraint().FromTable(DatabaseConstant.ITEMS_TABLE_NAME)
                                        .Columns(DatabaseConstant.NAME_COLUMN_NAME, DatabaseConstant.CODE_COLUMN_NAME, DatabaseConstant.LOCATION_COLUMN_NAME);

            Alter.Table(DatabaseConstant.ITEMS_TABLE_NAME).AlterColumn(DatabaseConstant.CODE_COLUMN_NAME).AsString(DatabaseConstant.ITEM_CODE_COLUMN_SIZE)
                                                                                                                            .NotNullable();

            Create.UniqueConstraint().OnTable(DatabaseConstant.ITEMS_TABLE_NAME)
                                        .Columns(DatabaseConstant.CODE_COLUMN_NAME, DatabaseConstant.LOCATION_COLUMN_NAME);
            Create.UniqueConstraint().OnTable(DatabaseConstant.ITEMS_TABLE_NAME)
                                        .Columns(DatabaseConstant.NAME_COLUMN_NAME, DatabaseConstant.CODE_COLUMN_NAME, DatabaseConstant.LOCATION_COLUMN_NAME);

            Alter.Table(DatabaseConstant.ORDERS_TABLE_NAME).AlterColumn(DatabaseConstant.ITEM_CODE_COLUMN_NAME).AsString(DatabaseConstant.ITEM_CODE_COLUMN_SIZE)
                                                                                                                                .NotNullable();
        }

        public override void Down()
        {
            Alter.Table(DatabaseConstant.ORDERS_TABLE_NAME).AlterColumn(DatabaseConstant.ITEM_CODE_COLUMN_NAME).AsInt32().NotNullable();

            Delete.UniqueConstraint().FromTable(DatabaseConstant.ITEMS_TABLE_NAME)
                                        .Columns(DatabaseConstant.CODE_COLUMN_NAME, DatabaseConstant.LOCATION_COLUMN_NAME);
            Delete.UniqueConstraint().FromTable(DatabaseConstant.ITEMS_TABLE_NAME)
                                        .Columns(DatabaseConstant.NAME_COLUMN_NAME, DatabaseConstant.CODE_COLUMN_NAME, DatabaseConstant.LOCATION_COLUMN_NAME);

            Alter.Table(DatabaseConstant.ITEMS_TABLE_NAME).AlterColumn(DatabaseConstant.CODE_COLUMN_NAME).AsInt32().Unique().NotNullable();

            Create.UniqueConstraint().OnTable(DatabaseConstant.ITEMS_TABLE_NAME)
                                        .Columns(DatabaseConstant.CODE_COLUMN_NAME, DatabaseConstant.LOCATION_COLUMN_NAME);
            Create.UniqueConstraint().OnTable(DatabaseConstant.ITEMS_TABLE_NAME)
                                        .Columns(DatabaseConstant.NAME_COLUMN_NAME, DatabaseConstant.CODE_COLUMN_NAME, DatabaseConstant.LOCATION_COLUMN_NAME);
        }
    }
}