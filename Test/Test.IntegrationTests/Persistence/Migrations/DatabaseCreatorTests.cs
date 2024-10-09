namespace Test.IntegrationTests.Persistence.Migrations
{
    using Dapper;

    using FluentAssertions;
    using FluentAssertions.Execution;

    using Microsoft.Data.SqlClient;
    using Test.IntegrationTests.Helpers;

    using static VSGBulgariaMarketplace.Persistence.Constants.DatabaseConnectionConstant;

    public class DatabaseCreatorTests
    {
        private readonly DatabaseHelper databaseHelper = new();

        [SetUp]
        public void CreateIntegrationTestsDatabase() => this.databaseHelper.CreateIntegrationTestsDatabase();

        [TearDown]
        public async Task DropIntegrationTestsDatabaseAsync() 
            => await this.databaseHelper.DropIntegrationTestsDatabaseIfExistsAsync();

        [Test]
        public async Task Create_CreatesDatabase()
        {
            // Arrange
            DynamicParameters parameters = new();
            parameters.Add(DB_NAME_PARAMETER_NAME, this.databaseHelper.integrationTestsDatabaseName);

            // Act
            bool isDatabaseCreated = false;
            string? name = null;
            await using (SqlConnection connection = new(this.databaseHelper.masterConnectionString))
            {
                var records = await connection.QueryAsync(CHECK_IF_DATABASE_EXISTS_SQL_QUERY, parameters);
                isDatabaseCreated = records.Any();
                name = records.FirstOrDefault()?.name;
            }

            // Assert
            using (new AssertionScope())
            {
                isDatabaseCreated.Should().BeTrue();
                name.Should().Be(this.databaseHelper.integrationTestsDatabaseName);
            }
        }

        [Test]
        public async Task Create_DoesNotTryToCreateDatabaseMoreThanOnce()
        {
            // Arrange
            databaseHelper.CreateIntegrationTestsDatabase(); // We try to recreate the database

            DynamicParameters parameters = new();
            parameters.Add(DB_NAME_PARAMETER_NAME, databaseHelper.integrationTestsDatabaseName);

            // Act
            int recordsCount = 0;
            await using (SqlConnection connection = new(databaseHelper.masterConnectionString))
            {
                dynamic records = await connection.QueryAsync(CHECK_IF_DATABASE_EXISTS_SQL_QUERY, parameters);
                recordsCount = records.Count;
            }

            // Assert
            recordsCount.Should().Be(1);
        }
    }
}
