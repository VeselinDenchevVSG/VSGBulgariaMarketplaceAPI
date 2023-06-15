namespace VSGBulgariaMarketplace.Persistence.Migrations
{
    using FluentMigrator;

    using VSGBulgariaMarketplace.Persistence.Constants;

    [Migration(DatabaseConstant.CHANGE_UNIQUE_CONSTRAINTS_INT_ITEMS_TABLE_MIGRATION_VERSION)]
    public class ChangeUniqueConstraintsInItemsTable : Migration
    {
        public override void Up()
        {
            Delete.Index().OnTable(DatabaseConstant.ITEMS_TABLE_NAME).OnColumn(DatabaseConstant.NAME_COLUMN_NAME);
            Delete.Index().OnTable(DatabaseConstant.ITEMS_TABLE_NAME).OnColumn(DatabaseConstant.CODE_COLUMN_NAME);

            Create.UniqueConstraint().OnTable(DatabaseConstant.ITEMS_TABLE_NAME)
                                        .Columns(DatabaseConstant.CODE_COLUMN_NAME, DatabaseConstant.LOCATION_COLUMN_NAME);
            Create.UniqueConstraint().OnTable(DatabaseConstant.ITEMS_TABLE_NAME)
                                        .Columns(DatabaseConstant.NAME_COLUMN_NAME, DatabaseConstant.LOCATION_COLUMN_NAME);
            Create.UniqueConstraint().OnTable(DatabaseConstant.ITEMS_TABLE_NAME)
                                        .Columns(DatabaseConstant.NAME_COLUMN_NAME, DatabaseConstant.CODE_COLUMN_NAME, DatabaseConstant.LOCATION_COLUMN_NAME);
        }

        public override void Down()
        {
            Delete.UniqueConstraint().FromTable(DatabaseConstant.ITEMS_TABLE_NAME)
                                        .Columns(DatabaseConstant.CODE_COLUMN_NAME, DatabaseConstant.LOCATION_COLUMN_NAME);
            Delete.UniqueConstraint().FromTable(DatabaseConstant.ITEMS_TABLE_NAME)
                                        .Columns(DatabaseConstant.NAME_COLUMN_NAME, DatabaseConstant.LOCATION_COLUMN_NAME);
            Delete.UniqueConstraint().FromTable(DatabaseConstant.ITEMS_TABLE_NAME)
                                        .Columns(DatabaseConstant.NAME_COLUMN_NAME, DatabaseConstant.CODE_COLUMN_NAME, DatabaseConstant.LOCATION_COLUMN_NAME);

            Create.UniqueConstraint().OnTable(DatabaseConstant.ITEMS_TABLE_NAME).Columns(DatabaseConstant.NAME_COLUMN_NAME);
            Create.UniqueConstraint().OnTable(DatabaseConstant.ITEMS_TABLE_NAME).Columns(DatabaseConstant.CODE_COLUMN_NAME);
        }
    }
}