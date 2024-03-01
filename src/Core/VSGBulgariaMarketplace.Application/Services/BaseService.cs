namespace VSGBulgariaMarketplace.Application.Services
{
    using AutoMapper;

    using VSGBulgariaMarketplace.Application.Models.Repositories;
    using VSGBulgariaMarketplace.Application.Services.HelpServices.Cache.Interfaces;
    using VSGBulgariaMarketplace.Domain.Entities;

    public abstract class BaseService<T, U> where T : IRepository<U, string>
                                            where U : BaseEntity<string>
    {
        protected readonly T repository;
        protected readonly IMemoryCacheAdapter cacheAdapter;
        protected readonly IMapper mapper;

        public BaseService(T repository, IMemoryCacheAdapter cacheAdapter, IMapper mapper)
        {
            this.repository = repository;
            this.cacheAdapter = cacheAdapter;
            this.mapper = mapper;
        }
    }
}
