namespace VSGBulgariaMarketplace.Persistence.Migrations
{
    using FluentMigrator;

    using VSGBulgariaMarketplace.Persistence.Migrations.Abstraction;

    [Migration(202305171120)]
    public class CreateTableCloudinaryImages : CreateTableMigration
    {
        protected override string TableName => "[CloudinaryImages]";

        public override void Up()
        {
            Create.Table(this.TableName).WithColumn("[Id]").AsString(20).PrimaryKey()
                                        .WithColumn("[SecureUrl]").AsString(150).Unique().NotNullable()
                                        .WithColumn("[CreatedAtUtc]").AsDateTime2().NotNullable().WithDefaultValue(SystemMethods.CurrentUTCDateTime)
                                        .WithColumn("[ModifiedAtUtc]").AsDateTime2().NotNullable().WithDefaultValue(SystemMethods.CurrentUTCDateTime)
                                        .WithColumn("[DeletedAtUtc]").AsDateTime2().Nullable().WithDefaultValue(null)
                                        .WithColumn("[IsDeleted]").AsBoolean().NotNullable().WithDefaultValue(false);
        }
    }
}
