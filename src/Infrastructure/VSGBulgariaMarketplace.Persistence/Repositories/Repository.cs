namespace VSGBulgariaMarketplace.Persistence.Repositories
{
    using Dapper;

    using System;
    using System.Data;
    using System.Text;

    using VSGBulgariaMarketplace.Application.Models.Repositories;
    using VSGBulgariaMarketplace.Application.Models.UnitOfWork;
    using VSGBulgariaMarketplace.Domain.Entities;

    public abstract class Repository<T, U> : IRepository<T, U> where T : BaseEntity<U>
    {
        protected readonly IUnitOfWork unitOfWork;

        protected string tableName;
        protected string columnNamesString;
        protected string parameterizedColumnsNamesString;
        protected string parameterizedColumnsNamesUpdateString;
        protected List<string> updateStringSkipProperties;

        public Repository(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            this.tableName = typeof(T).Name + 's';
            this.updateStringSkipProperties = new List<string>()
            {
                "CreatedAtUtc",
                "DeletedAtUtc",
                "IsDeleted"
            };
        }

        public IDbConnection DbConnection => this.unitOfWork.DbConnection;

        public IDbTransaction Transaction => this.unitOfWork.Transaction;

        public virtual void Create(T entity)
        {
            entity.CreatedAtUtc = DateTime.UtcNow;
            entity.ModifiedAtUtc = entity.CreatedAtUtc;

            string sql = $"INSERT INTO {this.tableName} {this.columnNamesString} " +
                            $"VALUES {this.parameterizedColumnsNamesString}";
            this.DbConnection.Execute(sql, entity, transaction: this.Transaction);
        }

        public virtual void Delete(U id)
        {
            string sql = $"UPDATE {this.tableName} SET IsDeleted = 1, DeletedAtUtc = GETUTCDATE() WHERE Id = @Id";
            bool hasBeenDeleted = 
                Convert.ToBoolean(this.DbConnection.Execute(sql, new { Id = id }, transaction: this.Transaction));
            if (!hasBeenDeleted)
            {
                throw new ArgumentException($"{typeof(T)} with id = {id} doesn't exist!");
            }
        }

        protected string GetParameterizedColumnNamesString()
        {
            StringBuilder stringBuilder = new StringBuilder("(");

            string[] parameters = this.columnNamesString.Replace("(", string.Empty)
                                                    .Replace(")", string.Empty)
                                                    .Split(", ");

            foreach (string propertyName in parameters)
            {
                stringBuilder.Append($"@{propertyName}, ");
            }

            stringBuilder.Remove(stringBuilder.Length - 2, 2);
            stringBuilder.Append(')');

            string tableColumnsNames = stringBuilder.ToString();

            return tableColumnsNames;
        }

        protected string GetParameterizedColumnNamesUpdateString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            string[] parameters = this.columnNamesString.Replace("(", string.Empty)
                                                    .Replace(")", string.Empty)
                                                    .Split(", ");

            foreach (string propertyName in parameters)
            {
                if (!updateStringSkipProperties.Contains(propertyName))
                {
                    stringBuilder.Append($"{propertyName} = @{propertyName}, ");
                }
            }

            stringBuilder.Remove(stringBuilder.Length - 2, 2);

            string tableColumnsNames = stringBuilder.ToString();

            return tableColumnsNames;
        }

        protected void SetUpRepository(bool hasUpdate = true)
        {
            this.parameterizedColumnsNamesString = this.GetParameterizedColumnNamesString();
            this.parameterizedColumnsNamesUpdateString = this.GetParameterizedColumnNamesUpdateString();
        }
    }
}