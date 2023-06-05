namespace VSGBulgariaMarketplace.Persistence.Migrations
{
    using FluentMigrator;

    using VSGBulgariaMarketplace.Persistence.Migrations.Abstraction;

    [Migration(202306021421)]
    public class CreateTableLoanedItems : CreateTableMigration
    {
        private const string ID_COLUMN_NAME = "Id";

        protected override string TableName => "ItemLoans";

        public override void Up()
        {
            Create.Table(this.TableName)
                   .WithColumn(ID_COLUMN_NAME).AsString(36).PrimaryKey()
                   .WithColumn("ItemId").AsString(36).ForeignKey("Items", ID_COLUMN_NAME).NotNullable()
                   .WithColumn("Email").AsString(30).NotNullable()
                   .WithColumn("Quantity").AsInt16().NotNullable()
                   .WithColumn("EndDatetimeUtc").AsDateTime2().Nullable()
                   .WithColumn("CreatedAtUtc").AsDateTime2().NotNullable().WithDefaultValue(SystemMethods.CurrentUTCDateTime)
                   .WithColumn("ModifiedAtUtc").AsDateTime2().NotNullable().WithDefaultValue(SystemMethods.CurrentUTCDateTime);
        }
    }
}
