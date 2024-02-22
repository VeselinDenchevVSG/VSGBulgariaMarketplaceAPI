namespace VSGBulgariaMarketplace.Persistence.Migrations
{
    using FluentMigrator;

    using static VSGBulgariaMarketplace.Persistence.Constants.DatabaseConstant;

    [Migration(ADD_QUANTITY_COMBINED_GREATER_THAN_OR_EQUAL_TO_AVAILABLE_QUANTITY_FOR_LOAN_CONSTRAINT_IN_ITEM_TABLE_MIGRATION_VERSION)]
    public class AddQuantityCombinedGreaterThanOrEqualToAvailableQuantityForLoanConstraintInItemsTable : Migration
    {
        public override void Up()
        {
            this.DropConstraint();

            Execute.Sql(string.Format(ADD_CONSTRAINT_SQL_QUERY_TEMPLATE, ITEMS_TABLE_NAME, CHECK_AVAILABLE_QUANTITY_CONSTRAINT_NAME, 
                                      $"({AVAILABLE_QUANTITY_COLUMN_NAME} >= 0 AND {AVAILABLE_QUANTITY_COLUMN_NAME} <= " +
                                      $"{QUANTITY_COMBINED_COLUMN_NAME} - {QUANTITY_FOR_SALE_COLUMN_NAME})"));
        }

        public override void Down()
        {
            this.DropConstraint();

            Execute.Sql(string.Format(ADD_CONSTRAINT_SQL_QUERY_TEMPLATE, ITEMS_TABLE_NAME, CHECK_AVAILABLE_QUANTITY_CONSTRAINT_NAME, 
                                    $"({AVAILABLE_QUANTITY_COLUMN_NAME} >= 0)"));
        }

        private void DropConstraint() => Execute.Sql(string.Format(DROP_CONSTRAINT_SQL_QUERY_TEMPLATE, ITEMS_TABLE_NAME, CHECK_AVAILABLE_QUANTITY_CONSTRAINT_NAME));
    }
}