namespace VSGBulgariaMarketplace.Persistence.Repositories
{
    using Dapper;

    using VSGBulgariaMarketplace.Application.Models.Image.Interfaces;
    using VSGBulgariaMarketplace.Application.Models.UnitOfWork;
    using VSGBulgariaMarketplace.Domain.Entities;

    public class ImageRepository : Repository<CloudinaryImage, string>, IImageRepository
    {
        public ImageRepository(IUnitOfWork unitOfWork) 
            : base(unitOfWork)
        {
            base.columnNamesString = "(Id, FileExtension, Version, CreatedAtUtc, ModifiedAtUtc)";
            base.SetUpRepository();
        }

        public CloudinaryImage GetImageBuildUrlInfoByItemCode(int itemCode)
        {
            string sql =    $"SELECT ci.FileExtension, ci.Version, ci.Id, i.ImagePublicId AS ImageId FROM CloudinaryImages AS ci " +
                            $"JOIN Items AS i " +
                            $"ON ci.Id = i.ImagePublicId " +
                            $"WHERE i.Code = @ItemCode";
            CloudinaryImage image = base.DbConnection.Query<CloudinaryImage, Item, CloudinaryImage>(sql, (image, item) =>
            {
                return image;
            }, new { ItemCode = itemCode }, splitOn: "ImageId", transaction: base.Transaction).FirstOrDefault();

            return image;
        }

        public string GetImageFileExtension(string publicId)
        {
            string sql = "SELECT FileExtension, Version FROM CloudinaryImages WHERE Id = @PublicId";
            string fileExtension = base.DbConnection.ExecuteScalar<string>(sql, new { PublicId = publicId }, transaction: base.Transaction);

            return fileExtension;
        }

        public void UpdateImageFileInfo(string publicId, CloudinaryImage image)
        {
            string sql = $"UPDATE CloudinaryImages SET FileExtension = FileExtension, Version = @Version WHERE Id = @PublicId";
            base.DbConnection.Execute(sql, new { PublicId = publicId, FileExtension = image.FileExtension, Version = image.Version }, transaction: this.Transaction);
        }
    }
}
