namespace Test.IntegrationTests.Persistence.Migrations
{
    using CsvHelper.Configuration.Attributes;

    using Dapper;

    using FluentAssertions;
    using FluentAssertions.Execution;

    using Microsoft.Data.SqlClient;

    using System.Reflection;

    using Test.IntegrationTests.Helpers;

    using static Test.IntegrationTests.Constants.IntegrationTestsDatabaseConstant;
    using static Test.IntegrationTests.Constants.PathConstant;

    public class MigrateDatabaseTests
    {
        private readonly string databaseFolderPath;
        private readonly DatabaseHelper databaseHelper = new();
        private readonly MigrateDatabaseWebApplicationFactory factory = new();
        private HttpClient httpClient;

        public MigrateDatabaseTests()
        {
            databaseFolderPath = ConstructDatabaseFolderPath();
        }

        public sealed class TableColumnInfo
        {
            public string TableName { get; set; }

            public string ColumnName { get; set; }

            public string DataType { get; set; }

            [NullValues("NULL")]
            public string? ConstraintType { get; set; }

            [NullValues("NULL")]
            public string? ConstraintName { get; set; }
        }

        public sealed class VersionInfo
        {
            public long Version { get; set; }

            public string? Description { get; set; }
        }

        [SetUp]
        public void CreateIntegrationsTestsDatabaseAndHttpClient()
        {
            this.databaseHelper.CreateIntegrationTestsDatabase();
            this.httpClient = this.factory.CreateClient();
        }

        [TearDown]
        public async Task DisposeHttpClientAndDropIntegrationTestsDatabase()
        {
            this.httpClient.Dispose();
            await this.databaseHelper.DropIntegrationTestsDatabaseAsync();
        }

        [Test]
        public async Task MigrateUpDatabase_RunsMigrationsOnStartup_IfNotAlreadyRun()
        {
            // Arrange
            string filePath = ConstructFileInDatabaseFolderPath(TABLE_COLUMNS_CSV_FILE_NAME);
            var tableColumnsFromFile = await ReadCsvHelper.GetListFromCsvFileAsync<TableColumnInfo>(filePath, true);

            filePath = ConstructFileInDatabaseFolderPath(VERSION_INFO_CSV_FILE_NAME);
            var versionInfoFromFile = await ReadCsvHelper.GetListFromCsvFileAsync<VersionInfo>(filePath, true);

            // Act
            string tableColumnsSql = string.Format(SELECT_TABLE_COLUMNS_SQL_QUERY_TEMPLATE, databaseHelper.integrationTestsDatabaseName);

            List<TableColumnInfo> tableColumnsFromDatabaseInitial;
            List<VersionInfo> versionInfoFromDatabaseInitial;
            await using (SqlConnection connection = new(databaseHelper.integrationTestsConnectionString))
            {
                tableColumnsFromDatabaseInitial = (await connection.QueryAsync<TableColumnInfo>(tableColumnsSql)).ToList();
                versionInfoFromDatabaseInitial = (await connection.QueryAsync<VersionInfo>(SELECT_VERSION_INFO_SQL_QUERY)).ToList();
            }

            // We dispose the current http client so we can recreate it and test if migrations are already applied
            httpClient.Dispose();
            httpClient = factory.CreateClient();

            List<TableColumnInfo> tableColumnsFromDatabaseLatest;
            List<VersionInfo> versionInfoFromDatabaseLatest;
            await using (SqlConnection connection = new(databaseHelper.integrationTestsConnectionString))
            {
                tableColumnsFromDatabaseLatest = (await connection.QueryAsync<TableColumnInfo>(tableColumnsSql)).ToList();
                versionInfoFromDatabaseLatest = (await connection.QueryAsync<VersionInfo>(SELECT_VERSION_INFO_SQL_QUERY)).ToList();
            }

            // Assert
            using (new AssertionScope())
            {
                tableColumnsFromFile.Should().BeEquivalentTo(tableColumnsFromDatabaseInitial, options => options.WithStrictOrdering());
                tableColumnsFromDatabaseInitial.Should().BeEquivalentTo(tableColumnsFromDatabaseLatest, options => options.WithStrictOrdering());

                versionInfoFromFile.Should().BeEquivalentTo(versionInfoFromDatabaseInitial, options => options.WithStrictOrdering());
                versionInfoFromDatabaseInitial.Should().BeEquivalentTo(versionInfoFromDatabaseLatest, options => options.WithStrictOrdering());
            }
        }

        private string ConstructDatabaseFolderPath()
        {
            string assemblyFolderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;

            List<string> paths = new() { assemblyFolderPath };
            for (int i = 0; i < 4; i++)
            {
                paths.Add(NAVIGATE_UP);
            }
            paths.Add(TEST_INTERGRATION_TESTS_FOLDER_NAME);
            paths.Add(PERSISTENCE_FOLDER_NAME);
            paths.Add(MIGRATIONS_FOLDER_NAME);
            paths.Add(CSV_FILES_FOLDER_NAME);

            string databaseFolderPath = Path.Combine(paths.ToArray());
            string absoluteDatabaseFolderPath = Path.GetFullPath(databaseFolderPath);

            return absoluteDatabaseFolderPath;
        }

        private string ConstructFileInDatabaseFolderPath(string fileName) => Path.Combine(databaseFolderPath, fileName);
    }
}
