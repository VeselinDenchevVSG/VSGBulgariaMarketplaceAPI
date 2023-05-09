namespace VSGBulgariaMarketplace.Persistence.Migrations
{
    using FluentMigrator;

    using VSGBulgariaMarketplace.Persistence.Migrations.Abstraction;

    [Migration(202305041526)]
    public class CreateTableOrders : CreateTableMigration
    {
        protected override string TableName => "[Orders]";

        public override void Up()
        {
            Create.Table(this.TableName)
                   .WithColumn("[Id]").AsInt32().PrimaryKey().Identity()
                   .WithColumn("[ItemId]").AsInt32().ForeignKey("[Items]", "[Id]").NotNullable()
                   .WithColumn("[Quantity]").AsInt16().NotNullable()
                   .WithColumn("[Email]").AsString(30).NotNullable()
                   .WithColumn("[Status]").AsInt32().NotNullable()
                   .WithColumn("[CreatedAtUtc]").AsDateTime2().NotNullable().WithDefaultValue(SystemMethods.CurrentUTCDateTime)
                   .WithColumn("[ModifiedAtUtc]").AsDateTime2().NotNullable().WithDefaultValue(SystemMethods.CurrentUTCDateTime)
                   .WithColumn("[DeletedAtUtc]").AsDateTime2().Nullable().WithDefaultValue(null)
                   .WithColumn("[IsDeleted]").AsBoolean().NotNullable().WithDefaultValue(false);

            Execute.Sql($"ALTER TABLE {this.TableName} ADD CONSTRAINT [CK_Order_Quantity] CHECK ([Quantity] >= 0)");
        }
    }
}
