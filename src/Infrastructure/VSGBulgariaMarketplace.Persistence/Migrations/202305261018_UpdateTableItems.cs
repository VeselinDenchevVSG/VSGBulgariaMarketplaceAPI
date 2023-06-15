namespace VSGBulgariaMarketplace.Persistence.Migrations
{
    using FluentMigrator;

    using VSGBulgariaMarketplace.Persistence.Constants;

    [Migration(DatabaseConstant.UPDATE_TABLE_ITEMS_MIGRATION_VERSION)]
    public class UpdateTableItems : Migration
    {
        public override void Up()
        {
            Delete.ForeignKey(DatabaseConstant.FOREIGN_KEY_ORDERS_ITEM_ID_ITEMS_ID).OnTable(DatabaseConstant.ORDERS_TABLE_NAME);

            Delete.PrimaryKey(DatabaseConstant.PRIMARY_KEY_ITEMS).FromTable(DatabaseConstant.ITEMS_TABLE_NAME);
            Alter.Table(DatabaseConstant.ITEMS_TABLE_NAME).AlterColumn(DatabaseConstant.ID_COLUMN_NAME).AsString(DatabaseConstant.STRING_ID_COLUMN_SIZE);
            Create.PrimaryKey(DatabaseConstant.PRIMARY_KEY_ITEMS).OnTable(DatabaseConstant.ITEMS_TABLE_NAME).Column(DatabaseConstant.ID_COLUMN_NAME);

            Delete.DefaultConstraint().OnTable(DatabaseConstant.ITEMS_TABLE_NAME).OnColumn(DatabaseConstant.DELETED_AT_UTC_COLUMN_NAME);
            Delete.Column(DatabaseConstant.DELETED_AT_UTC_COLUMN_NAME).FromTable(DatabaseConstant.ITEMS_TABLE_NAME);

            Delete.DefaultConstraint().OnTable(DatabaseConstant.ITEMS_TABLE_NAME).OnColumn(DatabaseConstant.IS_DELETED_COLUMN_NAME);
            Delete.Column(DatabaseConstant.IS_DELETED_COLUMN_NAME).FromTable(DatabaseConstant.ITEMS_TABLE_NAME);

            Alter.Table(DatabaseConstant.ITEMS_TABLE_NAME).AlterColumn(DatabaseConstant.NAME_COLUMN_NAME).AsString(150).Unique().NotNullable();

            Create.Column(DatabaseConstant.CODE_COLUMN_NAME).OnTable(DatabaseConstant.ITEMS_TABLE_NAME).AsInt32().Unique().NotNullable();
        }

        public override void Down()
        {
            Create.ForeignKey(DatabaseConstant.FOREIGN_KEY_ORDERS_ITEM_ID_ITEMS_ID).FromTable(DatabaseConstant.ORDERS_TABLE_NAME)
                    .ForeignColumn(DatabaseConstant.ITEM_ID_COLUMN_NAME).ToTable(DatabaseConstant.ITEMS_TABLE_NAME)
                    .PrimaryColumn(DatabaseConstant.ID_COLUMN_NAME);

            Delete.PrimaryKey(DatabaseConstant.PRIMARY_KEY_ITEMS).FromTable(DatabaseConstant.ITEMS_TABLE_NAME);
            Alter.Table(DatabaseConstant.ITEMS_TABLE_NAME).AlterColumn(DatabaseConstant.ID_COLUMN_NAME).AsInt32();
            Create.PrimaryKey(DatabaseConstant.PRIMARY_KEY_ITEMS).OnTable(DatabaseConstant.ITEMS_TABLE_NAME).Column(DatabaseConstant.ID_COLUMN_NAME);

            Create.Column(DatabaseConstant.DELETED_AT_UTC_COLUMN_NAME).OnTable(DatabaseConstant.ITEMS_TABLE_NAME).AsDateTime2().Nullable().WithDefaultValue(null);

            Create.Column(DatabaseConstant.IS_DELETED_COLUMN_NAME).OnTable(DatabaseConstant.ITEMS_TABLE_NAME).AsBoolean().NotNullable().WithDefaultValue(false);

            Delete.UniqueConstraint().FromTable(DatabaseConstant.ITEMS_TABLE_NAME).Column(DatabaseConstant.NAME_COLUMN_NAME);
            Alter.Table(DatabaseConstant.ITEMS_TABLE_NAME).AlterColumn(DatabaseConstant.NAME_COLUMN_NAME)
                                                                    .AsCustom(string.Format(DatabaseConstant.NVARCHAR_DATA_TYPE_TEMPLATE, 
                                                                            DatabaseConstant.ITEM_NAME_COLUMN_SIZE)).NotNullable();

            Delete.Column(DatabaseConstant.CODE_COLUMN_NAME).FromTable(DatabaseConstant.ITEMS_TABLE_NAME);
        }
    }
}