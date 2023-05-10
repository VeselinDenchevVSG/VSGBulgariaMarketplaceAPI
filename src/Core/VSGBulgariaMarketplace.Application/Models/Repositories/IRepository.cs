namespace VSGBulgariaMarketplace.Application.Models.Repositories
{
    using System.Data;

    using VSGBulgariaMarketplace.Domain.Entities;

    public interface IRepository<T, U> where T : BaseEntity<U>
    {
        public IDbTransaction Transaction { get; }

        T[] GetAll();

        T GetById(U id);

        void Create(T entity);

        //void CreateMany(T[] entities);

        void Update(U id, T entity);

        void Delete(U id);

        //void DeleteMany(U[] ids);
    }
}