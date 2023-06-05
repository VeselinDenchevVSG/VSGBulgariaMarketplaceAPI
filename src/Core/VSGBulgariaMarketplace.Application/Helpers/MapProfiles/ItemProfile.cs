namespace VSGBulgariaMarketplace.Application.Helpers.MapProfiles
{
    using AutoMapper;

    using VSGBulgariaMarketplace.Application.Models.Item.Dtos;
    using VSGBulgariaMarketplace.Application.Services.HelpServices;
    using VSGBulgariaMarketplace.Domain.Entities;
    using VSGBulgariaMarketplace.Domain.Enums;

    public class ItemProfile : Profile
    {
        public ItemProfile()
        {
            CreateMap<Item, MarketplaceItemDto>().ForMember(dto => dto.Category, x => x.MapFrom(e => EnumService.GetEnumDisplayName(e.Category)));
            CreateMap<Item, ItemDetailsDto>().ForMember(dto => dto.Category, x => x.MapFrom(e => EnumService.GetEnumDisplayName(e.Category)));
            CreateMap<Item, InventoryItemDto>().ForMember(dto => dto.QuantityForSale, x => x.MapFrom(e => e.QuantityForSale == null ? 0 : e.QuantityForSale))
                                                .ForMember(dto => dto.AvailableQuantity, x => x.MapFrom(e => e.AvailableQuantity == null ? 0 : e.AvailableQuantity))
                                                .ForMember(dto => dto.Category, x => x.MapFrom(e => EnumService.GetEnumDisplayName(e.Category)))
                                                .ForMember(dto => dto.Location, x => x.MapFrom(e => EnumService.GetEnumDisplayName(e.Location)));

            CreateMap<CreateItemDto, Item>().ForMember(e => e.QuantityCombined, x => x.MapFrom(dto => dto.Quantity))
                                            .ForMember(e => e.Category, x => x.MapFrom(dto => EnumService.GetEnumValueFromDisplayName<Category>(dto.Category)))
                                            .ForMember(e => e.Location, x => x.MapFrom(dto => EnumService.GetEnumValueFromDisplayName<Location>(dto.Location)));

            CreateMap<UpdateItemDto, Item>().ForMember(e => e.QuantityCombined, x => x.MapFrom(dto => dto.Quantity))
                                            .ForMember(e => e.Category, x => x.MapFrom(dto => EnumService.GetEnumValueFromDisplayName<Category>(dto.Category)))
                                            .ForMember(e => e.Location, x => x.MapFrom(dto => EnumService.GetEnumValueFromDisplayName<Location>(dto.Location)));
        }
    }
}
