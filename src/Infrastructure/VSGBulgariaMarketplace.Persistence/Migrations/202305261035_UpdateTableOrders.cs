namespace VSGBulgariaMarketplace.Persistence.Migrations
{
    using FluentMigrator;

    using VSGBulgariaMarketplace.Persistence.Constants;

    [Migration(DatabaseConstant.UPDATE_TABLE_ORDERS_MIGRATION_VERSION)]
    public class UpdateTableOrders : Migration
    {
        public override void Up()
        {
            Delete.PrimaryKey(DatabaseConstant.PRIMARY_KEY_ORDERS).FromTable(DatabaseConstant.ORDERS_TABLE_NAME);
            Delete.Column(DatabaseConstant.ID_COLUMN_NAME).FromTable(DatabaseConstant.ORDERS_TABLE_NAME);
            Create.Column(DatabaseConstant.ID_COLUMN_NAME).OnTable(DatabaseConstant.ORDERS_TABLE_NAME).AsString(DatabaseConstant.STRING_ID_COLUMN_SIZE);
            Create.PrimaryKey(DatabaseConstant.PRIMARY_KEY_ORDERS).OnTable(DatabaseConstant.ORDERS_TABLE_NAME)
                                                                                    .Column(DatabaseConstant.ID_COLUMN_NAME);

            Delete.DefaultConstraint().OnTable(DatabaseConstant.ORDERS_TABLE_NAME).OnColumn(DatabaseConstant.DELETED_AT_UTC_COLUMN_NAME);
            Delete.Column(DatabaseConstant.DELETED_AT_UTC_COLUMN_NAME).FromTable(DatabaseConstant.ORDERS_TABLE_NAME);

            Delete.DefaultConstraint().OnTable(DatabaseConstant.ORDERS_TABLE_NAME).OnColumn(DatabaseConstant.IS_DELETED_COLUMN_NAME);
            Delete.Column(DatabaseConstant.IS_DELETED_COLUMN_NAME).FromTable(DatabaseConstant.ORDERS_TABLE_NAME);

            Create.Column(DatabaseConstant.ITEM_CODE_COLUMN_NAME).OnTable(DatabaseConstant.ORDERS_TABLE_NAME).AsInt32().NotNullable();

            Create.Column(DatabaseConstant.ITEM_NAME_COLUMN_NAME).OnTable(DatabaseConstant.ORDERS_TABLE_NAME).AsString(DatabaseConstant.ITEM_NAME_COLUMN_SIZE)
                    .NotNullable();

            Create.Column(DatabaseConstant.ITEM_PRICE_COLUMN_NAME).OnTable(DatabaseConstant.ORDERS_TABLE_NAME).AsCustom(DatabaseConstant.MONEY_DATA_TYPE)
                    .NotNullable();

            Alter.Table(DatabaseConstant.ORDERS_TABLE_NAME).AlterColumn(DatabaseConstant.ITEM_ID_COLUMN_NAME).AsString(DatabaseConstant.STRING_ID_COLUMN_SIZE)
                                                                        .ForeignKey(DatabaseConstant.ITEMS_TABLE_NAME, DatabaseConstant.ID_COLUMN_NAME)
                                                                        .Nullable();
        }

        public override void Down()
        {
            Delete.PrimaryKey(DatabaseConstant.PRIMARY_KEY_ORDERS).FromTable(DatabaseConstant.ITEMS_TABLE_NAME);
            Alter.Table(DatabaseConstant.ORDERS_TABLE_NAME).AlterColumn(DatabaseConstant.ID_COLUMN_NAME).AsInt32().PrimaryKey().Identity();

            Create.Column(DatabaseConstant.DELETED_AT_UTC_COLUMN_NAME).OnTable(DatabaseConstant.ORDERS_TABLE_NAME).AsDateTime2().Nullable()
                                                                                                                                    .WithDefaultValue(null);

            Create.Column(DatabaseConstant.IS_DELETED_COLUMN_NAME).OnTable(DatabaseConstant.ORDERS_TABLE_NAME).AsBoolean().NotNullable().WithDefaultValue(false);

            Delete.Column(DatabaseConstant.ITEM_CODE_COLUMN_NAME).FromTable(DatabaseConstant.ORDERS_TABLE_NAME);

            Delete.Column(DatabaseConstant.ITEM_NAME_COLUMN_NAME).FromTable(DatabaseConstant.ORDERS_TABLE_NAME);

            Delete.ForeignKey(DatabaseConstant.ITEM_ID_COLUMN_NAME).OnTable(DatabaseConstant.ORDERS_TABLE_NAME);
            Alter.Table(DatabaseConstant.ORDERS_TABLE_NAME).AlterColumn(DatabaseConstant.ITEM_ID_COLUMN_NAME).AsInt32()
                                                                        .ForeignKey(DatabaseConstant.ITEMS_TABLE_NAME, DatabaseConstant.ID_COLUMN_NAME)
                                                                        .Nullable();
        }
    }
}