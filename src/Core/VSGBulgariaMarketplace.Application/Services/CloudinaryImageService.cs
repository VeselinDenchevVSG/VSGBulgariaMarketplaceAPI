namespace VSGBulgariaMarketplace.Application.Services
{
    using AutoMapper;

    using CloudinaryDotNet;
    using CloudinaryDotNet.Actions;

    using Microsoft.AspNetCore.Http;
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Configuration;

    using System;
    using System.Threading.Tasks;

    using VSGBulgariaMarketplace.Application.Models.Image.Interfaces;
    using VSGBulgariaMarketplace.Domain.Entities;

    public class CloudinaryImageService : ICloudImageService
    {
        private IImageRepository imageRepository;
        private IMapper mapper;

        private Account cloudinaryAccount;
        private Cloudinary cloudinary;

        public CloudinaryImageService(IImageRepository imageRepository, IMapper mapper, IConfiguration configuration)
        {
            this.imageRepository = imageRepository;
            this.mapper = mapper;

            // Set Cloudinary account
            string cloudinaryUrl = configuration["Cloudinary:Cloud"];
            string cloudinaryApiKey = configuration["Cloudinary:ApiKey"];
            string cloudinaryApiSecret = configuration["Cloudinary:ApiSecret"];

            cloudinaryAccount = new Account(cloudinaryUrl, cloudinaryApiKey, cloudinaryApiSecret);
            cloudinary = new Cloudinary(cloudinaryAccount);
            cloudinary.Api.Secure = true;
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
            using Stream stream = ConvertIFormFileToStream(imageFile);

            string uniqueFileName = GenerateUniqueFileName();

            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(uniqueFileName, stream),
                Folder = "VSG_Marketplace"
            };
            ImageUploadResult uploadResult = await cloudinary.UploadAsync(uploadParams);
            if (uploadResult.Error != null) throw new InvalidOperationException("Failed to upload file: " + uploadResult.Error.Message);

            CloudinaryImage image = mapper.Map<ImageUploadResult, CloudinaryImage>(uploadResult);

            try
            {
                this.imageRepository.Create(image);
            }
            catch (SqlException se)
            {
                await DeleteAsync(uploadResult.PublicId);

                throw se;
            }

            return image.Id;
        }

        public async Task UpdateAsync(string publicId, IFormFile newimageFile)
        {
            publicId = publicId.Replace("%2F", "/");

            bool exists = await ExistsAsync(publicId);
            if (exists)
            {
                using Stream stream = ConvertIFormFileToStream(newimageFile);
                string uniqueFileName = GenerateUniqueFileName();

                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(uniqueFileName, stream),
                    PublicId = publicId, // Use the publicId of the existing file
                    Overwrite = true // Overwrite the existing file
                };

                ImageUploadResult uploadResult = await cloudinary.UploadAsync(uploadParams);

                CloudinaryImage image = mapper.Map<ImageUploadResult, CloudinaryImage>(uploadResult);
                imageRepository.Update(publicId, image.SecureUrl);

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

            if (deletionResult.Result == "not found") throw new FileNotFoundException("Image not found!");

            imageRepository.Delete(publicId);
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