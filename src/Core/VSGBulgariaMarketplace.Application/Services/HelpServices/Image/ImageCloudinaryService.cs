namespace VSGBulgariaMarketplace.Application.Services.HelpServices.Image
{
    using CloudinaryDotNet;
    using CloudinaryDotNet.Actions;

    using Microsoft.AspNetCore.Http;

    using System;
    using System.Threading.Tasks;

    using VSGBulgariaMarketplace.Application.Services.HelpServices.Image.Interfaces;

    public class ImageCloudinaryService : IImageCloudService
    {
        private Account cloudinaryAccount;
        private Cloudinary cloudinary;

        public ImageCloudinaryService()
        {
            string cloudinaryUrl = Environment.GetEnvironmentVariable("CLOUDINARY_CLOUD");
            string cloudinaryApiKey = Environment.GetEnvironmentVariable("CLOUDINARY_API_KEY");
            string cloudinaryApiSecret = Environment.GetEnvironmentVariable("CLOUDINARY_API_SECRET");

            this.cloudinaryAccount = new Account(cloudinaryUrl, cloudinaryApiKey, cloudinaryApiSecret);
            this.cloudinary = new Cloudinary(this.cloudinaryAccount);
            this.cloudinary.Api.Secure = true;
        }

        public async Task<bool> ExistsAsync(string publicId)
        {
            var parameters = new GetResourceParams(publicId) { ResourceType = ResourceType.Image };
            GetResourceResult result = await cloudinary.GetResourceAsync(parameters);

            bool exists = result != null && result.PublicId == publicId;

            return exists;
        }

        public async Task<string> UploadAsync(IFormFile imageFile)
        {
            using Stream stream = this.ConvertIFormFileToStream(imageFile);

            string uniqueFileName = this.GenerateUniqueFileName();

            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(uniqueFileName, stream),
                Folder = "VSG_Marketplace"
            };
            ImageUploadResult uploadResult = await cloudinary.UploadAsync(uploadParams);
            if (uploadResult.Error == null)
            {
                Console.WriteLine("File updated successfully: " + uploadResult.SecureUrl);
            }
            else throw new InvalidOperationException("Failed to upload file: " + uploadResult.Error.Message);

            return uploadResult.PublicId.Split('/').TakeLast(1).First();
        }

        public async Task UpdateAsync(string publicId, IFormFile newimageFile)
        {
            publicId = publicId.Replace("%2F", "/");

            bool exists = await this.ExistsAsync(publicId);
            if (exists)
            {
                using Stream stream = this.ConvertIFormFileToStream(newimageFile);
                string uniqueFileName = this.GenerateUniqueFileName();

                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(uniqueFileName, stream),
                    PublicId = publicId, // Use the publicId of the existing file
                    Overwrite = true // Overwrite the existing file
                };

                ImageUploadResult uploadResult = await cloudinary.UploadAsync(uploadParams);

                if (uploadResult.Error == null)
                {
                    Console.WriteLine("File updated successfully: " + uploadResult.SecureUrl);
                }
                else throw new InvalidOperationException("Failed to update file: " + uploadResult.Error.Message);
            }
            else throw new FileNotFoundException("Image not found!");
        }

        public async Task DeleteAsync(string publicId)
        {
            publicId = publicId.Replace("%2F", "/");
            var deletionParams = new DeletionParams(publicId);
            var deletionResult = await cloudinary.DestroyAsync(deletionParams);

            if (deletionResult.Result == "not found")
            {
                throw new FileNotFoundException("Image not found!");
            }
        }

        private Stream ConvertIFormFileToStream(IFormFile file)
        {
            MemoryStream stream = new MemoryStream();
            file.CopyTo(stream);
            stream.Position = 0; // Reset the stream position to the beginning

            return stream;
        }

        private string GenerateUniqueFileName()
        {
            string uniqueFileName = Guid.NewGuid().ToString().Substring(0, 8);

            return uniqueFileName;
        }
    }
}
