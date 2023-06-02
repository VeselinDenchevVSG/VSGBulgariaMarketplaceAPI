namespace VSGBulgariaMarketplace.Persistence.Migrations
{
    using FluentMigrator;

    [Migration(202306021001)]
    public class DeleteUniqueConstraintCodeNameLocationInTableItems : Migration
    {
        private const string ITEMS_TABLE_NAME = "Items";

        private const string NAME_COLUMN_NAME = "Name";
        private const string CODE_COLUMN_NAME = "Code";
        private const string LOCATION_COLUMN_NAME = "Location";

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
