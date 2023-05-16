namespace VSGBulgariaMarketplace.Persistence.Configurations
{
    using FluentMigrator.Runner;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;

    using System.Reflection;

    public static class MigrationsConfiguration
    {
        public static IServiceCollection AddMigrationsConfiguration(this IServiceCollection services)
        {
            services
                .AddFluentMigratorCore()
                .ConfigureRunner(c => c.AddSqlServer()
                .WithGlobalConnectionString(Environment.GetEnvironmentVariable("DEFAULT_CONNECTION_STRING"))
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
