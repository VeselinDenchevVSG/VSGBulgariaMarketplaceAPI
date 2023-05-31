namespace VSGBulgariaMarketplace.Application.Helpers.MapProfiles
{
    using AutoMapper;

    using VSGBulgariaMarketplace.Application.Models.Item.Dtos;
    using VSGBulgariaMarketplace.Domain.Entities;

    public class ItemProfile : Profile
    {
        public ItemProfile()
        {
            CreateMap<Item, MarketplaceItemDto>();
            CreateMap<Item, ItemDetailsDto>();
            CreateMap<Item, InventoryItemDto>().ForMember(dto => dto.QuantityForSale, x => x.MapFrom(e => e.QuantityForSale == null ? 0 : e.QuantityForSale));
            CreateMap<Item, CreateItemDto>().ForMember(dto => dto.Quantity, x => x.MapFrom(e => e.QuantityCombined))
                                            .ForMember(dto => dto.Image, x => x.Ignore())
                                            .ReverseMap();
            CreateMap<Item, UpdateItemDto>().ForMember(dto => dto.Quantity, x => x.MapFrom(e => e.QuantityCombined))
                                            .ForMember(dto => dto.Image, x => x.Ignore())
                                            .ReverseMap();

        }
    }
}
