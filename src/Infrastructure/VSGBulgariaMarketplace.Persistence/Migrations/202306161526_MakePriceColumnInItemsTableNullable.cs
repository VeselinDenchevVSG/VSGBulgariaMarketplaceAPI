namespace VSGBulgariaMarketplace.Persistence.Migrations
{
    using FluentMigrator;

    using VSGBulgariaMarketplace.Persistence.Constants;

    [Migration(DatabaseConstant.MAKE_PRICE_IN_ITEMS_TABLE_NULLABLE_MIGRATION_VERSION)]
    public class MakePriceColumnInItemsTableNullable : Migration
    {
        public override void Up()
        {
            Alter.Table(DatabaseConstant.ITEMS_TABLE_NAME).AlterColumn(DatabaseConstant.PRICE_COLUMN_NAME)
                                                                    .AsCustom(DatabaseConstant.MONEY_DATA_TYPE).Nullable();
        }

        public override void Down()
        {
            Alter.Table(DatabaseConstant.ITEMS_TABLE_NAME).AlterColumn(DatabaseConstant.PRICE_COLUMN_NAME)
                                                        .AsCustom(DatabaseConstant.MONEY_DATA_TYPE).NotNullable();
        }

    }
}
