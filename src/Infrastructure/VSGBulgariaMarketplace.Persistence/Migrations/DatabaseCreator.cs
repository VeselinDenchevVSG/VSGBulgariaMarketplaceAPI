namespace VSGBulgariaMarketplace.Persistence.Migrations
{
    using Dapper;

    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Configuration;

    using static VSGBulgariaMarketplace.Persistence.Constants.DatabaseConnectionConstant;

    public static class DatabaseCreator
    {
        public static void Create(IConfiguration configuration, string connectionStringName, string masterConnectionStringName)
        {
            SqlConnectionStringBuilder defaultConnectionStringBuilder = new(configuration.GetConnectionString(connectionStringName));
            string dbName = defaultConnectionStringBuilder.InitialCatalog;

            DynamicParameters parameters = new();
            parameters.Add(DB_NAME_PARAMETER_NAME, dbName);

            using SqlConnection connection = new(configuration.GetConnectionString(masterConnectionStringName));
            var records = connection.Query(CHECK_IF_DATABASE_EXISTS_SQL_QUERY, parameters);

            if (records.Any()) return;

            connection.Execute(string.Format(CREATE_DATABASE_SQL_QUERY_TEMPLATE, dbName));
        }
    }
}
