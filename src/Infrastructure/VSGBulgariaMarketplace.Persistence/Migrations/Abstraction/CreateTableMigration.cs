namespace VSGBulgariaMarketplace.Persistence.Migrations.Abstraction
{
    using FluentMigrator;

    public abstract class CreateTableMigration : Migration
    {
        protected abstract string TableName { get; }

        public override void Down() => Delete.Table(this.TableName);
    }
}
