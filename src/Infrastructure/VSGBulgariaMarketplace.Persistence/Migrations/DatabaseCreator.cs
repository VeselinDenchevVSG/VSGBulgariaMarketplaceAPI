namespace VSGBulgariaMarketplace.Persistence.Migrations
{
    using Dapper;

    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Configuration;

    public static class DatabaseCreator
    {
        private static readonly string defaultConnection = Environment.GetEnvironmentVariable("DEFAULT_CONNECTION_STRING");
        private static readonly string masterDbConnection = Environment.GetEnvironmentVariable("MASTER_DB_CONNECTION_STRING");

        public static void Create(IConfiguration configuration)
        {
            SqlConnectionStringBuilder defaultConnectionStringBuilder = new SqlConnectionStringBuilder(defaultConnection);
            string dbName = defaultConnectionStringBuilder.InitialCatalog;

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("name", dbName);

            using SqlConnection connection = new SqlConnection(masterDbConnection);
            var records = connection.Query("SELECT * FROM sys.databases WHERE name = @name",
                 parameters);

            if (records.Any() == false)
            {
                connection.Execute($"CREATE DATABASE {dbName}");
            }
        }
    }
}
