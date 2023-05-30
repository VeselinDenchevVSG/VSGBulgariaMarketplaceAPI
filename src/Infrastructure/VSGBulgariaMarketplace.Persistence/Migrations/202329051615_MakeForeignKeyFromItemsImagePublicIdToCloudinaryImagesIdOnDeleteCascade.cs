namespace VSGBulgariaMarketplace.Persistence.Migrations
{
    using FluentMigrator;

    using System.Data;

    [Migration(202329051615)]
    public class MakeForeignKeyFromItemsImagePublicIdToCloudinaryImagesIdOnDeleteSetNull : Migration
    {
        private const string ITEMS_TABLE_NAME = "Items";
        private const string CLOUDINARY_IMAGES_TABLE_NAME = "CloudinaryImages";

        private const string ID_COLUMN_NAME = "Id";
        private const string IMAGE_PUBLIC_ID_COLUMN_NAME = "ImagePublicId";

        private const string FOREIGN_KEY_ITEMS_IMAGE_PUBLIC_ID_CLOUDINARY_IMAGES_ID_NAME = "FK_Items_ImagePublicId_CloudinaryImages_Id";

        public override void Up()
        {
            Delete.ForeignKey(FOREIGN_KEY_ITEMS_IMAGE_PUBLIC_ID_CLOUDINARY_IMAGES_ID_NAME).OnTable(ITEMS_TABLE_NAME);

            Create.ForeignKey().FromTable(ITEMS_TABLE_NAME).ForeignColumn(IMAGE_PUBLIC_ID_COLUMN_NAME)
                    .ToTable(CLOUDINARY_IMAGES_TABLE_NAME).PrimaryColumn(ID_COLUMN_NAME)
                    .OnDelete(Rule.SetNull);
        }

        public override void Down()
        {
            Delete.ForeignKey(FOREIGN_KEY_ITEMS_IMAGE_PUBLIC_ID_CLOUDINARY_IMAGES_ID_NAME).OnTable(ITEMS_TABLE_NAME);

            Create.ForeignKey(FOREIGN_KEY_ITEMS_IMAGE_PUBLIC_ID_CLOUDINARY_IMAGES_ID_NAME).FromTable(ITEMS_TABLE_NAME).ForeignColumn(IMAGE_PUBLIC_ID_COLUMN_NAME)
                                                                            .ToTable(CLOUDINARY_IMAGES_TABLE_NAME).PrimaryColumn(ID_COLUMN_NAME);
        }
    }
}
