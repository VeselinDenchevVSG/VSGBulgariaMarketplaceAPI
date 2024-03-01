namespace Test.IntegrationTests.Persistence.Migrations
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.Extensions.DependencyInjection;

    using VSGBulgariaMarketplace.Persistence.Configurations;

    using static Test.IntegrationTests.Constants.IntegrationTestsDatabaseConstant;

    internal class MigrateDatabaseWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {   // Example: Remove a specific service by type
                var serviceDescriptorsToRemove = services.Where(descriptor => descriptor.ServiceType.Namespace is not null &&
                                                                descriptor.ServiceType.Namespace.StartsWith(nameof(FluentMigrator)))
                                                         .ToList();

                foreach (ServiceDescriptor service in serviceDescriptorsToRemove)
                {
                    services.Remove(service);
                }

                // Override services for testing here if necessary
                services.AddMigrationsConfiguration(INTEGRATION_TESTS_CONNECTION_STRING_NAME);
            });
        }
    }
}
