namespace VSGBulgariaMarketplace.Application.Models.Image.Interfaces
{
    using VSGBulgariaMarketplace.Application.Models.Repositories;
    using VSGBulgariaMarketplace.Domain.Entities;

    public interface IImageRepository : IRepository<CloudinaryImage, string>
    {
        CloudinaryImage GetImageBuildUrlInfoByItemId(string itemId);

        void UpdateImageFileInfo(string publicId, CloudinaryImage image);
    }
}
