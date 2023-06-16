namespace VSGBulgariaMarketplace.Persistence.Migrations
{
    using FluentMigrator;

    using VSGBulgariaMarketplace.Persistence.Constants;
    using VSGBulgariaMarketplace.Persistence.Migrations.Abstraction;

    [Migration(DatabaseConstant.CREATE_TABLE_ITEM_LOANS_MIGRATION_VERSION)]
    public class CreateTableItemLoans : CreateTableMigration
    {
        protected override string TableName => DatabaseConstant.ITEM_LOANS_TABLE_NAME;

        public override void Up()
        {
            Create.Table(this.TableName)
                   .WithColumn(DatabaseConstant.ID_COLUMN_NAME).AsString(DatabaseConstant.STRING_ID_COLUMN_SIZE).PrimaryKey()
                   .WithColumn(DatabaseConstant.ITEM_ID_COLUMN_NAME).AsString(DatabaseConstant.STRING_ID_COLUMN_SIZE)
                                                                            .ForeignKey(DatabaseConstant.ITEMS_TABLE_NAME, 
                                                                                        DatabaseConstant.ID_COLUMN_NAME)
                                                .NotNullable()
                   .WithColumn(DatabaseConstant.EMAIL_COLUMN_NAME).AsString(30).NotNullable()
                   .WithColumn(DatabaseConstant.QUANTITY_COLUMN_NAME).AsInt16().NotNullable()
                   .WithColumn(DatabaseConstant.END_DATE_TIME_UTC_COLUMN_NAME).AsDateTime2().Nullable()
                   .WithColumn(DatabaseConstant.CREATED_AT_UTC_COLUMN_NAME).AsDateTime2().NotNullable().WithDefaultValue(SystemMethods.CurrentUTCDateTime)
                   .WithColumn(DatabaseConstant.MODIFIED_AT_UTC_COLUMN_NAME).AsDateTime2().NotNullable().WithDefaultValue(SystemMethods.CurrentUTCDateTime);
        }
    }
}
