namespace VSGBulgariaMarketplace.Persistence.Migrations
{
    using FluentMigrator;

    using VSGBulgariaMarketplace.Persistence.Migrations.Abstraction;
    
    using static VSGBulgariaMarketplace.Persistence.Constants.DatabaseConstant;

    [Migration(CREATE_TABLE_ITEMS_MIGRATION_VERSION)]
    public class CreateTableItems : CreateTableMigration
    {
        protected override string TableName => ITEMS_TABLE_NAME;

        public override void Up()
        {
            Create.Table(this.TableName)
                   .WithColumn(ID_COLUMN_NAME).AsInt32().PrimaryKey()
                   .WithColumn(NAME_COLUMN_NAME).AsCustom(string.Format(NVARCHAR_DATA_TYPE_TEMPLATE, ITEM_NAME_COLUMN_SIZE)).NotNullable()
                   .WithColumn(IMAGE_PUBLIC_ID_COLUMN_NAME).AsString(INT_ID_COLUMN_SIZE)
                                                                                    .ForeignKey(CLOUDINARY_IMAGES_TABLE_NAME, 
                                                                                                ID_COLUMN_NAME).Nullable()
                   .WithColumn(PRICE_COLUMN_NAME).AsCustom(MONEY_DATA_TYPE).NotNullable()
                   .WithColumn(CATEGORY_COLUMN_NAME).AsInt32().NotNullable()
                   .WithColumn(QUANTITY_COMBINED_COLUMN_NAME).AsInt16().NotNullable()
                   .WithColumn(QUANTITY_FOR_SALE_COLUMN_NAME).AsInt16().Nullable()
                   .WithColumn(DESCRIPTION_COLUMN_NAME).AsCustom(string.Format(NVARCHAR_DATA_TYPE_TEMPLATE, DESCRIPTION_COLUMN_SIZE)).Nullable()
                   .WithColumn(CREATED_AT_UTC_COLUMN_NAME).AsDateTime2().NotNullable().WithDefaultValue(SystemMethods.CurrentUTCDateTime)
                   .WithColumn(MODIFIED_AT_UTC_COLUMN_NAME).AsDateTime2().NotNullable().WithDefaultValue(SystemMethods.CurrentUTCDateTime)
                   .WithColumn(DELETED_AT_UTC_COLUMN_NAME).AsDateTime2().Nullable().WithDefaultValue(null)
                   .WithColumn(IS_DELETED_COLUMN_NAME).AsBoolean().NotNullable().WithDefaultValue(false);

            Execute.Sql(string.Format(ADD_CONSTRAINT_SQL_QUERY_TEMPLATE, this.TableName, CHECK_ITEM_QUANTITY_COMBINED_CONSTRAINT_NAME, $"({QUANTITY_COMBINED_COLUMN_NAME} >= 0)"));
            Execute.Sql(string.Format(ADD_CONSTRAINT_SQL_QUERY_TEMPLATE, this.TableName, CHECK_ITEM_QUANTITY_FOR_SALE_CONSTRAINT_NAME,
                                      $"({QUANTITY_FOR_SALE_COLUMN_NAME} >= 0 AND " + $"{QUANTITY_FOR_SALE_COLUMN_NAME} <= {QUANTITY_COMBINED_COLUMN_NAME})"));
        }
    }
}
