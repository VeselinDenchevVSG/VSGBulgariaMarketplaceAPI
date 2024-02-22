namespace VSGBulgariaMarketplace.Persistence.Repositories
{
    using Dapper;

    using VSGBulgariaMarketplace.Application.Models.Image.Interfaces;
    using VSGBulgariaMarketplace.Application.Models.UnitOfWork;
    using VSGBulgariaMarketplace.Domain.Entities;

    using static VSGBulgariaMarketplace.Persistence.Constants.RepositoryConstant;

    public class ImageRepository : Repository<CloudinaryImage, string>, IImageRepository
    {
        public ImageRepository(IUnitOfWork unitOfWork) 
            : base(unitOfWork)
        {
            base.columnNamesString = IMAGE_REPOSITORY_COLUMN_NAMES_STRING;
            base.SetUpRepository();
        }

        public CloudinaryImage GetImageBuildUrlInfoByItemId(string itemId)
        {
            string sql = GET_IMAGE_BUILD_URL_INFO_BY_ITEM_ID_SQL_QUERY;
            CloudinaryImage image = base.DbConnection.Query<CloudinaryImage, Item, CloudinaryImage>(sql, (image, item) =>
            {
                return image;
            }, new { ItemId = itemId }, splitOn: CLOUDINARY_IMAGE_ID_ALIAS, transaction: base.Transaction).FirstOrDefault();

            return image;
        }

        public void UpdateImageFileInfo(string publicId, CloudinaryImage image)
        {
            string sql = UPDATE_IMAGE_FILE_INFO_SQL_QUERY;
            base.DbConnection.Execute(sql, new { PublicId = publicId, FileExtension = image.FileExtension, Version = image.Version }, transaction: this.Transaction);
        }
    }
}
