namespace VSGBulgariaMarketplace.Persistence.Migrations
{
    using Dapper;

    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Configuration;

    public static class DatabaseCreator
    {
        public static void Create(IConfiguration configuration)
        {
            var evaluationConnectionString = new SqlConnectionStringBuilder(configuration.GetConnectionString("DefaultConnection"));
            string dbName = evaluationConnectionString.InitialCatalog;

            var parameters = new DynamicParameters();
            parameters.Add("name", dbName);

            using var connection = new SqlConnection(configuration.GetConnectionString("MasterDbConnection"));
            var records = connection.Query("SELECT * FROM sys.databases WHERE name = @name",
                 parameters);

            if (records.Any() == false)
            {
                connection.Execute($"CREATE DATABASE {dbName}");
            }
        }
    }
}
