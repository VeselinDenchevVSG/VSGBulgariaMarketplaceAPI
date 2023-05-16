namespace VSGBulgariaMarketplace.Persistence.Migrations
{
    using FluentMigrator;

    using VSGBulgariaMarketplace.Persistence.Migrations.Abstraction;

    [Migration(202305041729)]
    public class CreateTableLogs : CreateTableMigration
    {
        protected override string TableName => "[Logs]";

        public override void Up()
        {
            Create.Table(this.TableName)
                   .WithColumn("[Id]").AsInt32().PrimaryKey().Identity()
                   .WithColumn("[Level]").AsString(11).NotNullable()
                   .WithColumn("[CallSite]").AsString(int.MaxValue).NotNullable()
                   .WithColumn("[Type]").AsString(int.MaxValue).NotNullable()
                   .WithColumn("[Message]").AsString(int.MaxValue).NotNullable()
                   .WithColumn("[StackTrace]").AsString(int.MaxValue).NotNullable()
                   .WithColumn("[InnerException]").AsString(int.MaxValue).NotNullable()
                   .WithColumn("[AdditionalInfo]").AsString(int.MaxValue).NotNullable()
                   .WithColumn("[LoggedOnDatetimeUtc]").AsCustom("DATETIME2(7)").NotNullable().WithDefaultValue(DateTime.UtcNow);
        }
    }
}
