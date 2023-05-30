namespace VSGBulgariaMarketplace.Application.Models.Image.Interfaces
{
    using VSGBulgariaMarketplace.Application.Models.Repositories;
    using VSGBulgariaMarketplace.Domain.Entities;

    public interface IImageRepository : IRepository<CloudinaryImage, string>
    {
        CloudinaryImage GetImagePublicIdAndFileExtensionByItemCode(int itemCode);

        string GetImageFileExtension(string publicId);

        void UpdateFileExtension(string publicId, string newSecureUrl);
    }
}
