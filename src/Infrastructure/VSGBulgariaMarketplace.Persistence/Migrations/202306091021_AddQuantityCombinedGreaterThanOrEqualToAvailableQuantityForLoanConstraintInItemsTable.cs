namespace VSGBulgariaMarketplace.Persistence.Migrations
{
    using FluentMigrator;

    [Migration(202306091021)]
    public class AddQuantityCombinedGreaterThanOrEqualToAvailableQuantityForLoanConstraintInItemsTable : Migration
    {
        private const string ITEMS_TABLE_NAME = "Items";
        private const string AVAILABLE_QUANTITY_COLUMN_NAME = "AvailableQuantity";
        private const string AVAILABLE_QUANTITY_CHECK_CONSTRAINT_NAME = "CK_Item_AvailableQuantity";

        public override void Up()
        {
            this.DropConstraint();

            Execute.Sql($"ALTER TABLE {ITEMS_TABLE_NAME} ADD CONSTRAINT {AVAILABLE_QUANTITY_CHECK_CONSTRAINT_NAME} CHECK ({AVAILABLE_QUANTITY_COLUMN_NAME} >= 0 AND " +
                        $"{AVAILABLE_QUANTITY_COLUMN_NAME} <= QuantityCombined - QuantityForSale)");
        }

        public override void Down()
        {
            this.DropConstraint();

            Execute.Sql($"ALTER TABLE {ITEMS_TABLE_NAME} ADD CONSTRAINT {AVAILABLE_QUANTITY_CHECK_CONSTRAINT_NAME} CHECK ({AVAILABLE_QUANTITY_COLUMN_NAME} >= 0)");
        }

        private void DropConstraint() => Execute.Sql($"ALTER TABLE {ITEMS_TABLE_NAME} DROP CONSTRAINT {AVAILABLE_QUANTITY_CHECK_CONSTRAINT_NAME}");
    }
}