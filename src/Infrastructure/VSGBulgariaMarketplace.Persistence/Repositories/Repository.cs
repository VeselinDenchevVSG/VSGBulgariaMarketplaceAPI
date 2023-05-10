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
        //protected string[] classPropertiesNames;
        protected string columnNamesString;
        //protected string columnNamesUpdateString;
        protected string parameterizedColumnsNamesString;
        protected string parameterizedColumnsNamesUpdateString;
        //protected string insertSqlCommand;
        //protected readonly string updateSqlCommand;
        //protected string[] createSkipProperties;
        //protected List<string> updateSkipProperties;

        public Repository(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            this.tableName = typeof(T).Name + 's';
            //this.classPropertiesNames = GetClassPropertiesNames();

            //this.columnNamesString = this.GetColumnsNamesString();
            //this.parameterizedColumnsNamesString = this.GetParameterizedColumnNamesString();

            //this.updateSkipProperties = new List<string>
            //{
            //    "CreatedAtUtc"
            //};

            //this.parameterizedColumnsNamesUpdateString = this.GetParameterizedColumnsNamesUpdateString();
            //this.insertSqlCommand = $"INSERT INTO {this.tableName} {this.columnNamesString} " +
            //                            $"VALUES {this.parameterizedColumnsNamesString}";
            //this.updateSqlCommand = $"UPDATE {this.tableName} SET {this.parameterizedColumnsNamesString} WHERE Id LIKE @Id AND " +
            //                        $"IsDeleted = 0";
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

            string sql = $"INSERT INTO {this.tableName} {this.columnNamesString} " +
                            $"VALUES {this.parameterizedColumnsNamesString}";
            this.DbConnection.Execute(sql, entity, transaction: this.Transaction);
        }

        //public virtual void CreateMany(T[] entities)
        //{
        //    foreach (T entity in entities)
        //    {
        //        entity.CreatedAtUtc = DateTime.UtcNow;
        //        entity.ModifiedAtUtc = entity.CreatedAtUtc;
        //    }

        //    DbConnection.Execute(insertSqlCommand, entities, transaction: this.Transaction);
        //}


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

        //public virtual void DeleteMany(U[] ids)
        //{
        //    string sql = $"UPDATE {tableName} SET IsDeleted = 1, DeletedAtUtc = GETDATE() WHERE Id IN @Id";


        //    DbConnection.Execute(sql, new { Id = ids }, transaction: this.Transaction);
        //}

        //private static string[] GetClassPropertiesNames() => typeof(T).GetProperties().Select(p => p.Name).ToArray();

        protected static string GetParameterizedColumnNamesString(string columnNamesString)
        {
            StringBuilder stringBuilder = new StringBuilder("(");

            //stringBuilder.Append(string.Join(", @", this.classPropertiesNames));
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
            stringBuilder.Clear();

            return tableColumnsNames;
        }

        //private string GetColumnsNamesString()
        //{
        //    this.stringBuilder = new StringBuilder("(");

        //    foreach (string propertyName in this.classPropertiesNames)
        //    {
        //        if (propertyName != "Id")
        //        {
        //            this.stringBuilder.Append($"{propertyName}, ");
        //        }
        //    }

        //    this.stringBuilder.Remove(stringBuilder.Length - 2, 2);
        //    this.stringBuilder.Append(')');

        //    string columnNames = this.stringBuilder.ToString();
        //    this.stringBuilder.Clear();

        //    return columnNames;
        //}

        //private string GetParameterizedColumnsNamesUpdateString()
        //{
        //    this.stringBuilder = new StringBuilder();

        //    foreach (string propertyName in this.classPropertiesNames)
        //    {
        //        if (!updateSkipProperties.Contains(propertyName))
        //        {
        //            stringBuilder.Append($"{propertyName}=@{propertyName}, ");
        //        }
        //    }

        //    this.stringBuilder.Remove(stringBuilder.Length - 2, 2);

        //    string parameterizedColumnsNamesUpdateString = this.stringBuilder.ToString();
        //    this.stringBuilder.Clear();

        //    return parameterizedColumnsNamesUpdateString;
        //}
    }
}