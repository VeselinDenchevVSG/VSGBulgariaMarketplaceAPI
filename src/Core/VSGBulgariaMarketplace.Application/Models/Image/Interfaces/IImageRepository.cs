namespace VSGBulgariaMarketplace.Application.Models.Image.Interfaces
{
    using VSGBulgariaMarketplace.Application.Models.Repositories;
    using VSGBulgariaMarketplace.Domain.Entities;

    public interface IImageRepository : IRepository<CloudinaryImage, string>
    {
        void Update(string publicId, string newSecureUrl);
    }
}
