namespace VSGBulgariaMarketplace.Persistence.Migrations
{
    using FluentMigrator;

    [Migration(202306011030)]
    public class ChangeUniqueConstraintsInItemsTable : Migration
    {
        private const string ITEMS_TABLE_NAME = "Items";

        private const string NAME_COLUMN_NAME = "Name";
        private const string CODE_COLUMN_NAME = "Code";
        private const string LOCATION_COLUMN_NAME = "Location";

        public override void Up()
        {
            Delete.Index().OnTable(ITEMS_TABLE_NAME).OnColumn(NAME_COLUMN_NAME);
            Delete.Index().OnTable(ITEMS_TABLE_NAME).OnColumn(CODE_COLUMN_NAME);

            Create.UniqueConstraint().OnTable(ITEMS_TABLE_NAME).Columns(CODE_COLUMN_NAME, LOCATION_COLUMN_NAME);
            Create.UniqueConstraint().OnTable(ITEMS_TABLE_NAME).Columns(NAME_COLUMN_NAME, LOCATION_COLUMN_NAME);
            Create.UniqueConstraint().OnTable(ITEMS_TABLE_NAME).Columns(NAME_COLUMN_NAME, CODE_COLUMN_NAME, LOCATION_COLUMN_NAME);
        }

        public override void Down()
        {
            Delete.UniqueConstraint().FromTable(ITEMS_TABLE_NAME).Columns(CODE_COLUMN_NAME, LOCATION_COLUMN_NAME);
            Delete.UniqueConstraint().FromTable(ITEMS_TABLE_NAME).Columns(NAME_COLUMN_NAME, LOCATION_COLUMN_NAME);
            Delete.UniqueConstraint().FromTable(ITEMS_TABLE_NAME).Columns(NAME_COLUMN_NAME, CODE_COLUMN_NAME, LOCATION_COLUMN_NAME);

            Create.UniqueConstraint().OnTable(ITEMS_TABLE_NAME).Columns(NAME_COLUMN_NAME);
            Create.UniqueConstraint().OnTable(ITEMS_TABLE_NAME).Columns(CODE_COLUMN_NAME);
        }
    }
}
