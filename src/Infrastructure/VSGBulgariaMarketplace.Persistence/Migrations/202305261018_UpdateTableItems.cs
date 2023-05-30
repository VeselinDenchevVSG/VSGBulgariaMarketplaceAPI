namespace VSGBulgariaMarketplace.Persistence.Migrations
{
    using FluentMigrator;

    [Migration(202305261018)]
    public class UpdateTableItems : Migration
    {
        private const string ITEMS_TABLE_NAME = "Items";
        private const string ORDERS_TABLE_NAME = "Orders";

        private const string ID_COLUMN_NAME = "Id";
        private const string DELETED_AT_UTC_COLUMN_NAME = "DeletedAtUtc";
        private const string IS_DELETED_COLUMN_NAME = "IsDeleted";
        private const string CODE_COLUMN_NAME = "Code";
        private const string NAME_COLUMN_NAME = "Name";
        private const string ITEM_ID_COLUMN_NAME = "ItemId";

        private const string PRIMARY_KEY_ITEMS = "PK_Items";
        private const string FOREIGN_KEY_ORDERS_ITEM_ID_ITEMS_ID = "FK_Orders_ItemId_Items_Id";

        public override void Up()
        {
            Delete.ForeignKey(FOREIGN_KEY_ORDERS_ITEM_ID_ITEMS_ID).OnTable(ORDERS_TABLE_NAME);

            Delete.PrimaryKey(PRIMARY_KEY_ITEMS).FromTable(ITEMS_TABLE_NAME);
            Alter.Table(ITEMS_TABLE_NAME).AlterColumn(ID_COLUMN_NAME).AsString(36);
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
            Create.ForeignKey(FOREIGN_KEY_ORDERS_ITEM_ID_ITEMS_ID).FromTable(ORDERS_TABLE_NAME).ForeignColumn(ITEM_ID_COLUMN_NAME)
                                                                    .ToTable(ITEMS_TABLE_NAME).PrimaryColumn(ID_COLUMN_NAME);

            Delete.PrimaryKey(PRIMARY_KEY_ITEMS).FromTable(ITEMS_TABLE_NAME);
            Alter.Table(ITEMS_TABLE_NAME).AlterColumn(ID_COLUMN_NAME).AsInt32();
            Create.PrimaryKey(PRIMARY_KEY_ITEMS).OnTable(ITEMS_TABLE_NAME).Column(ID_COLUMN_NAME);

            Create.Column(DELETED_AT_UTC_COLUMN_NAME).OnTable(ITEMS_TABLE_NAME).AsDateTime2().Nullable().WithDefaultValue(null);

            Create.Column(IS_DELETED_COLUMN_NAME).OnTable(ITEMS_TABLE_NAME).AsBoolean().NotNullable().WithDefaultValue(false);

            Delete.UniqueConstraint().FromTable(ITEMS_TABLE_NAME).Column(NAME_COLUMN_NAME);
            Alter.Table(ITEMS_TABLE_NAME).AlterColumn(NAME_COLUMN_NAME).AsCustom("NVARCHAR(150)").NotNullable();

            Delete.Column(CODE_COLUMN_NAME).FromTable(ITEMS_TABLE_NAME);
        }
    }
}