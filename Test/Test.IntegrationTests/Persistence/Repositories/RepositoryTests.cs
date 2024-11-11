namespace Test.IntegrationTests.Persistence.Repositories
{
    using Dapper;

    using Microsoft.Data.SqlClient;

    using Test.IntegrationTests.Helpers;

    using VSGBulgariaMarketplace.Application.Models.UnitOfWork;
    using VSGBulgariaMarketplace.Persistence.UnitOfWork;

    using static Test.IntegrationTests.Constants.IntegrationTestsDatabaseConstant;

    internal class RepositoryTests
    {
        protected readonly IUnitOfWork unitOfWork;
        protected readonly DatabaseHelper databaseHelper = new();

        public RepositoryTests()
        {
            this.unitOfWork = new UnitOfWork(this.databaseHelper.configuration, INTEGRATION_TESTS_CONNECTION_STRING_NAME);
        }

        protected async Task DeleteFromTableAsync(string tableName)
        {
            await using SqlConnection connection = new(this.databaseHelper.integrationTestsConnectionString);
            await connection.QueryAsync($"DELETE FROM {tableName}");
        }
    }
}
