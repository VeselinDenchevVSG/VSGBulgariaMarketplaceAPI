using DotNetEnv;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

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

    builder.Services.AddHttpContextAccessor();

    builder.Services
        .AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            string clientId = Environment.GetEnvironmentVariable("AZURE_AD_CLIENT_ID");
            string tenantId = Environment.GetEnvironmentVariable("AZURE_AD_TENANT_ID");

            options.Authority = $"https://login.microsoftonline.com/{tenantId}/v2.0";
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidAudience = clientId,
                ValidateLifetime = true,
            };
        });

    builder.Services.AddAuthorization(options =>
    {
        string adminGroupId = Environment.GetEnvironmentVariable("AZURE_AD_ADMIN_GROUP_ID");

        options.AddPolicy("Admin", policy => policy.RequireClaim("groups", adminGroupId));
    });

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

    app.UseAuthentication();

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