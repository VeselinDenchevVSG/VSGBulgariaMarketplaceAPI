namespace VSGBulgariaMarketplace.Persistence.Migrations
{
    using FluentMigrator;

    using static VSGBulgariaMarketplace.Persistence.Constants.DatabaseConstant;

    [Migration(MAKE_PRICE_IN_ITEMS_TABLE_NULLABLE_MIGRATION_VERSION)]
    public class MakePriceColumnInItemsTableNullable : Migration
    {
        public override void Up()
        {
            Alter.Table(ITEMS_TABLE_NAME).AlterColumn(PRICE_COLUMN_NAME).AsCustom(MONEY_DATA_TYPE).Nullable();
        }

        public override void Down()
        {
            Alter.Table(ITEMS_TABLE_NAME).AlterColumn(PRICE_COLUMN_NAME).AsCustom(MONEY_DATA_TYPE).NotNullable();
        }

    }
}
