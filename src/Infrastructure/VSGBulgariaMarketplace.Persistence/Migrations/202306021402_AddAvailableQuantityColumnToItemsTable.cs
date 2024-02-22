namespace VSGBulgariaMarketplace.Persistence.Migrations
{
    using FluentMigrator;

    using static VSGBulgariaMarketplace.Persistence.Constants.DatabaseConstant;

    [Migration(ADD_AVAILABLE_QUANTITY_COLUMN_TO_ITEMS_TABLE_MIGRATION_VERSION)]
    public class AddAvailableQuantityColumnToItemsTable : Migration
    {
        public override void Up()
        {
            Alter.Table(ITEMS_TABLE_NAME).AddColumn(AVAILABLE_QUANTITY_COLUMN_NAME).AsInt16().Nullable();

            Execute.Sql(string.Format(ADD_CONSTRAINT_SQL_QUERY_TEMPLATE, ITEMS_TABLE_NAME, CHECK_AVAILABLE_QUANTITY_CONSTRAINT_NAME, 
                                    $"({AVAILABLE_QUANTITY_COLUMN_NAME} >= 0)"));
        }

        public override void Down()
        {
            Execute.Sql(string.Format(DROP_CONSTRAINT_SQL_QUERY_TEMPLATE, ITEMS_TABLE_NAME, CHECK_AVAILABLE_QUANTITY_CONSTRAINT_NAME));

            Delete.Column(AVAILABLE_QUANTITY_COLUMN_NAME).FromTable(ITEMS_TABLE_NAME);
        }
    }
}