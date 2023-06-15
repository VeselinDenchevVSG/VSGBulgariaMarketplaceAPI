namespace VSGBulgariaMarketplace.Persistence.Migrations
{
    using FluentMigrator;

    using System.Data;

    using VSGBulgariaMarketplace.Persistence.Constants;
    using VSGBulgariaMarketplace.Persistence.Migrations.Abstraction;

    [Migration(DatabaseConstant.CREATE_TABLE_ORDERS_MIGRATION_VERSION)]
    public class CreateTableOrders : CreateTableMigration
    {
        protected override string TableName => DatabaseConstant.ORDERS_TABLE_NAME;

        public override void Up()
        {
            Create.Table(this.TableName)
                   .WithColumn(DatabaseConstant.ID_COLUMN_NAME).AsInt32().PrimaryKey().Identity()
                   .WithColumn(DatabaseConstant.ITEM_ID_COLUMN_NAME).AsInt32().ForeignKey(DatabaseConstant.ITEMS_TABLE_NAME, 
                                                                                                DatabaseConstant.ID_COLUMN_NAME).OnUpdate(Rule.Cascade)
                                                                                    .NotNullable()
                   .WithColumn(DatabaseConstant.QUANTITY_COLUMN_NAME).AsInt16().NotNullable()
                   .WithColumn(DatabaseConstant.EMAIL_COLUMN_NAME).AsString(DatabaseConstant.EMAIL_COLUMN_SIZE).NotNullable()
                   .WithColumn(DatabaseConstant.STATUS_COLUMN_NAME).AsInt32().NotNullable()
                   .WithColumn(DatabaseConstant.CREATED_AT_UTC_COLUMN_NAME).AsDateTime2().NotNullable().WithDefaultValue(SystemMethods.CurrentUTCDateTime)
                   .WithColumn(DatabaseConstant.MODIFIED_AT_UTC_COLUMN_NAME).AsDateTime2().NotNullable().WithDefaultValue(SystemMethods.CurrentUTCDateTime)
                   .WithColumn(DatabaseConstant.DELETED_AT_UTC_COLUMN_NAME).AsDateTime2().Nullable().WithDefaultValue(null)
                   .WithColumn(DatabaseConstant.IS_DELETED_COLUMN_NAME).AsBoolean().NotNullable().WithDefaultValue(false);

            Execute.Sql(string.Format(DatabaseConstant.ADD_CONSTRAINT_SQL_QUERY_TEMPLATE, this.TableName, 
                                                    DatabaseConstant.CHECK_ORDER_QUANTITY_CONSTRAINT_NAME, $"({DatabaseConstant.QUANTITY_COLUMN_NAME} >= 0)"));
        }
    }
}
