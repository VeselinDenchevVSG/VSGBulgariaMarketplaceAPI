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

            this.cloudinaryAccount = new Account(cloudinaryUrl, cloudinaryApiKey, cloudinaryApiSecret);
            this.cloudinary = new Cloudinary(cloudinaryAccount);
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
            this.UpdateImageFileMetadata(image, uploadResult.Format, uploadResult.Version);

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
                    Overwrite = true, // Overwrite the existing file
                    Invalidate = true,
                };

                ImageUploadResult uploadResult = await cloudinary.UploadAsync(uploadParams);
                if (uploadResult.Error != null) throw new InvalidOperationException("Failed to update file: " + uploadResult.Error.Message);

                CloudinaryImage image = mapper.Map<ImageUploadResult, CloudinaryImage>(uploadResult);
                this.UpdateImageFileMetadata(image, uploadResult.Format, uploadResult.Version);

                publicId = publicId.Split('/')[1];

                this.imageRepository.UpdateImageFileInfo(publicId, image);
                
            }
            else throw new FileNotFoundException("Image not found!");
        }

        public async Task DeleteAsync(string publicId)
        {
            publicId = publicId.Replace("%2F", "/");
            var deletionParams = new DeletionParams(publicId);
            var deletionResult = await cloudinary.DestroyAsync(deletionParams);

            if (deletionResult.Result == "not found") throw new FileNotFoundException("Image not found!");

            publicId = publicId.Split('/')[1];
            this.imageRepository.DeleteById(publicId);
        }

        public string GetImageUrlByItemId(string itemId)
        {
            CloudinaryImage image = this.imageRepository.GetImageBuildUrlInfoByItemId(itemId);
            if (image is not null)
            {
                if (image.FileExtension is null) throw new FileNotFoundException("Image not found!");
                else
                {
                    string imageUrl = this.cloudinary.Api.UrlImgUp.BuildUrl($"v{image.Version}/VSG_Marketplace/{image.Id}.{image.FileExtension}");

                    return imageUrl;
                }
            }

            return null;
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

        private void UpdateImageFileMetadata(CloudinaryImage image, string fileExtension, string version)
        {
            image.FileExtension = fileExtension;
            image.Version = int.Parse(version);
        }
    }
}