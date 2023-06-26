namespace VSGBulgariaMarketplace.Application.Models.Image.Interfaces
{
    using Microsoft.AspNetCore.Http;

    public interface ICloudImageService
    {
        public Task<bool> ExistsAsync(string publicId, CancellationToken cancellationToken);

        public Task<string> UploadAsync(IFormFile imageFile, CancellationToken cancellationToken);

        public Task UpdateAsync(string publicId, IFormFile newimageFile, CancellationToken cancellationToken);

        public Task<string> DeleteAsync(string publicId);

        public string GetImageUrlByItemId(string itemId);
    }
}
