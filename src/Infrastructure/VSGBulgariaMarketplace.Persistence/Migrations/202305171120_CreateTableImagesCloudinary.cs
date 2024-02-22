namespace VSGBulgariaMarketplace.Persistence.Migrations
{
    using FluentMigrator;

    using VSGBulgariaMarketplace.Persistence.Migrations.Abstraction;
    
    using static VSGBulgariaMarketplace.Persistence.Constants.DatabaseConstant;

    [Migration(CREATE_TABLE_CLOUDINARY_IMAGES_MIGRATION_VERSION)]
    public class CreateTableCloudinaryImages : CreateTableMigration
    {
        protected override string TableName => CLOUDINARY_IMAGES_TABLE_NAME;

        public override void Up()
        {
            Create.Table(this.TableName).WithColumn(ID_COLUMN_NAME).AsString(INT_ID_COLUMN_SIZE).PrimaryKey()
                                        .WithColumn(SECURE_URL_COLUMN_NAME).AsString(SECURE_URL_COLUMN_SIZE).Unique().NotNullable()
                                        .WithColumn(CREATED_AT_UTC_COLUMN_NAME).AsDateTime2().NotNullable().WithDefaultValue(SystemMethods.CurrentUTCDateTime)
                                        .WithColumn(MODIFIED_AT_UTC_COLUMN_NAME).AsDateTime2().NotNullable().WithDefaultValue(SystemMethods.CurrentUTCDateTime)
                                        .WithColumn(DELETED_AT_UTC_COLUMN_NAME).AsDateTime2().Nullable().WithDefaultValue(null)
                                        .WithColumn(IS_DELETED_COLUMN_NAME).AsBoolean().NotNullable().WithDefaultValue(false);
        }
    }
}