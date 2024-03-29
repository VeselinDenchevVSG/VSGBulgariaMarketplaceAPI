﻿namespace VSGBulgariaMarketplace.Persistence.UnitOfWork
{
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Configuration;

    using System.Data;

    using VSGBulgariaMarketplace.Application.Models.UnitOfWork;
    
    using static VSGBulgariaMarketplace.Persistence.Constants.DatabaseConnectionConstant;

    public class UnitOfWork : IUnitOfWork
    {
        public UnitOfWork(IConfiguration configuration, string connectionStringName = DEFAULT_CONNECTION_STRING_NAME)
        {
            this.DbConnection = new SqlConnection(configuration.GetConnectionString(connectionStringName));
            this.DbConnection?.Open();
        }

        public IDbConnection DbConnection { get; set; }

        public IDbTransaction Transaction { get; set; }

        public void Begin()
        {
            this.Transaction = this.DbConnection.BeginTransaction();
        }
        public void Commit()
        {
            this.Transaction.Commit();
            this.Dispose();
        }

        public void Dispose()
        {
            if (this.Transaction != null)
            {
                this.Transaction.Dispose();
                this.Transaction = null;
            }
            if (this.DbConnection != null)
            {
                this.DbConnection.Dispose();
                this.DbConnection = null;
            }
        }
        public void Rollback()
        {
            this.Transaction.Rollback();
            this.Dispose();
        }
    }
}