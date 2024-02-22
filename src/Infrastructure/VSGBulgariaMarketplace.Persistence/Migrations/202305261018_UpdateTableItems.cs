namespace VSGBulgariaMarketplace.Persistence.Migrations
{
    using FluentMigrator;

    using static VSGBulgariaMarketplace.Persistence.Constants.DatabaseConstant;

    [Migration(UPDATE_TABLE_ITEMS_MIGRATION_VERSION)]
    public class UpdateTableItems : Migration
    {
        public override void Up()
        {
            Delete.ForeignKey(FOREIGN_KEY_ORDERS_ITEM_ID_ITEMS_ID).OnTable(ORDERS_TABLE_NAME);

            Delete.PrimaryKey(PRIMARY_KEY_ITEMS).FromTable(ITEMS_TABLE_NAME);
            Alter.Table(ITEMS_TABLE_NAME).AlterColumn(ID_COLUMN_NAME).AsString(STRING_ID_COLUMN_SIZE);
            Create.PrimaryKey(PRIMARY_KEY_ITEMS).OnTable(ITEMS_TABLE_NAME).Column(ID_COLUMN_NAME);

            Delete.DefaultConstraint().OnTable(ITEMS_TABLE_NAME).OnColumn(DELETED_AT_UTC_COLUMN_NAME);
            Delete.Column(DELETED_AT_UTC_COLUMN_NAME).FromTable(ITEMS_TABLE_NAME);

            Delete.DefaultConstraint().OnTable(ITEMS_TABLE_NAME).OnColumn(IS_DELETED_COLUMN_NAME);
            Delete.Column(IS_DELETED_COLUMN_NAME).FromTable(ITEMS_TABLE_NAME);

            Alter.Table(ITEMS_TABLE_NAME).AlterColumn(NAME_COLUMN_NAME).AsString(150).Unique().NotNullable();

            Create.Column(CODE_COLUMN_NAME).OnTable(ITEMS_TABLE_NAME).AsInt32().Unique().NotNullable();
        }

        public override void Down()
        {
            Create.ForeignKey(FOREIGN_KEY_ORDERS_ITEM_ID_ITEMS_ID).FromTable(ORDERS_TABLE_NAME)
                  .ForeignColumn(ITEM_ID_COLUMN_NAME).ToTable(ITEMS_TABLE_NAME)
                  .PrimaryColumn(ID_COLUMN_NAME);

            Delete.PrimaryKey(PRIMARY_KEY_ITEMS).FromTable(ITEMS_TABLE_NAME);
            Alter.Table(ITEMS_TABLE_NAME).AlterColumn(ID_COLUMN_NAME).AsInt32();
            Create.PrimaryKey(PRIMARY_KEY_ITEMS).OnTable(ITEMS_TABLE_NAME).Column(ID_COLUMN_NAME);

            Create.Column(DELETED_AT_UTC_COLUMN_NAME).OnTable(ITEMS_TABLE_NAME).AsDateTime2().Nullable().WithDefaultValue(null);

            Create.Column(IS_DELETED_COLUMN_NAME).OnTable(ITEMS_TABLE_NAME).AsBoolean().NotNullable().WithDefaultValue(false);

            Delete.UniqueConstraint().FromTable(ITEMS_TABLE_NAME).Column(NAME_COLUMN_NAME);
            Alter.Table(ITEMS_TABLE_NAME).AlterColumn(NAME_COLUMN_NAME)
                                                                    .AsCustom(string.Format(NVARCHAR_DATA_TYPE_TEMPLATE, 
                                                                            ITEM_NAME_COLUMN_SIZE)).NotNullable();

            Delete.Column(CODE_COLUMN_NAME).FromTable(ITEMS_TABLE_NAME);
        }
    }
}