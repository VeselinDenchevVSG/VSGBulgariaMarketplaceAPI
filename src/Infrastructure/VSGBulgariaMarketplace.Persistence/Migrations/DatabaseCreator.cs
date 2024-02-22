namespace VSGBulgariaMarketplace.Persistence.Migrations
{
    using Dapper;

    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Configuration;

    using static VSGBulgariaMarketplace.Persistence.Constants.DatabaseConnectionConstant;

    public static class DatabaseCreator
    {
        public static void Create(IConfiguration configuration)
        {
            SqlConnectionStringBuilder defaultConnectionStringBuilder = new SqlConnectionStringBuilder(configuration.GetConnectionString(DEFAULT_CONNECTION_STRING_NAME));
            string dbName = defaultConnectionStringBuilder.InitialCatalog;

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add(DB_NAME_PARAMETER_NAME, dbName);

            using SqlConnection connection = new SqlConnection(configuration.GetConnectionString(MASTER_CONNECTION_STRING_NAME));
            var records = connection.Query(CHECK_IF_DATABASE_EXISTS_SQL_QUERY, parameters);

            if (records.Any() == false)
            {
                connection.Execute(string.Format(CREATE_DATABASE_SQL_QUERY_TEMPLATE, dbName));
            }
        }
    }
}
