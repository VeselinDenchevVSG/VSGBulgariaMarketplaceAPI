namespace VSGBulgariaMarketplace.Persistence.Migrations
{
    using FluentMigrator;

    using static VSGBulgariaMarketplace.Persistence.Constants.DatabaseConstant;

    [Migration(ADD_VERSION_COLUMN_TO_CLOUDINARY_IMAGES_TABLE_MIGRATION_VERSION)]
    public class AddVersionColumnToCloudinaryImagesTable : Migration
    {
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