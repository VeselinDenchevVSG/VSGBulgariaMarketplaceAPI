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
            base.columnNamesString = "(Id, FileExtension, CreatedAtUtc, ModifiedAtUtc)";
            base.SetUpRepository();
        }

        public CloudinaryImage GetImagePublicIdAndFileExtensionByItemCode(int itemCode)
        {
            string sql =    $"SELECT ci.FileExtension, ci.Id, i.ImagePublicId AS ImageId FROM CloudinaryImages AS ci " +
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
            string sql = "SELECT FileExtension FROM CloudinaryImages WHERE Id = @PublicId";
            string fileExtension = base.DbConnection.ExecuteScalar<string>(sql, new { PublicId = publicId }, transaction: base.Transaction);

            return fileExtension;
        }

        public void UpdateFileExtension(string publicId, string newFileExtension)
        {
            string sql = $"UPDATE CloudinaryImages SET FileExtension = @NewFileExtension WHERE Id = @PublicId";
            base.DbConnection.Execute(sql, new { PublicId = publicId, NewFileExtension = newFileExtension }, transaction: this.Transaction);
        }
    }
}
