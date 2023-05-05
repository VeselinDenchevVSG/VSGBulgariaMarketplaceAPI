namespace VSGBulgariaMarketplace.Application.Models.UnitOfWork
{
    using System.Data;

    public interface IUnitOfWork : IDisposable
    {
        public IDbConnection DbConnection { get; set; }

        public IDbTransaction Transaction { get; set; }

        public void Begin();

        public void Rollback();

        void Commit();
    }
}
