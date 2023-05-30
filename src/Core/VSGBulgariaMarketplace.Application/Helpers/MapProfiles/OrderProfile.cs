namespace VSGBulgariaMarketplace.Application.Helpers.MapProfiles
{
    using AutoMapper;
    using VSGBulgariaMarketplace.Application.Models.Order.Dtos;
    using VSGBulgariaMarketplace.Domain.Entities;
    using VSGBulgariaMarketplace.Domain.Enums;

    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<Order, PendingOrderDto>().ForMember(dto => dto.OrderDate, x => x.MapFrom(e => e.CreatedAtUtc.ToLocalTime()));
            CreateMap<Order, UserOrderDto>().ForMember(dto => dto.OrderDate, x => x.MapFrom(e => e.CreatedAtUtc.ToLocalTime()))
                                            .ForMember(dto => dto.Status, x => x.MapFrom(e => e.Status.ToString()));
            CreateMap<CreateOrderDto, Order>().ForMember(e => e.Status, x => x.MapFrom(dto => OrderStatus.Pending));
        }
    }
}
