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
            base.columnNamesString = "(Id, SecureUrl, CreatedAtUtc, ModifiedAtUtc, DeletedAtUtc, IsDeleted)";
            base.SetUpRepository();
        }

        public void Update(string publicId, string newSecureUrl)
        {
            string sql = $"UPDATE Items SET SecureUrl = @NewSecureUrl WHERE Id = @PublicId";

            base.DbConnection.Execute(sql, new { PublicId = publicId, NewSecureUrl = newSecureUrl }, transaction: this.Transaction);
        }
    }
}
