namespace Test.IntegrationTests.Persistence.Repositories
{
    using Test.IntegrationTests.Helpers;

    [SetUpFixture]
    internal class RepositoriesTestsSetUpFixture
    {
        private readonly DatabaseHelper databaseHelper = new();

        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            this.databaseHelper.CreateIntegrationTestsDatabase();
            this.databaseHelper.RunMigrations();
        }

        [OneTimeTearDown]
        public async Task RunAfterAllTests()
            => await this.databaseHelper.DropIntegrationTestsDatabaseAsync();
    }
}
