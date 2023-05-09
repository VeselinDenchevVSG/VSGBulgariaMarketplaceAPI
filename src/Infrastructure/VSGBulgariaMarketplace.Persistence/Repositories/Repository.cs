namespace VSGBulgariaMarketplace.Persistence.Repositories
{
    using Dapper;

    using System;
    using System.Data;
    using System.Linq;
    using System.Text;

    using VSGBulgariaMarketplace.Application.Models.Repositories;
    using VSGBulgariaMarketplace.Application.Models.UnitOfWork;
    using VSGBulgariaMarketplace.Domain.Entities;

    public abstract class Repository<T, U> : IRepository<T, U> where T : BaseEntity<U>
    {
        protected readonly IUnitOfWork unitOfWork;

        protected string tableName;
        private StringBuilder stringBuilder;
        protected string[] classPropertiesNames;
        protected string columnNamesString;
        protected string parameterizedColumnsNamesString;
        protected string parameterizedColumnsNamesUpdateString;
        protected string insertSqlCommand;
        protected readonly string updateSqlCommand;
        protected string[] updateSkipProperties;

        public Repository(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            this.tableName = typeof(T).Name + 's';
            this.classPropertiesNames = GetClassPropertiesNames();
            this.columnNamesString = this.GetColumnsNamesString();
            this.parameterizedColumnsNamesString = this.GetParameterizedColumnNamesString();

            this.updateSkipProperties = new string[]
            {
                "Id", "CreatedAtUtc"
            };

            this.parameterizedColumnsNamesUpdateString = GetParameterizedColumnsNamesUpdateString();

            this.insertSqlCommand = $"INSERT INTO {this.tableName} {this.columnNamesString} " +
                                        $"VALUES {this.parameterizedColumnsNamesString}";
            this.updateSqlCommand = $"UPDATE {this.tableName} SET {this.parameterizedColumnsNamesUpdateString} WHERE Id LIKE @Id AND " +
                                    $"IsDeleted = 0";
        }

        public IDbConnection DbConnection => this.unitOfWork.DbConnection;

        public IDbTransaction Transaction => this.unitOfWork.Transaction;

        public T[] GetAll()
        {
            string sql = $"SELECT * FROM {this.tableName} WHERE IsDeleted = 0";
            T[] entities = DbConnection.Query<T>(sql, transaction: this.Transaction).ToArray();

            return entities;
        }

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

            DbConnection.Execute(this.insertSqlCommand, entity, transaction: this.Transaction);
        }

        public virtual void CreateMany(T[] entities)
        {
            foreach (T entity in entities)
            {
                entity.CreatedAtUtc = DateTime.UtcNow;
                entity.ModifiedAtUtc = entity.CreatedAtUtc;
            }

            DbConnection.Execute(this.insertSqlCommand, entities, transaction: this.Transaction);
        }


        public virtual void Update(U id, T entity)
        {
            entity.Id = id;
            entity.ModifiedAtUtc = DateTime.UtcNow;

            DbConnection.Execute(this.updateSqlCommand, entity, transaction: this.Transaction);
        }

        public virtual void Delete(U id)
        {
            string sql = $"UPDATE {this.tableName} SET IsDeleted = 1, DeletedAtUtc = GETDATEUTC() WHERE Id = @Id";
            DbConnection.Execute(sql, new { Id = id }, transaction: this.Transaction);
        }

        public virtual void DeleteMany(U[] ids)
        {
            string sql = $"UPDATE {tableName} SET IsDeleted = 1, DeletedAtUtc = GETDATEUTC() WHERE Id IN @Id";
            DbConnection.Execute(sql, new { Id = ids }, transaction: this.Transaction);
        }

        private static string[] GetClassPropertiesNames() => typeof(T).GetProperties().Select(p => p.Name).ToArray(); // Skip the Id

        private string GetParameterizedColumnNamesString()
        {
            this.stringBuilder = new StringBuilder("(");

            foreach (string propertyName in this.classPropertiesNames)
            {
                if (propertyName != "Id")
                {
                    this.stringBuilder.Append($"@{propertyName}, ");
                }
            }

            this.stringBuilder.Remove(stringBuilder.Length - 2, 2);
            this.stringBuilder.Append(')');

            string tableColumnsNames = this.stringBuilder.ToString();
            this.stringBuilder.Clear();

            return tableColumnsNames;
        }

        private string GetColumnsNamesString()
        {
            this.stringBuilder = new StringBuilder("(");

            foreach (string propertyName in this.classPropertiesNames)
            {
                if (propertyName != "Id")
                {
                    this.stringBuilder.Append($"{propertyName}, ");
                }
            }

            this.stringBuilder.Remove(stringBuilder.Length - 2, 2);
            this.stringBuilder.Append(')');

            string columnNames = this.stringBuilder.ToString();
            this.stringBuilder.Clear();

            return columnNames;
        }

        private string GetParameterizedColumnsNamesUpdateString()
        {
            this.stringBuilder = new StringBuilder();

            foreach (string propertyName in this.classPropertiesNames)
            {
                if (!updateSkipProperties.Contains(propertyName))
                {
                    stringBuilder.Append($"{propertyName}=@{propertyName}, ");
                }
            }

            this.stringBuilder.Remove(stringBuilder.Length - 2, 2);

            string parameterizedColumnsNamesUpdateString = this.stringBuilder.ToString();
            this.stringBuilder.Clear();

            return parameterizedColumnsNamesUpdateString;
        }
    }
}