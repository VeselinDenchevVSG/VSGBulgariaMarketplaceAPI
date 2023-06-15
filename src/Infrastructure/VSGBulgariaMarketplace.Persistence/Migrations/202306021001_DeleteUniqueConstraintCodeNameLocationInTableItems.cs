namespace VSGBulgariaMarketplace.Persistence.Migrations
{
    using FluentMigrator;

    using VSGBulgariaMarketplace.Persistence.Constants;

    [Migration(DatabaseConstant.DELETE_UNIQUE_CONSTRAINT_CODE_NAME_LOCATION_IN_TABLE_ITEMS_MIGRATION_VERSION)]
    public class DeleteUniqueConstraintCodeNameLocationInTableItems : Migration
    {
        public override void Up()
        {
            Delete.UniqueConstraint().FromTable(DatabaseConstant.ITEMS_TABLE_NAME)
                                        .Columns(DatabaseConstant.NAME_COLUMN_NAME, DatabaseConstant.CODE_COLUMN_NAME, DatabaseConstant.LOCATION_COLUMN_NAME);
        }

        public override void Down()
        {
            Create.UniqueConstraint().OnTable(DatabaseConstant.ITEMS_TABLE_NAME)
                                        .Columns(DatabaseConstant.NAME_COLUMN_NAME, DatabaseConstant.CODE_COLUMN_NAME, DatabaseConstant.LOCATION_COLUMN_NAME);
        }
    }
}
