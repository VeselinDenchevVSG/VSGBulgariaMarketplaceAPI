namespace VSGBulgariaMarketplace.Application.Services.HelpServices.Image.Interfaces
{
    using Microsoft.AspNetCore.Http;

    public interface IImageCloudService
    {
        public Task<bool> ExistsAsync(string publicId);

        public Task<string> UploadAsync(IFormFile imageFile);

        public Task UpdateAsync(string publicId, IFormFile newimageFile);

        public Task DeleteAsync(string publicId);
    }
}
