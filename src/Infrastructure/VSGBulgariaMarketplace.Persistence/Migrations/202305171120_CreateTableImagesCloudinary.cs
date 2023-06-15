namespace VSGBulgariaMarketplace.Persistence.Migrations
{
    using FluentMigrator;

    using VSGBulgariaMarketplace.Persistence.Constants;
    using VSGBulgariaMarketplace.Persistence.Migrations.Abstraction;

    [Migration(DatabaseConstant.CREATE_TABLE_CLOUDINARY_IMAGES_MIGRATION_VERSION)]
    public class CreateTableCloudinaryImages : CreateTableMigration
    {
        protected override string TableName => DatabaseConstant.CLOUDINARY_IMAGES_TABLE_NAME;

        public override void Up()
        {
            Create.Table(this.TableName).WithColumn(DatabaseConstant.ID_COLUMN_NAME).AsString(DatabaseConstant.INT_ID_COLUMN_SIZE).PrimaryKey()
                                        .WithColumn(DatabaseConstant.SECURE_URL_COLUMN_NAME).AsString(DatabaseConstant.SECURE_URL_COLUMN_SIZE).Unique().NotNullable()
                                        .WithColumn(DatabaseConstant.CREATED_AT_UTC_COLUMN_NAME).AsDateTime2().NotNullable()
                                                                                                                    .WithDefaultValue(SystemMethods.CurrentUTCDateTime)
                                        .WithColumn(DatabaseConstant.MODIFIED_AT_UTC_COLUMN_NAME).AsDateTime2().NotNullable()
                                                                                                                    .WithDefaultValue(SystemMethods.CurrentUTCDateTime)
                                        .WithColumn(DatabaseConstant.DELETED_AT_UTC_COLUMN_NAME).AsDateTime2().Nullable().WithDefaultValue(null)
                                        .WithColumn(DatabaseConstant.IS_DELETED_COLUMN_NAME).AsBoolean().NotNullable().WithDefaultValue(false);
        }
    }
}