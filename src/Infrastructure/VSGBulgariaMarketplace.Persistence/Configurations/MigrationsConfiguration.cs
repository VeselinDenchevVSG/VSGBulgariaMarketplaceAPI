namespace VSGBulgariaMarketplace.Persistence.Configurations
{
    using FluentMigrator.Runner;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;

    using NLog.Extensions.Logging;

    using System.Reflection;

    using VSGBulgariaMarketplace.Persistence.Constants;

    public static class MigrationsConfiguration
    {
        public static IServiceCollection AddMigrationsConfiguration(this IServiceCollection services)
        {
            services.AddFluentMigratorCore()
                    .AddLogging(l => l.AddNLog())
                    .ConfigureRunner(c => c.AddSqlServer().WithGlobalConnectionString(DatabaseConnectionConstant.DEFAULT_CONNECTION_STRING_NAME)
                                                            .ScanIn(Assembly.GetExecutingAssembly()).For.Migrations());

            return services;
        }
        public static void MigrateUpDatabase(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var migrator = scope.ServiceProvider.GetService<IMigrationRunner>();
            migrator.MigrateUp();
        }
    }
}
