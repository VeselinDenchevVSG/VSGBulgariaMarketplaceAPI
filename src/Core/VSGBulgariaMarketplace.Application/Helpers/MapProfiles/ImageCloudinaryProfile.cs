namespace VSGBulgariaMarketplace.Application.Helpers.MapProfiles
{
    using AutoMapper;
    using CloudinaryDotNet.Actions;
    using VSGBulgariaMarketplace.Domain.Entities;

    public class ImageCloudinaryProfile : Profile
    {
        public ImageCloudinaryProfile()
        {
            CreateMap<ImageUploadResult, CloudinaryImage>().ForMember(i => i.Id, x => x.MapFrom(r => r.PublicId));
        }
    }
}