namespace VSGBulgariaMarketplace.Application.Models.Repositories
{
    using System.Data;

    using VSGBulgariaMarketplace.Domain.Entities;

    public interface IRepository<T, U> where T : BaseEntity<U>
    {
        public IDbTransaction Transaction { get; }

        T GetById(U id);

        void Create(T entity);

        void Update(U id, T entity);

        void Delete(U id);
    }
}