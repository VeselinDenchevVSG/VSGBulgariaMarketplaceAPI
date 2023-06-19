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

    using VSGBulgariaMarketplace.Application.Constants;
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
            string cloudinaryUrl = configuration[ServiceConstant.CLOUDINARY_CONFIGURATION_CLOUD];
            string cloudinaryApiKey = configuration[ServiceConstant.CLOUDINARY_CONFIGURATION_API_KEY];
            string cloudinaryApiSecret = configuration[ServiceConstant.CLOUDINARY_CONFIGURATION_API_SECRET];

            this.cloudinaryAccount = new Account(cloudinaryUrl, cloudinaryApiKey, cloudinaryApiSecret);
            this.cloudinary = new Cloudinary(this.cloudinaryAccount);
            this.cloudinary.Api.Secure = true;
        }

        public async Task<bool> ExistsAsync(string publicId)
        {
            var parameters = new GetResourceParams(publicId) { ResourceType = ResourceType.Image };
            GetResourceResult result = await this.cloudinary.GetResourceAsync(parameters);

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
                Folder = ServiceConstant.CLOUDINARY_VSG_MARKETPLACE_IMAGES_FOLDER_NAME
            };
            ImageUploadResult uploadResult = await cloudinary.UploadAsync(uploadParams);
            if (uploadResult.Error != null) throw new InvalidOperationException(string.Format(ServiceConstant.FAILED_TO_UPLOAD_FILE_ERROR_MESSAGE_TEMPLATE, 
                                                                                                        uploadResult.Error.Message));

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

        public async Task<string> UpdateAsync(string publicId, IFormFile newimageFile)
        {
            publicId = publicId.Replace(ServiceConstant.SLASH_URL_ENCODING, "/");

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
                if (uploadResult.Error != null) throw new InvalidOperationException(string.Format(ServiceConstant.FAILED_TO_UPDATE_FILE_ERROR_MESSAGE_TEMPLATE,
                                                                                                        uploadResult.Error.Message));

                CloudinaryImage image = mapper.Map<ImageUploadResult, CloudinaryImage>(uploadResult);
                this.UpdateImageFileMetadata(image, uploadResult.Format, uploadResult.Version);

                publicId = publicId.Split('/')[1];

                this.imageRepository.UpdateImageFileInfo(publicId, image);

                return publicId;
                
            }
            else throw new FileNotFoundException(ServiceConstant.IMAGE_NOT_FOUND_ERROR_MESSAGE);
        }

        public async Task DeleteAsync(string publicId)
        {
            publicId = publicId.Replace(ServiceConstant.SLASH_URL_ENCODING, "/");
            var deletionParams = new DeletionParams(publicId);
            var deletionResult = await cloudinary.DestroyAsync(deletionParams);

            publicId = publicId.Split('/')[1];
            this.imageRepository.Delete(publicId);
        }

        public string GetImageUrlByItemId(string itemId)
        {
            CloudinaryImage image = this.imageRepository.GetImageBuildUrlInfoByItemId(itemId);
            if (image is not null)
            {
                if (image.FileExtension is null) throw new FileNotFoundException(ServiceConstant.IMAGE_NOT_FOUND_ERROR_MESSAGE);
                else
                {
                    string imageUrl = this.cloudinary.Api.UrlImgUp.BuildUrl(string.Format(ServiceConstant.CLOUDINARY_IMAGE_URL_TEMPLATE, image.Version,
                                                                                            ServiceConstant.CLOUDINARY_VSG_MARKETPLACE_IMAGES_FOLDER_NAME, 
                                                                                            image.Id, image.FileExtension));

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