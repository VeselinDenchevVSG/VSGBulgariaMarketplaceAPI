namespace VSGBulgariaMarketplace.Persistence.Migrations
{
    using FluentMigrator;

    using VSGBulgariaMarketplace.Persistence.Constants;

    [Migration(DatabaseConstant.ADD_QUANTITY_COMBINED_GREATER_THAN_OR_EQUAL_TO_AVAILABLE_QUANTITY_FOR_LOAN_CONSTRAINT_IN_ITEM_TABLE_MIGRATION_VERSION)]
    public class AddQuantityCombinedGreaterThanOrEqualToAvailableQuantityForLoanConstraintInItemsTable : Migration
    {
        public override void Up()
        {
            this.DropConstraint();

            Execute.Sql(string.Format(DatabaseConstant.ADD_CONSTRAINT_SQL_QUERY_TEMPLATE, DatabaseConstant.ITEMS_TABLE_NAME,
                                                    DatabaseConstant.CHECK_AVAILABLE_QUANTITY_CONSTRAINT_NAME, 
                                                    $"({DatabaseConstant.AVAILABLE_QUANTITY_COLUMN_NAME} >= 0 AND {DatabaseConstant.AVAILABLE_QUANTITY_COLUMN_NAME} <= " +
                                                    $"{DatabaseConstant.QUANTITY_COMBINED_COLUMN_NAME} - {DatabaseConstant.QUANTITY_FOR_SALE_COLUMN_NAME})"));
        }

        public override void Down()
        {
            this.DropConstraint();

            Execute.Sql(string.Format(DatabaseConstant.ADD_CONSTRAINT_SQL_QUERY_TEMPLATE, DatabaseConstant.ITEMS_TABLE_NAME,
                                                    DatabaseConstant.CHECK_AVAILABLE_QUANTITY_CONSTRAINT_NAME, $"({DatabaseConstant.AVAILABLE_QUANTITY_COLUMN_NAME} >= 0)"));
        }

        private void DropConstraint() => Execute.Sql(string.Format(DatabaseConstant.DROP_CONSTRAINT_SQL_QUERY_TEMPLATE, DatabaseConstant.ITEMS_TABLE_NAME,
                                                                                DatabaseConstant.CHECK_AVAILABLE_QUANTITY_CONSTRAINT_NAME));
    }
}