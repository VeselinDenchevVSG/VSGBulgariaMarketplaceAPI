namespace VSGBulgariaMarketplace.Persistence.Migrations
{
    using FluentMigrator;

    using VSGBulgariaMarketplace.Persistence.Constants;
    using VSGBulgariaMarketplace.Persistence.Migrations.Abstraction;

    [Migration(DatabaseConstant.CREATE_TABLE_ITEMS_MIGRATION_VERSION)]
    public class CreateTableItems : CreateTableMigration
    {
        protected override string TableName => DatabaseConstant.ITEMS_TABLE_NAME;

        public override void Up()
        {
            Create.Table(this.TableName)
                   .WithColumn(DatabaseConstant.ID_COLUMN_NAME).AsInt32().PrimaryKey()
                   .WithColumn(DatabaseConstant.NAME_COLUMN_NAME).AsCustom(string.Format(DatabaseConstant.NVARCHAR_DATA_TYPE_TEMPLATE, 
                                                                                                        DatabaseConstant.ITEM_NAME_COLUMN_SIZE)).NotNullable()
                   .WithColumn(DatabaseConstant.IMAGE_PUBLIC_ID_COLUMN_NAME).AsString(DatabaseConstant.INT_ID_COLUMN_SIZE)
                                                                                    .ForeignKey(DatabaseConstant.CLOUDINARY_IMAGES_TABLE_NAME, 
                                                                                                DatabaseConstant.ID_COLUMN_NAME).Nullable()
                   .WithColumn(DatabaseConstant.PRICE_COLUMN_NAME).AsCustom(DatabaseConstant.MONEY_DATA_TYPE).NotNullable()
                   .WithColumn(DatabaseConstant.CATEGORY_COLUMN_NAME).AsInt32().NotNullable()
                   .WithColumn(DatabaseConstant.QUANTITY_COMBINED_COLUMN_NAME).AsInt16().NotNullable()
                   .WithColumn(DatabaseConstant.QUANTITY_FOR_SALE_COLUMN_NAME).AsInt16().Nullable()
                   .WithColumn(DatabaseConstant.DESCRIPTION_COLUMN_NAME).AsCustom(string.Format(DatabaseConstant.NVARCHAR_DATA_TYPE_TEMPLATE,
                                                                                                                    DatabaseConstant.DESCRIPTION_COLUMN_SIZE)).Nullable()
                   .WithColumn(DatabaseConstant.CREATED_AT_UTC_COLUMN_NAME).AsDateTime2().NotNullable().WithDefaultValue(SystemMethods.CurrentUTCDateTime)
                   .WithColumn(DatabaseConstant.MODIFIED_AT_UTC_COLUMN_NAME).AsDateTime2().NotNullable().WithDefaultValue(SystemMethods.CurrentUTCDateTime)
                   .WithColumn(DatabaseConstant.DELETED_AT_UTC_COLUMN_NAME).AsDateTime2().Nullable().WithDefaultValue(null)
                   .WithColumn(DatabaseConstant.IS_DELETED_COLUMN_NAME).AsBoolean().NotNullable().WithDefaultValue(false);

            Execute.Sql(string.Format(DatabaseConstant.ADD_CONSTRAINT_SQL_QUERY_TEMPLATE, this.TableName,
                                                    DatabaseConstant.CHECK_ITEM_QUANTITY_COMBINED_CONSTRAINT_NAME,
                                                    $"({DatabaseConstant.QUANTITY_COMBINED_COLUMN_NAME} >= 0)"));
            Execute.Sql(string.Format(DatabaseConstant.ADD_CONSTRAINT_SQL_QUERY_TEMPLATE, this.TableName,
                                                    DatabaseConstant.CHECK_ITEM_QUANTITY_FOR_SALE_CONSTRAINT_NAME,
                                                    $"({DatabaseConstant.QUANTITY_FOR_SALE_COLUMN_NAME} >= 0 AND " +
                                                    $"{DatabaseConstant.QUANTITY_FOR_SALE_COLUMN_NAME} <= {DatabaseConstant.QUANTITY_COMBINED_COLUMN_NAME})"));
        }
    }
}
