namespace VSGBulgariaMarketplace.Persistence.Migrations
{
    using FluentMigrator;

    using VSGBulgariaMarketplace.Persistence.Constants;

    [Migration(DatabaseConstant.UPDATE_TABLE_CLOUDINARY_IMAGES_MIGRATION_VERSION)]
    public class UpdateTableCloudinaryImages : Migration
    {
        public override void Up()
        {
            Delete.Index().OnTable(DatabaseConstant.CLOUDINARY_IMAGES_TABLE_NAME).OnColumn(DatabaseConstant.SECURE_URL_COLUMN_NAME);
            Delete.Column(DatabaseConstant.SECURE_URL_COLUMN_NAME).FromTable(DatabaseConstant.CLOUDINARY_IMAGES_TABLE_NAME);

            Delete.DefaultConstraint().OnTable(DatabaseConstant.CLOUDINARY_IMAGES_TABLE_NAME).OnColumn(DatabaseConstant.DELETED_AT_UTC_COLUMN_NAME);
            Delete.Column(DatabaseConstant.DELETED_AT_UTC_COLUMN_NAME).FromTable(DatabaseConstant.CLOUDINARY_IMAGES_TABLE_NAME);
            Delete.DefaultConstraint().OnTable(DatabaseConstant.CLOUDINARY_IMAGES_TABLE_NAME).OnColumn(DatabaseConstant.IS_DELETED_COLUMN_NAME);
            Delete.Column(DatabaseConstant.IS_DELETED_COLUMN_NAME).FromTable(DatabaseConstant.CLOUDINARY_IMAGES_TABLE_NAME);

            Create.Column(DatabaseConstant.FILE_EXTENSION_COLUMN_NAME).OnTable(DatabaseConstant.CLOUDINARY_IMAGES_TABLE_NAME)
                                                                                    .AsAnsiString(DatabaseConstant.FILE_EXTENSION_COLUMN_SIZE).NotNullable();
        }

        public override void Down()
        {
            Create.Column(DatabaseConstant.SECURE_URL_COLUMN_NAME).OnTable(DatabaseConstant.CLOUDINARY_IMAGES_TABLE_NAME)
                                                                                .AsString(DatabaseConstant.SECURE_URL_COLUMN_SIZE).Unique().NotNullable();
            Create.Column(DatabaseConstant.DELETED_AT_UTC_COLUMN_NAME).OnTable(DatabaseConstant.CLOUDINARY_IMAGES_TABLE_NAME).AsDateTime2().Nullable()
                                                                                                                                                .WithDefaultValue(null);
            Create.Column(DatabaseConstant.IS_DELETED_COLUMN_NAME).OnTable(DatabaseConstant.CLOUDINARY_IMAGES_TABLE_NAME).AsBoolean().NotNullable()
                                                                                                                                                        .WithDefaultValue(false);

            Delete.Column(DatabaseConstant.FILE_EXTENSION_COLUMN_NAME).FromTable(DatabaseConstant.CLOUDINARY_IMAGES_TABLE_NAME);
        }
    }
}