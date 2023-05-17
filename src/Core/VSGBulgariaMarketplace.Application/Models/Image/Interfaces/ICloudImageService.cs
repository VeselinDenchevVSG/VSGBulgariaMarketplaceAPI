namespace VSGBulgariaMarketplace.Application.Models.Image.Interfaces
{
    using Microsoft.AspNetCore.Http;

    public interface ICloudImageService
    {
        public Task<bool> ExistsAsync(string publicId);

        public Task<string> UploadAsync(IFormFile imageFile);

        public Task UpdateAsync(string publicId, IFormFile newimageFile);

        public Task DeleteAsync(string publicId);
    }
}
