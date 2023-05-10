namespace VSGBulgariaMarketplace.Application.Services
{
    using AutoMapper;

    using Microsoft.Extensions.Caching.Memory;
    using VSGBulgariaMarketplace.Application.Models.Repositories;
    using VSGBulgariaMarketplace.Domain.Entities;

    public abstract class BaseService<T, U> where T : IRepository<U, int>
                                            where U : BaseEntity<int>
    {
        protected T repository;
        protected IMemoryCache memoryCache;
        protected IMapper mapper;

        public BaseService(T repository, IMemoryCache memoryCache, IMapper mapper)
        {
            this.repository = repository;
            this.memoryCache = memoryCache;
            this.mapper = mapper;
        }
    }
}
