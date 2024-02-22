namespace VSGBulgariaMarketplace.Persistence.Migrations
{
    using FluentMigrator;

    using static VSGBulgariaMarketplace.Persistence.Constants.DatabaseConstant;

    [Migration(CHANGE_UNIQUE_CONSTRAINTS_INT_ITEMS_TABLE_MIGRATION_VERSION)]
    public class ChangeUniqueConstraintsInItemsTable : Migration
    {
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