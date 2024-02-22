using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using NLog;
using NLog.Web;

using System.Reflection;

using VSGBulgariaMarketplace.Application.Helpers.Configurations;
using VSGBulgariaMarketplace.Application.Helpers.Middlewares;
using VSGBulgariaMarketplace.Persistence.Configurations;
using VSGBulgariaMarketplace.Persistence.Migrations;

using static VSGBulgariaMarketplace.API.Constants.NLogConstant;
using static VSGBulgariaMarketplace.API.Constants.BuilderConstant;
using static VSGBulgariaMarketplace.Application.Constants.AuthorizationConstant;

var logger = LogManager.Setup().LoadConfigurationFromAssemblyResource(Assembly.GetEntryAssembly(), NLOG_CONFIG_FILE_NAME)
                                .GetCurrentClassLogger();
try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Configuration.AddJsonFile(BUILDER_CONFIGURATION_APP_SETTINGS_JSON_FILE_NAME, optional: false, reloadOnChange: true)
                         .AddJsonFile(string.Format(BUILDER_CONFIGURATION_APP_SETTINGS_ENVIRONMENT_JSON_FILE_NAME_TEMPLATE, 
                                                            builder.Environment.EnvironmentName), optional: true, reloadOnChange: true)
                         .AddEnvironmentVariables();

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc(SWAGGER_GEN_SWAGGER_DOC_VERSION_NAME, new OpenApiInfo 
        { 
            Title = SWAGGER_GEN_SWAGGER_DOC_TITLE, 
            Version = SWAGGER_GEN_SWAGGER_DOC_VERSION_NAME 
        });

        var securitySchema = new OpenApiSecurityScheme
        {
            Description = SWAGGER_GEN_OPEN_API_SECURITY_SCHEMA_DESCRIPTION,
            Name = SWAGGER_GEN_OPEN_API_SECURITY_SCHEMA_NAME,
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = OPEN_API_SECURITY_SCHEMA_SCHEME_NAME,
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = OPEN_API_SECURITY_SCHEMA_REFERENCE_ID
            }
        };
        c.AddSecurityDefinition(SWAGGER_GEN_SECURITY_DEFINITION_NAME, securitySchema);
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                securitySchema, 
                new[] { SWAGGER_GEN_SECURITY_REQUIREMENT_NAME } 
            }
        });
    });

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
            string clientId = builder.Configuration[AZURE_AD_CONFIGURATION_CLIENT_ID];
            string tenantId = builder.Configuration[AZURE_AD_CONFIGURATION_TENANT_ID];

            options.Authority = string.Format(JWT_BEARER_AUTHORITY_TEMPLATE, tenantId);
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
        string adminGroupId = builder.Configuration[CONFIGURATION_AZURE_AD_ADMIN_GROUP_ID];

        options.AddPolicy(AUTHORIZATION_ADMIN_POLICY_NAME, policy => policy.RequireClaim(GROUPS_CLAIM_TYPE_NAME, adminGroupId));
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

    app.UseSwagger();
    app.UseSwaggerUI();

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
    logger.Error(exception, NLOG_GLOBAL_ERROR_MESSAGE);
    throw;
}
finally
{
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    LogManager.Shutdown();
}