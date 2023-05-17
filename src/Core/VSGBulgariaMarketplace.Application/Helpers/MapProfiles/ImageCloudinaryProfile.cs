namespace VSGBulgariaMarketplace.Application.Helpers.MapProfiles
{
    using AutoMapper;
    using CloudinaryDotNet.Actions;
    using VSGBulgariaMarketplace.Domain.Entities;
    using System.Linq;

    public class ImageCloudinaryProfile : Profile
    {
        public ImageCloudinaryProfile()
        {
            CreateMap<ImageUploadResult, CloudinaryImage>().ForMember(i => i.Id, x => x.MapFrom(r => SimplifyPublicId(r.PublicId)));
        }

        private string SimplifyPublicId(string fullPublicId)
        {
            string simplifiedPublicId = fullPublicId.Split('/').TakeLast(1).First();

            return simplifiedPublicId;
        }
    }
}