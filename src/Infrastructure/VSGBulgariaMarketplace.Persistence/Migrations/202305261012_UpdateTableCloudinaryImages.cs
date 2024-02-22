namespace VSGBulgariaMarketplace.Persistence.Migrations
{
    using FluentMigrator;

    using static VSGBulgariaMarketplace.Persistence.Constants.DatabaseConstant;

    [Migration(UPDATE_TABLE_CLOUDINARY_IMAGES_MIGRATION_VERSION)]
    public class UpdateTableCloudinaryImages : Migration
    {
        public override void Up()
        {
            Delete.Index().OnTable(CLOUDINARY_IMAGES_TABLE_NAME).OnColumn(SECURE_URL_COLUMN_NAME);
            Delete.Column(SECURE_URL_COLUMN_NAME).FromTable(CLOUDINARY_IMAGES_TABLE_NAME);

            Delete.DefaultConstraint().OnTable(CLOUDINARY_IMAGES_TABLE_NAME).OnColumn(DELETED_AT_UTC_COLUMN_NAME);
            Delete.Column(DELETED_AT_UTC_COLUMN_NAME).FromTable(CLOUDINARY_IMAGES_TABLE_NAME);
            Delete.DefaultConstraint().OnTable(CLOUDINARY_IMAGES_TABLE_NAME).OnColumn(IS_DELETED_COLUMN_NAME);
            Delete.Column(IS_DELETED_COLUMN_NAME).FromTable(CLOUDINARY_IMAGES_TABLE_NAME);

            Create.Column(FILE_EXTENSION_COLUMN_NAME).OnTable(CLOUDINARY_IMAGES_TABLE_NAME).AsAnsiString(FILE_EXTENSION_COLUMN_SIZE).NotNullable();
        }

        public override void Down()
        {
            Create.Column(SECURE_URL_COLUMN_NAME).OnTable(CLOUDINARY_IMAGES_TABLE_NAME).AsString(SECURE_URL_COLUMN_SIZE).Unique().NotNullable();
            Create.Column(DELETED_AT_UTC_COLUMN_NAME).OnTable(CLOUDINARY_IMAGES_TABLE_NAME).AsDateTime2().Nullable().WithDefaultValue(null);
            Create.Column(IS_DELETED_COLUMN_NAME).OnTable(CLOUDINARY_IMAGES_TABLE_NAME).AsBoolean().NotNullable().WithDefaultValue(false);

            Delete.Column(FILE_EXTENSION_COLUMN_NAME).FromTable(CLOUDINARY_IMAGES_TABLE_NAME);
        }
    }
}