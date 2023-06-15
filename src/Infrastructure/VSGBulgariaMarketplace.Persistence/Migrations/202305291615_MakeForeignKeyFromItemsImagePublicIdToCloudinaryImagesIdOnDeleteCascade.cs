namespace VSGBulgariaMarketplace.Persistence.Migrations
{
    using FluentMigrator;

    using System.Data;

    using VSGBulgariaMarketplace.Persistence.Constants;

    [Migration(DatabaseConstant.MAKE_FOREIGN_KEY_FROM_ITEMS_IMAGE_PUBLIC_ID_TO_CLOUDINARY_IMAGES_ID_ON_DELETE_SET_NULL_MIGRATION_VERSION)]
    public class MakeForeignKeyFromItemsImagePublicIdToCloudinaryImagesIdOnDeleteSetNull : Migration
    {
        public override void Up()
        {
            Delete.ForeignKey(DatabaseConstant.FOREIGN_KEY_ITEMS_IMAGE_PUBLIC_ID_CLOUDINARY_IMAGES_ID_NAME).OnTable(DatabaseConstant.ITEMS_TABLE_NAME);

            Create.ForeignKey().FromTable(DatabaseConstant.ITEMS_TABLE_NAME).ForeignColumn(DatabaseConstant.IMAGE_PUBLIC_ID_COLUMN_NAME)
                    .ToTable(DatabaseConstant.CLOUDINARY_IMAGES_TABLE_NAME).PrimaryColumn(DatabaseConstant.ID_COLUMN_NAME)
                    .OnDelete(Rule.SetNull);
        }

        public override void Down()
        {
            Delete.ForeignKey(DatabaseConstant.FOREIGN_KEY_ITEMS_IMAGE_PUBLIC_ID_CLOUDINARY_IMAGES_ID_NAME).OnTable(DatabaseConstant.ITEMS_TABLE_NAME);

            Create.ForeignKey(DatabaseConstant.FOREIGN_KEY_ITEMS_IMAGE_PUBLIC_ID_CLOUDINARY_IMAGES_ID_NAME).FromTable(DatabaseConstant.ITEMS_TABLE_NAME)
                                                                                                                .ForeignColumn(DatabaseConstant.IMAGE_PUBLIC_ID_COLUMN_NAME)
                                                                                                                    .ToTable(DatabaseConstant.CLOUDINARY_IMAGES_TABLE_NAME)
                                                                                                                        .PrimaryColumn(DatabaseConstant.ID_COLUMN_NAME);
        }
    }
}