namespace VSGBulgariaMarketplace.Application.Services
{
    using AutoMapper;

    using VSGBulgariaMarketplace.Application.Models.Repositories;
    using VSGBulgariaMarketplace.Application.Services.HelpServices.Interfaces;
    using VSGBulgariaMarketplace.Domain.Entities;

    public abstract class BaseService<T, U> where T : IRepository<U, int>
                                            where U : BaseEntity<int>
    {
        protected T repository;
        protected IMemoryCacheAdapter cacheAdapter;
        protected IMapper mapper;

        public BaseService(T repository, IMemoryCacheAdapter cacheAdapter, IMapper mapper)
        {
            this.repository = repository;
            this.cacheAdapter = cacheAdapter;
            this.mapper = mapper;
        }
    }
}
