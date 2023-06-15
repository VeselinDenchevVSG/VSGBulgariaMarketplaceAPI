namespace VSGBulgariaMarketplace.Persistence.Migrations
{
    using FluentMigrator;

    using VSGBulgariaMarketplace.Persistence.Constants;

    [Migration(DatabaseConstant.ADD_VERSION_COLUMN_TO_CLOUDINARY_IMAGES_TABLE_MIGRATION_VERSION)]
    public class AddVersionColumnToCloudinaryImagesTable : Migration
    {
        public override void Up()
        {
            Create.Column(DatabaseConstant.VERSION_COLUMN_NAME).OnTable(DatabaseConstant.CLOUDINARY_IMAGES_TABLE_NAME).AsInt32().Unique().NotNullable();
        }

        public override void Down()
        {
            Delete.UniqueConstraint().FromTable(DatabaseConstant.CLOUDINARY_IMAGES_TABLE_NAME).Column(DatabaseConstant.VERSION_COLUMN_NAME);
            Delete.Column(DatabaseConstant.VERSION_COLUMN_NAME).FromTable(DatabaseConstant.CLOUDINARY_IMAGES_TABLE_NAME);
        }

    }
}