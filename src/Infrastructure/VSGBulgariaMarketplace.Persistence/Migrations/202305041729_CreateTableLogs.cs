namespace VSGBulgariaMarketplace.Persistence.Migrations
{
    using FluentMigrator;

    using VSGBulgariaMarketplace.Persistence.Constants;
    using VSGBulgariaMarketplace.Persistence.Migrations.Abstraction;

    [Migration(DatabaseConstant.CREATE_TABLE_LOGS_MIGRATION_VERSION)]
    public class CreateTableLogs : CreateTableMigration
    {
        protected override string TableName => DatabaseConstant.LOGS_TABLE_NAME;

        public override void Up()
        {
            Create.Table(this.TableName)
                   .WithColumn(DatabaseConstant.ID_COLUMN_NAME).AsInt32().PrimaryKey().Identity()
                   .WithColumn(DatabaseConstant.LEVEL_COLUMN_NAME).AsString(DatabaseConstant.LEVEL_COLUMN_SIZE).NotNullable()
                   .WithColumn(DatabaseConstant.CALL_SITE_COLUMN_NAME).AsString(int.MaxValue).NotNullable()
                   .WithColumn(DatabaseConstant.TYPE_COLUMN_NAME).AsString(int.MaxValue).NotNullable()
                   .WithColumn(DatabaseConstant.MESSAGE_COLUMN_NAME).AsString(int.MaxValue).NotNullable()
                   .WithColumn(DatabaseConstant.STACK_TRACE_COLUMN_NAME).AsString(int.MaxValue).NotNullable()
                   .WithColumn(DatabaseConstant.INNER_EXCEPTION_COLUMN_NAME).AsString(int.MaxValue).NotNullable()
                   .WithColumn(DatabaseConstant.ADDITIONAL_INFO_COLUMN_NAME).AsString(int.MaxValue).NotNullable()
                   .WithColumn(DatabaseConstant.LOGGED_ON_DATETIME_UTC_COLUMN_NAME).AsCustom(string.Format(DatabaseConstant.DATETIME2_DATA_TYPE_TEMPLATE, 
                                                                                                        DatabaseConstant.LOGGED_ON_DATETIME_UTC_COLUMN_SIZE))
                                                                                            .NotNullable().WithDefaultValue(DateTime.UtcNow);
        }
    }
}
