namespace VSGBulgariaMarketplace.Persistence.Migrations
{
    using FluentMigrator;

    using VSGBulgariaMarketplace.Persistence.Migrations.Abstraction;
    
    using static VSGBulgariaMarketplace.Persistence.Constants.DatabaseConstant;

    [Migration(CREATE_TABLE_ITEM_LOANS_MIGRATION_VERSION)]
    public class CreateTableItemLoans : CreateTableMigration
    {
        protected override string TableName => ITEM_LOANS_TABLE_NAME;

        public override void Up()
        {
            Create.Table(this.TableName)
                   .WithColumn(ID_COLUMN_NAME).AsString(STRING_ID_COLUMN_SIZE).PrimaryKey()
                   .WithColumn(ITEM_ID_COLUMN_NAME).AsString(STRING_ID_COLUMN_SIZE)
                                                                            .ForeignKey(ITEMS_TABLE_NAME, 
                                                                                        ID_COLUMN_NAME)
                                                .NotNullable()
                   .WithColumn(EMAIL_COLUMN_NAME).AsString(30).NotNullable()
                   .WithColumn(QUANTITY_COLUMN_NAME).AsInt16().NotNullable()
                   .WithColumn(END_DATE_TIME_UTC_COLUMN_NAME).AsDateTime2().Nullable()
                   .WithColumn(CREATED_AT_UTC_COLUMN_NAME).AsDateTime2().NotNullable().WithDefaultValue(SystemMethods.CurrentUTCDateTime)
                   .WithColumn(MODIFIED_AT_UTC_COLUMN_NAME).AsDateTime2().NotNullable().WithDefaultValue(SystemMethods.CurrentUTCDateTime);
        }
    }
}
