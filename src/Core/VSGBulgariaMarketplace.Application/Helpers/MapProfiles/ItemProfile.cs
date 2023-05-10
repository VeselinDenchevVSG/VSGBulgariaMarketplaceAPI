namespace VSGBulgariaMarketplace.Application.Helpers.MapProfiles
{
    using AutoMapper;

    using VSGBulgariaMarketplace.Application.Models.Item.Dtos;
    using VSGBulgariaMarketplace.Domain.Entities;

    public class ItemProfile : Profile
    {
        public ItemProfile()
        {
            CreateMap<Item, GridItemDto>().ForMember(dto => dto.Code, x => x.MapFrom(e => e.Id));
            CreateMap<Item, ItemDetailsDto>();
            CreateMap<ManageItemDto, Item>().ForMember(e => e.Id, x => x.MapFrom(dto => dto.Code)).ReverseMap();
        }
    }
}
