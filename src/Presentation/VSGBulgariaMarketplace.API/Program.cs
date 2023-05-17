using DotNetEnv;

using NLog;
using NLog.Web;

using System.Reflection;

using VSGBulgariaMarketplace.Application.Helpers.Configurations;
using VSGBulgariaMarketplace.Application.Helpers.Middlewares;
using VSGBulgariaMarketplace.Persistence.Configurations;
using VSGBulgariaMarketplace.Persistence.Migrations;

var logger = LogManager.Setup().LoadConfigurationFromAssemblyResource(Assembly.GetEntryAssembly(), "nlog.config").GetCurrentClassLogger();
try
{
    var builder = WebApplication.CreateBuilder(args);

    Env.Load(Directory.GetCurrentDirectory() + "\\.env"); // Load enviromental variables

    builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                         .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                         .AddEnvironmentVariables();

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddApplicationLayerConfiguration();
    builder.Services.AddRepositoriesConfiguration();
    builder.Services.AddMigrationsConfiguration();

    builder.Services.AddMemoryCache();

    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    var app = builder.Build();

    app.UseCors(builder =>
    {
        builder.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
    });

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    DatabaseCreator.Create(builder.Configuration);

    app.MigrateUpDatabase();

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.UseMiddleware<ExceptionHandlingMiddleware>();

    app.MapControllers();

    app.Run();
}
catch (Exception exception)
{
    // NLog: catch setup errors
    logger.Error(exception, "Stopped program because of exception");
    throw;
}
finally
{
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    LogManager.Shutdown();
}