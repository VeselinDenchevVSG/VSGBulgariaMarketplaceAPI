namespace VSGBulgariaMarketplace.Application.Models.Order.Interfaces
{
    using VSGBulgariaMarketplace.Application.Models.Repositories;
    using VSGBulgariaMarketplace.Domain.Entities;

    public interface IOrderRepository : IRepository<Order, int>
    {
    }
}