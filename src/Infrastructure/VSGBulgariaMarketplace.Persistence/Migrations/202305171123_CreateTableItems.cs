namespace VSGBulgariaMarketplace.Persistence.Migrations
{
    using FluentMigrator;

    using VSGBulgariaMarketplace.Persistence.Migrations.Abstraction;

    [Migration(202305171123)]
    public class CreateTableItems : CreateTableMigration
    {
        protected override string TableName => "Items";

        public override void Up()
        {
            Create.Table(this.TableName)
                   .WithColumn("Id").AsInt32().PrimaryKey()
                   .WithColumn("Name").AsCustom("NVARCHAR(150)").NotNullable()
                   .WithColumn("ImagePublicId").AsString(20).ForeignKey("CloudinaryImages", "Id").Nullable()
                   .WithColumn("Price").AsCustom("MONEY").NotNullable()
                   .WithColumn("Category").AsInt32().NotNullable()
                   .WithColumn("QuantityCombined").AsInt16().NotNullable()
                   .WithColumn("QuantityForSale").AsInt16().Nullable()
                   .WithColumn("Description").AsCustom("NVARCHAR(1000)").Nullable()
                   .WithColumn("CreatedAtUtc").AsDateTime2().NotNullable().WithDefaultValue(SystemMethods.CurrentUTCDateTime)
                   .WithColumn("ModifiedAtUtc").AsDateTime2().NotNullable().WithDefaultValue(SystemMethods.CurrentUTCDateTime)
                   .WithColumn("DeletedAtUtc").AsDateTime2().Nullable().WithDefaultValue(null)
                   .WithColumn("IsDeleted").AsBoolean().NotNullable().WithDefaultValue(false);

            Execute.Sql($"ALTER TABLE {TableName} ADD CONSTRAINT CK_Item_QuantityCombined CHECK (QuantityCombined >= 0)");
            Execute.Sql($"ALTER TABLE {TableName} ADD CONSTRAINT CK_Item_QuantityForSale CHECK (QuantityForSale >= 0 AND QuantityForSale <= QuantityCombined)");
        }
    }
}
