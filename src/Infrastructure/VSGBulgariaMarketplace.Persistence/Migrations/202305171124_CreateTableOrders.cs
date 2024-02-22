namespace VSGBulgariaMarketplace.Persistence.Migrations
{
    using FluentMigrator;

    using System.Data;

    using VSGBulgariaMarketplace.Persistence.Migrations.Abstraction;

    using static VSGBulgariaMarketplace.Persistence.Constants.DatabaseConstant;

    [Migration(CREATE_TABLE_ORDERS_MIGRATION_VERSION)]
    public class CreateTableOrders : CreateTableMigration
    {
        protected override string TableName => ORDERS_TABLE_NAME;

        public override void Up()
        {
            Create.Table(this.TableName)
                   .WithColumn(ID_COLUMN_NAME).AsInt32().PrimaryKey().Identity()
                   .WithColumn(ITEM_ID_COLUMN_NAME).AsInt32().ForeignKey(ITEMS_TABLE_NAME,ID_COLUMN_NAME).OnUpdate(Rule.Cascade).NotNullable()
                   .WithColumn(QUANTITY_COLUMN_NAME).AsInt16().NotNullable()
                   .WithColumn(EMAIL_COLUMN_NAME).AsString(EMAIL_COLUMN_SIZE).NotNullable()
                   .WithColumn(STATUS_COLUMN_NAME).AsInt32().NotNullable()
                   .WithColumn(CREATED_AT_UTC_COLUMN_NAME).AsDateTime2().NotNullable().WithDefaultValue(SystemMethods.CurrentUTCDateTime)
                   .WithColumn(MODIFIED_AT_UTC_COLUMN_NAME).AsDateTime2().NotNullable().WithDefaultValue(SystemMethods.CurrentUTCDateTime)
                   .WithColumn(DELETED_AT_UTC_COLUMN_NAME).AsDateTime2().Nullable().WithDefaultValue(null)
                   .WithColumn(IS_DELETED_COLUMN_NAME).AsBoolean().NotNullable().WithDefaultValue(false);

            Execute.Sql(string.Format(ADD_CONSTRAINT_SQL_QUERY_TEMPLATE, this.TableName, CHECK_ORDER_QUANTITY_CONSTRAINT_NAME, $"({QUANTITY_COLUMN_NAME} >= 0)"));
        }
    }
}
