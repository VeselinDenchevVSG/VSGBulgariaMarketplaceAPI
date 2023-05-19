namespace VSGBulgariaMarketplace.Application.Helpers.MapProfiles
{
    using AutoMapper;

    using VSGBulgariaMarketplace.Application.Models.Item.Dtos;
    using VSGBulgariaMarketplace.Domain.Entities;

    public class ItemProfile : Profile
    {
        public ItemProfile()
        {
            CreateMap<Item, MarketplaceItemDto>().ForMember(dto => dto.Code, x => x.MapFrom(e => e.Id))
                                                    .ForMember(dto => dto.ImageUrl, x => x.MapFrom(e => e.Image.SecureUrl));
            CreateMap<Item, ItemDetailsDto>().ForMember(dto => dto.ImageUrl, x => x.MapFrom(e => e.Image.SecureUrl));
            CreateMap<ManageItemDto, Item>().ForMember(e => e.Id, x => x.MapFrom(dto => dto.Code));
            CreateMap<Item, ManageItemDto>().ForMember(dto => dto.Code, x => x.MapFrom(e => e.Id));
            CreateMap<Item, InventoryItemDto>().ForMember(dto => dto.Code, x => x.MapFrom(e => e.Id))
                                                .ForMember(dto => dto.QuantityForSale, x => x.MapFrom(e => e.QuantityForSale == null ? 0 : e.QuantityForSale));
        }
    }
}
