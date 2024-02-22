namespace VSGBulgariaMarketplace.Persistence.Migrations
{
    using FluentMigrator;

    using static VSGBulgariaMarketplace.Persistence.Constants.DatabaseConstant;

    [Migration(UPDATE_TABLE_ORDERS_MIGRATION_VERSION)]
    public class UpdateTableOrders : Migration
    {
        public override void Up()
        {
            Delete.PrimaryKey(PRIMARY_KEY_ORDERS).FromTable(ORDERS_TABLE_NAME);
            Delete.Column(ID_COLUMN_NAME).FromTable(ORDERS_TABLE_NAME);
            Create.Column(ID_COLUMN_NAME).OnTable(ORDERS_TABLE_NAME).AsString(STRING_ID_COLUMN_SIZE);
            Create.PrimaryKey(PRIMARY_KEY_ORDERS).OnTable(ORDERS_TABLE_NAME).Column(ID_COLUMN_NAME);

            Delete.DefaultConstraint().OnTable(ORDERS_TABLE_NAME).OnColumn(DELETED_AT_UTC_COLUMN_NAME);
            Delete.Column(DELETED_AT_UTC_COLUMN_NAME).FromTable(ORDERS_TABLE_NAME);

            Delete.DefaultConstraint().OnTable(ORDERS_TABLE_NAME).OnColumn(IS_DELETED_COLUMN_NAME);
            Delete.Column(IS_DELETED_COLUMN_NAME).FromTable(ORDERS_TABLE_NAME);

            Create.Column(ITEM_CODE_COLUMN_NAME).OnTable(ORDERS_TABLE_NAME).AsInt32().NotNullable();

            Create.Column(ITEM_NAME_COLUMN_NAME).OnTable(ORDERS_TABLE_NAME).AsString(ITEM_NAME_COLUMN_SIZE).NotNullable();

            Create.Column(ITEM_PRICE_COLUMN_NAME).OnTable(ORDERS_TABLE_NAME).AsCustom(MONEY_DATA_TYPE).NotNullable();

            Alter.Table(ORDERS_TABLE_NAME).AlterColumn(ITEM_ID_COLUMN_NAME).AsString(STRING_ID_COLUMN_SIZE)
                                                                           .ForeignKey(ITEMS_TABLE_NAME, ID_COLUMN_NAME)
                                                                           .Nullable();
        }

        public override void Down()
        {
            Delete.PrimaryKey(PRIMARY_KEY_ORDERS).FromTable(ITEMS_TABLE_NAME);
            Alter.Table(ORDERS_TABLE_NAME).AlterColumn(ID_COLUMN_NAME).AsInt32().PrimaryKey().Identity();

            Create.Column(DELETED_AT_UTC_COLUMN_NAME).OnTable(ORDERS_TABLE_NAME).AsDateTime2().Nullable().WithDefaultValue(null);

            Create.Column(IS_DELETED_COLUMN_NAME).OnTable(ORDERS_TABLE_NAME).AsBoolean().NotNullable().WithDefaultValue(false);

            Delete.Column(ITEM_CODE_COLUMN_NAME).FromTable(ORDERS_TABLE_NAME);

            Delete.Column(ITEM_NAME_COLUMN_NAME).FromTable(ORDERS_TABLE_NAME);

            Delete.ForeignKey(ITEM_ID_COLUMN_NAME).OnTable(ORDERS_TABLE_NAME);
            Alter.Table(ORDERS_TABLE_NAME).AlterColumn(ITEM_ID_COLUMN_NAME).AsInt32().ForeignKey(ITEMS_TABLE_NAME, ID_COLUMN_NAME).Nullable();
        }
    }
}