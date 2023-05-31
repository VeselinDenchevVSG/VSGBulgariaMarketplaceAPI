namespace VSGBulgariaMarketplace.Persistence.Migrations
{
    using FluentMigrator;

    [Migration(202331051556)]
    public class AddVersionColumnToCloudinaryImagesTable : Migration
    {
        private const string CLOUDINARY_IMAGES_TABLE_NAME = "CloudinaryImages";
        private const string VERSION_COLUMN_NAME = "Version";

        public override void Up()
        {
            Create.Column(VERSION_COLUMN_NAME).OnTable(CLOUDINARY_IMAGES_TABLE_NAME).AsInt32().Unique().NotNullable();
        }

        public override void Down()
        {
            Delete.UniqueConstraint().FromTable(CLOUDINARY_IMAGES_TABLE_NAME).Column(VERSION_COLUMN_NAME);
            Delete.Column(VERSION_COLUMN_NAME).FromTable(CLOUDINARY_IMAGES_TABLE_NAME);
        }

    }
}