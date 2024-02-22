namespace VSGBulgariaMarketplace.Persistence.Migrations
{
    using FluentMigrator;

    using VSGBulgariaMarketplace.Persistence.Migrations.Abstraction;

    using static VSGBulgariaMarketplace.Persistence.Constants.DatabaseConstant;

    [Migration(CREATE_TABLE_LOGS_MIGRATION_VERSION)]
    public class CreateTableLogs : CreateTableMigration
    {
        protected override string TableName => LOGS_TABLE_NAME;

        public override void Up()
        {
            Create.Table(this.TableName)
                   .WithColumn(ID_COLUMN_NAME).AsInt32().PrimaryKey().Identity()
                   .WithColumn(LEVEL_COLUMN_NAME).AsString(LEVEL_COLUMN_SIZE).NotNullable()
                   .WithColumn(CALL_SITE_COLUMN_NAME).AsString(int.MaxValue).NotNullable()
                   .WithColumn(TYPE_COLUMN_NAME).AsString(int.MaxValue).NotNullable()
                   .WithColumn(MESSAGE_COLUMN_NAME).AsString(int.MaxValue).NotNullable()
                   .WithColumn(STACK_TRACE_COLUMN_NAME).AsString(int.MaxValue).NotNullable()
                   .WithColumn(INNER_EXCEPTION_COLUMN_NAME).AsString(int.MaxValue).NotNullable()
                   .WithColumn(ADDITIONAL_INFO_COLUMN_NAME).AsString(int.MaxValue).NotNullable()
                   .WithColumn(LOGGED_ON_DATETIME_UTC_COLUMN_NAME).AsCustom(string.Format(DATETIME2_DATA_TYPE_TEMPLATE, LOGGED_ON_DATETIME_UTC_COLUMN_SIZE))
                                                                  .NotNullable().WithDefaultValue(DateTime.UtcNow);
        }
    }
}
