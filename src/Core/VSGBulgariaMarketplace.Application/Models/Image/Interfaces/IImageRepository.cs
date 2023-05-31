namespace VSGBulgariaMarketplace.Application.Models.Image.Interfaces
{
    using VSGBulgariaMarketplace.Application.Models.Repositories;
    using VSGBulgariaMarketplace.Domain.Entities;

    public interface IImageRepository : IRepository<CloudinaryImage, string>
    {
        CloudinaryImage GetImageBuildUrlInfoByItemCode(int itemCode);

        void UpdateImageFileInfo(string publicId, CloudinaryImage image);
    }
}
