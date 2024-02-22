namespace VSGBulgariaMarketplace.Persistence.Migrations
{
    using FluentMigrator;

    using System.Data;

    using static VSGBulgariaMarketplace.Persistence.Constants.DatabaseConstant;

    [Migration(MAKE_FOREIGN_KEY_FROM_ITEMS_IMAGE_PUBLIC_ID_TO_CLOUDINARY_IMAGES_ID_ON_DELETE_SET_NULL_MIGRATION_VERSION)]
    public class MakeForeignKeyFromItemsImagePublicIdToCloudinaryImagesIdOnDeleteSetNull : Migration
    {
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