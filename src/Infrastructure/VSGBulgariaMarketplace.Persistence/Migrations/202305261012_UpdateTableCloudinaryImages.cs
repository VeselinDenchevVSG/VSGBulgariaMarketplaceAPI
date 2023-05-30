namespace VSGBulgariaMarketplace.Persistence.Migrations
{
    using FluentMigrator;
    using FluentMigrator.Builders.Delete.Constraint;

    [Migration(202305261012)]
    public class UpdateTableCloudinaryImages : Migration
    {
        private const string CLOUDINARY_IMAGES_TABLE_NAME = "CloudinaryImages";

        private const string SECURE_URL_COLUMN_NAME = "SecureUrl";
        private const string DELETED_AT_UTC_COLUMN_NAME = "DeletedAtUtc";
        private const string IS_DELETED_COLUMN_NAME = "IsDeleted";
        private const string FILE_EXTENSION_COLUMN_NAME = "FileExtension";

        public override void Up()
        {
            Delete.Index().OnTable(CLOUDINARY_IMAGES_TABLE_NAME).OnColumn(SECURE_URL_COLUMN_NAME);
            Delete.Column(SECURE_URL_COLUMN_NAME).FromTable(CLOUDINARY_IMAGES_TABLE_NAME);

            Delete.DefaultConstraint().OnTable(CLOUDINARY_IMAGES_TABLE_NAME).OnColumn(DELETED_AT_UTC_COLUMN_NAME);
            Delete.Column(DELETED_AT_UTC_COLUMN_NAME).FromTable(CLOUDINARY_IMAGES_TABLE_NAME);
            Delete.DefaultConstraint().OnTable(CLOUDINARY_IMAGES_TABLE_NAME).OnColumn(IS_DELETED_COLUMN_NAME);
            Delete.Column(IS_DELETED_COLUMN_NAME).FromTable(CLOUDINARY_IMAGES_TABLE_NAME);

            Create.Column(FILE_EXTENSION_COLUMN_NAME).OnTable(CLOUDINARY_IMAGES_TABLE_NAME).AsAnsiString(5).NotNullable();
        }

        public override void Down()
        {
            Create.Column(SECURE_URL_COLUMN_NAME).OnTable(CLOUDINARY_IMAGES_TABLE_NAME).AsString(150).Unique().NotNullable();
            Create.Column(DELETED_AT_UTC_COLUMN_NAME).OnTable(CLOUDINARY_IMAGES_TABLE_NAME).AsDateTime2().Nullable().WithDefaultValue(null);
            Create.Column(IS_DELETED_COLUMN_NAME).OnTable(CLOUDINARY_IMAGES_TABLE_NAME).AsBoolean().NotNullable().WithDefaultValue(false);

            Delete.Column(FILE_EXTENSION_COLUMN_NAME).FromTable(CLOUDINARY_IMAGES_TABLE_NAME);
        }
    }
}