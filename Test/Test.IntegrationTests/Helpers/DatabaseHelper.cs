namespace Test.IntegrationTests.Helpers
{
    using Dapper;

    using FluentMigrator.Runner;

    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using System.Reflection;

    using VSGBulgariaMarketplace.Persistence.Migrations;

    using static VSGBulgariaMarketplace.Persistence.Constants.DatabaseConnectionConstant;

    using static VSGBulgariaMarketplace.API.Constants.BuilderConstant;

    using static Test.IntegrationTests.Constants.AssembliesConstant;
    using static Test.IntegrationTests.Constants.IntegrationTestsDatabaseConstant;

    internal class DatabaseHelper
    {
        protected readonly SqlConnectionStringBuilder integrationTestsConnectionStringBuilder;
        internal readonly string integrationTestsConnectionString;
        internal readonly IConfiguration configuration;
        internal readonly string masterConnectionString;
        internal readonly string integrationTestsDatabaseName;

        internal DatabaseHelper()
        {
            this.configuration = this.LoadConfigurationFromApiAssembly();
            this.masterConnectionString = configuration.GetConnectionString(MASTER_CONNECTION_STRING_NAME)!;
            this.integrationTestsConnectionString = configuration.GetConnectionString(INTEGRATION_TESTS_CONNECTION_STRING_NAME)!;
            this.integrationTestsConnectionStringBuilder = new(integrationTestsConnectionString);
            this.integrationTestsDatabaseName = integrationTestsConnectionStringBuilder.InitialCatalog;
        }

        internal void CreateIntegrationTestsDatabase() => 
            DatabaseCreator.Create(this.configuration, INTEGRATION_TESTS_CONNECTION_STRING_NAME, MASTER_CONNECTION_STRING_NAME);

        internal async Task DropIntegrationTestsDatabaseAsync()
        {
            SqlConnection.ClearAllPools();

            await using SqlConnection connection = new(this.masterConnectionString);

            try
            {
                await connection.QueryAsync(@$"
                    ALTER DATABASE {this.integrationTestsDatabaseName} SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                    DROP DATABASE {this.integrationTestsDatabaseName}");
            }
            catch (SqlException)
            {
                // Assume database doesn't exist
            }
        }

        internal void RunMigrations()
        {
            IServiceProvider serviceProvider = CreateServices();
            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                MigrateUp(scope.ServiceProvider);
            }
        }

        private IServiceProvider CreateServices()
        {
            Assembly persistenceAssembly = Assembly.Load(PERSISTENCE_ASSEMBLY_NAME);

            return new ServiceCollection()
                // Add common FluentMigrator services
                .AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    // Add SQL Server support
                    .AddSqlServer()
                    // Set the connection string
                    .WithGlobalConnectionString(this.integrationTestsConnectionString)
                    // Define the assembly containing your migrations
                    .ScanIn(persistenceAssembly).For.Migrations())
                .BuildServiceProvider(false);
        }

        private void MigrateUp(IServiceProvider serviceProvider)
        {
            var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
            runner.MigrateUp();
        }

        private IConfiguration LoadConfigurationFromApiAssembly()
        {
            // Specify the path to the directory containing the target appsettings.json
            // This could be a relative path or an absolute path

            // Assuming you know the name of the assembly
            Assembly assembly = Assembly.Load(API_ASSEMBLY_NAME);
            // Now get the location
            string assemblyLocation = assembly.Location;

            // And if you need the directory
            string assemblyDirectory = Path.GetDirectoryName(assemblyLocation)!;

            // Build the configuration
            IConfiguration configuration = new ConfigurationBuilder().SetBasePath(assemblyDirectory)
                                                                     .AddJsonFile(BUILDER_CONFIGURATION_APP_SETTINGS_JSON_FILE_NAME)
                                                                     .Build();

            return configuration;
        }
    }
}
