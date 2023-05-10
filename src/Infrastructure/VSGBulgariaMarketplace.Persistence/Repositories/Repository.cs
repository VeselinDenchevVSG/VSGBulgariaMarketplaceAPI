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

        public Repository(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            this.tableName = typeof(T).Name + 's';
        }

        public IDbConnection DbConnection => this.unitOfWork.DbConnection;

        public IDbTransaction Transaction => this.unitOfWork.Transaction;

        public virtual T GetById(U id)
        {
            string sql = $"SELECT * FROM {this.tableName} WHERE Id = @Id AND IsDeleted = 0";
            T entity = this.DbConnection.QueryFirstOrDefault<T>(sql, new { Id = id }, transaction: this.Transaction);

            return entity;
        }

        public virtual void Create(T entity)
        {
            entity.CreatedAtUtc = DateTime.UtcNow;
            entity.ModifiedAtUtc = entity.CreatedAtUtc;

            string sql = $"INSERT INTO {this.tableName} {this.columnNamesString} " +
                            $"VALUES {this.parameterizedColumnsNamesString}";
            this.DbConnection.Execute(sql, entity, transaction: this.Transaction);
        }

        public virtual void Update(U id, T entity)
        {
            entity.ModifiedAtUtc = DateTime.UtcNow;

            string sql = $"UPDATE {this.tableName} SET {this.parameterizedColumnsNamesUpdateString} WHERE Id = @OldId AND IsDeleted = 0";
            bool hasBeenUpdated = Convert.ToBoolean(this.DbConnection.Execute(sql, entity, transaction: this.Transaction));
            if (!hasBeenUpdated)
            {
                throw new ArgumentException($"{typeof(T)} with id = {id} doesn't exist!");
            }
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

        protected static string GetParameterizedColumnNamesString(string columnNamesString)
        {
            StringBuilder stringBuilder = new StringBuilder("(");

            string[] parameters = columnNamesString.Replace("(", string.Empty)
                                                    .Replace(")", string.Empty)
                                                    .Split(", ");

            foreach (string propertyName in parameters)
            {
                stringBuilder.Append($"{propertyName} = @{propertyName}, ");
            }

            stringBuilder.Remove(stringBuilder.Length - 2, 2);
            stringBuilder.Append(')');

            string tableColumnsNames = stringBuilder.ToString();

            return tableColumnsNames;
        }

        protected void SetParameterizedColumnNamesUpdateString()
        {
            this.parameterizedColumnsNamesUpdateString = this.parameterizedColumnsNamesString
                                                    .Replace("(", string.Empty)
                                                    .Replace(" CreatedAtUtc = @CreatedAtUtc,", string.Empty)
                                                    .Replace(" DeletedAtUtc = @DeletedAtUtc,", string.Empty)
                                                    .Replace(", IsDeleted = @IsDeleted", string.Empty)
                                                    .Replace(")", string.Empty);
        }

        protected void SetUpRepository(bool hasUpdate = true)
        {
            this.parameterizedColumnsNamesString = GetParameterizedColumnNamesString(this.columnNamesString);
            this.SetParameterizedColumnNamesUpdateString();
        }
    }
}