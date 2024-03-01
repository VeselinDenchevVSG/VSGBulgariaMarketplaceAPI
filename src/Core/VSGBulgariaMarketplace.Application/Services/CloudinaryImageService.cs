namespace VSGBulgariaMarketplace.Application.Services
{
    using AutoMapper;

    using CloudinaryDotNet;
    using CloudinaryDotNet.Actions;

    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;

    using System;
    using System.Threading.Tasks;

    using VSGBulgariaMarketplace.Application.Models.Image.Interfaces;
    using VSGBulgariaMarketplace.Domain.Entities;

    using static VSGBulgariaMarketplace.Application.Constants.ServiceConstant;

    public class CloudinaryImageService : ICloudImageService
    {
        private readonly IImageRepository imageRepository;
        private readonly IMapper mapper;

        private readonly Account cloudinaryAccount;
        private readonly Cloudinary cloudinary;

        public CloudinaryImageService(IImageRepository imageRepository, IMapper mapper, IConfiguration configuration)
        {
            this.imageRepository = imageRepository;
            this.mapper = mapper;

            // Set Cloudinary account
            string cloudinaryUrl = configuration[CLOUDINARY_CONFIGURATION_CLOUD];
            string cloudinaryApiKey = configuration[CLOUDINARY_CONFIGURATION_API_KEY];
            string cloudinaryApiSecret = configuration[CLOUDINARY_CONFIGURATION_API_SECRET];

            this.cloudinaryAccount = new Account(cloudinaryUrl, cloudinaryApiKey, cloudinaryApiSecret);
            this.cloudinary = new Cloudinary(this.cloudinaryAccount);
            this.cloudinary.Api.Secure = true;
        }

        public async Task<bool> ExistsAsync(string publicId, CancellationToken cancellationToken)
        {
            var parameters = new GetResourceParams(publicId) { ResourceType = ResourceType.Image };
            GetResourceResult result = await this.cloudinary.GetResourceAsync(parameters, cancellationToken);

            bool exists = result != null && result.PublicId == publicId;

            return exists;
        }

        public async Task<string> UploadAsync(IFormFile imageFile, CancellationToken cancellationToken)
        {
            using Stream stream = ConvertIFormFileToStream(imageFile);

            string uniqueFileName = GenerateUniqueFileName();

            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(uniqueFileName, stream),
                Folder = CLOUDINARY_VSG_MARKETPLACE_IMAGES_FOLDER_NAME
            };
            ImageUploadResult uploadResult = await cloudinary.UploadAsync(uploadParams, cancellationToken);
            if (uploadResult.Error != null) throw new InvalidOperationException(string.Format(FAILED_TO_UPLOAD_FILE_ERROR_MESSAGE_TEMPLATE, 
                                                                                                        uploadResult.Error.Message));

            CloudinaryImage image = mapper.Map<ImageUploadResult, CloudinaryImage>(uploadResult);
            this.UpdateImageFileMetadata(image, uploadResult.Format, uploadResult.Version);

            try
            {
                await this.imageRepository.CreateAsync(image, cancellationToken);
            }
            catch (Exception)
            {
                await DeleteAsync(uploadResult.PublicId);

                throw;
            }

            return image.Id;
        }

        public async Task UpdateAsync(string publicId, IFormFile newImageFile, CancellationToken cancellationToken)
        {
            publicId = publicId.Replace(SLASH_URL_ENCODING, "/");

            bool exists = await ExistsAsync(publicId, cancellationToken);
            if (exists)
            {
                using Stream stream = ConvertIFormFileToStream(newImageFile);
                string uniqueFileName = GenerateUniqueFileName();

                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(uniqueFileName, stream),
                    PublicId = publicId, // Use the publicId of the existing file
                    Overwrite = true, // Overwrite the existing file
                    Invalidate = true,
                };

                ImageUploadResult uploadResult = await cloudinary.UploadAsync(uploadParams, cancellationToken);
                if (uploadResult.Error != null) throw new InvalidOperationException(string.Format(FAILED_TO_UPDATE_FILE_ERROR_MESSAGE_TEMPLATE,
                                                                                                        uploadResult.Error.Message));

                CloudinaryImage image = mapper.Map<ImageUploadResult, CloudinaryImage>(uploadResult);
                this.UpdateImageFileMetadata(image, uploadResult.Format, uploadResult.Version);

                this.imageRepository.UpdateImageFileInfo(publicId, image);
                
            }
            else throw new FileNotFoundException(IMAGE_NOT_FOUND_ERROR_MESSAGE);
        }

        public async Task<string> DeleteAsync(string publicId)
        {
            publicId = publicId.Replace(SLASH_URL_ENCODING, "/");
            var deletionParams = new DeletionParams(publicId);
            var deletionResult = await cloudinary.DestroyAsync(deletionParams);

            try
            {
                publicId = publicId.Split('/')[1];
            }
            catch (IndexOutOfRangeException) when (publicId is not null)
            {
                // Image publicId is splitted
            }

            this.imageRepository.Delete(publicId);

            return deletionResult.Result;
        }

        public string GetImageUrlByItemId(string itemId)
        {
            CloudinaryImage image = this.imageRepository.GetImageBuildUrlInfoByItemId(itemId);
            if (image is not null)
            {
                if (image.FileExtension is null) throw new FileNotFoundException(IMAGE_NOT_FOUND_ERROR_MESSAGE);
                else
                {
                    string imageUrl = this.cloudinary.Api.UrlImgUp.BuildUrl(string.Format(CLOUDINARY_IMAGE_URL_TEMPLATE, image.Version,
                                                                                            CLOUDINARY_VSG_MARKETPLACE_IMAGES_FOLDER_NAME, 
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
            try
            {
                image.Id = image.Id.Split('/').TakeLast(1).First();
            }
            catch (IndexOutOfRangeException) when (image.Id is not null)
            {
                // Image publicId is splitted
            }

            image.FileExtension = fileExtension;
            image.Version = int.Parse(version);
        }
    }
}