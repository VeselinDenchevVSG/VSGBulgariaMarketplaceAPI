namespace VSGBulgariaMarketplace.Persistence.Repositories
{
    using Dapper;

    using Microsoft.Data.SqlClient;

    using System;
    using System.Data;
    using System.Text;

    using VSGBulgariaMarketplace.Application.Models.Exceptions;
    using VSGBulgariaMarketplace.Application.Models.Repositories;
    using VSGBulgariaMarketplace.Application.Models.UnitOfWork;
    using VSGBulgariaMarketplace.Domain.Entities;
    using VSGBulgariaMarketplace.Persistence.Constants;

    public abstract class Repository<T, U> : IRepository<T, U> where T : BaseEntity<U>
    {
        protected readonly IUnitOfWork unitOfWork;

        protected string entityName;
        protected string tableName;
        protected string columnNamesString;
        protected string parameterizedColumnsNamesString;
        protected string parameterizedColumnsNamesUpdateString;
        protected List<string> updateStringSkipProperties;

        public Repository(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            this.entityName = typeof(T).Name;
            this.tableName = this.entityName + 's';
            this.updateStringSkipProperties = new List<string>()
            {
                DatabaseConstant.ID_COLUMN_NAME,
                DatabaseConstant.CREATED_AT_UTC_COLUMN_NAME,
            };
        }

        public IDbConnection DbConnection => this.unitOfWork.DbConnection;

        public IDbTransaction Transaction => this.unitOfWork.Transaction;

        public virtual void Create(T entity)
        {
            entity.CreatedAtUtc = DateTime.UtcNow;
            entity.ModifiedAtUtc = entity.CreatedAtUtc;

            string sql = string.Format(RepositoryConstant.CREATE_ENTITY_SQL_QUERY, this.tableName, this.columnNamesString, this.parameterizedColumnsNamesString);

            try
            {
                this.DbConnection.Execute(sql, entity, transaction: this.Transaction);
            }
            catch (SqlException se) when (se.Number == 2627)
            {
                throw new EntityAlreadyExistsException(string.Format(RepositoryConstant.ENTITY_ALREADY_EXISTS_ERROR_MESSAGE, this.entityName));
            }
        }

        public virtual void Delete(U id)
        {
            string sql = string.Format(RepositoryConstant.DELETE_ENTITY_SQL_QUERY, this.tableName);
            bool hasBeenDeleted = 
                Convert.ToBoolean(this.DbConnection.Execute(sql, new { Id = id }, transaction: this.Transaction));
            if (!hasBeenDeleted)
            {
                throw new NotFoundException(string.Format(RepositoryConstant.ENTITY_DOES_NOT_EXIST_ERROR_MESSAGE, typeof(T).Name));
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
                stringBuilder.Append(string.Format(RepositoryConstant.SQL_QUERY_PARAMETER_TEMPLATE, propertyName));
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
                    stringBuilder.Append(string.Format(RepositoryConstant.SQL_QUERY_COLUMN_PARAMETER_TEMPLATE, propertyName));
                }
            }

            stringBuilder.Remove(stringBuilder.Length - 2, 2);

            string tableColumnsNames = stringBuilder.ToString();

            return tableColumnsNames;
        }

        protected void SetUpRepository()
        {
            this.parameterizedColumnsNamesString = this.GetParameterizedColumnNamesString();
            this.parameterizedColumnsNamesUpdateString = this.GetParameterizedColumnNamesUpdateString();
        }
    }
}