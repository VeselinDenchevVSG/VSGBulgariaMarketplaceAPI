namespace VSGBulgariaMarketplace.Persistence.Migrations
{
    using Dapper;

    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Configuration;

    public static class DatabaseCreator
    {
        public static void Create(IConfiguration configuration)
        {
            SqlConnectionStringBuilder defaultConnectionStringBuilder = new SqlConnectionStringBuilder(configuration.GetConnectionString("DefaultConnection"));
            string dbName = defaultConnectionStringBuilder.InitialCatalog;

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("name", dbName);

            using SqlConnection connection = new SqlConnection(configuration.GetConnectionString("MasterConnection"));
            var records = connection.Query("SELECT * FROM sys.databases WHERE name = @name",
                 parameters);

            if (records.Any() == false)
            {
                connection.Execute($"CREATE DATABASE {dbName}");
            }
        }
    }
}
