namespace VSGBulgariaMarketplace.Persistence.Migrations
{
    using FluentMigrator;

    using static VSGBulgariaMarketplace.Persistence.Constants.DatabaseConstant;

    [Migration(DELETE_UNIQUE_CONSTRAINT_CODE_NAME_LOCATION_IN_TABLE_ITEMS_MIGRATION_VERSION)]
    public class DeleteUniqueConstraintCodeNameLocationInTableItems : Migration
    {
        public override void Up()
        {
            Delete.UniqueConstraint().FromTable(ITEMS_TABLE_NAME).Columns(NAME_COLUMN_NAME, CODE_COLUMN_NAME, LOCATION_COLUMN_NAME);
        }

        public override void Down()
        {
            Create.UniqueConstraint().OnTable(ITEMS_TABLE_NAME).Columns(NAME_COLUMN_NAME, CODE_COLUMN_NAME, LOCATION_COLUMN_NAME);
        }
    }
}
