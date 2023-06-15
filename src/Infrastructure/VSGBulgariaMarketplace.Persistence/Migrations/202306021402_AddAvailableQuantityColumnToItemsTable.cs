namespace VSGBulgariaMarketplace.Persistence.Migrations
{
    using FluentMigrator;

    using VSGBulgariaMarketplace.Persistence.Constants;

    [Migration(DatabaseConstant.ADD_AVAILABLE_QUANTITY_COLUMN_TO_ITEMS_TABLE_MIGRATION_VERSION)]
    public class AddAvailableQuantityColumnToItemsTable : Migration
    {
        public override void Up()
        {
            Alter.Table(DatabaseConstant.ITEMS_TABLE_NAME).AddColumn(DatabaseConstant.AVAILABLE_QUANTITY_COLUMN_NAME).AsInt16().Nullable();

            Execute.Sql(string.Format(DatabaseConstant.ADD_CONSTRAINT_SQL_QUERY_TEMPLATE, DatabaseConstant.ITEMS_TABLE_NAME,
                                                    DatabaseConstant.CHECK_AVAILABLE_QUANTITY_CONSTRAINT_NAME, $"({DatabaseConstant.AVAILABLE_QUANTITY_COLUMN_NAME} >= 0)"));
        }

        public override void Down()
        {
            Execute.Sql(string.Format(DatabaseConstant.DROP_CONSTRAINT_SQL_QUERY_TEMPLATE, DatabaseConstant.ITEMS_TABLE_NAME,
                                                    DatabaseConstant.CHECK_AVAILABLE_QUANTITY_CONSTRAINT_NAME));

            Delete.Column(DatabaseConstant.AVAILABLE_QUANTITY_COLUMN_NAME).FromTable(DatabaseConstant.ITEMS_TABLE_NAME);
        }
    }
}