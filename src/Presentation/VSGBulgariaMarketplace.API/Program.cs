using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using NLog;
using NLog.Web;

using System.Reflection;

using VSGBulgariaMarketplace.API.Constants;
using VSGBulgariaMarketplace.Application.Constants;
using VSGBulgariaMarketplace.Application.Helpers.Configurations;
using VSGBulgariaMarketplace.Application.Helpers.Middlewares;
using VSGBulgariaMarketplace.Persistence.Configurations;
using VSGBulgariaMarketplace.Persistence.Migrations;

var logger = LogManager.Setup().LoadConfigurationFromAssemblyResource(Assembly.GetEntryAssembly(), NLogConstant.NLOG_CONFIG_FILE_NAME)
                                .GetCurrentClassLogger();
try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Configuration.AddJsonFile(BuilderConstant.BUILDER_CONFIGURATION_APP_SETTINGS_JSON_FILE_NAME, optional: false, reloadOnChange: true)
                         .AddJsonFile(string.Format(BuilderConstant.BUILDER_CONFIGURATION_APP_SETTINGS_ENVIRONMENT_JSON_FILE_NAME_TEMPLATE, 
                                                            builder.Environment.EnvironmentName), optional: true, reloadOnChange: true)
                         .AddEnvironmentVariables();

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc(BuilderConstant.SWAGGER_GEN_SWAGGER_DOC_VERSION_NAME, new OpenApiInfo 
        { 
            Title = BuilderConstant.SWAGGER_GEN_SWAGGER_DOC_TITLE, 
            Version = BuilderConstant.SWAGGER_GEN_SWAGGER_DOC_VERSION_NAME 
        });

        var securitySchema = new OpenApiSecurityScheme
        {
            Description = AuthorizationConstant.SWAGGER_GEN_OPEN_API_SECURITY_SCHEMA_DESCRIPTION,
            Name = AuthorizationConstant.SWAGGER_GEN_OPEN_API_SECURITY_SCHEMA_NAME,
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = AuthorizationConstant.OPEN_API_SECURITY_SCHEMA_SCHEME_NAME,
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = AuthorizationConstant.OPEN_API_SECURITY_SCHEMA_REFERENCE_ID
            }
        };
        c.AddSecurityDefinition(AuthorizationConstant.SWAGGER_GEN_SECURITY_DEFINITION_NAME, securitySchema);
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                securitySchema, 
                new[] { AuthorizationConstant.SWAGGER_GEN_SECURITY_REQUIREMENT_NAME } 
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
            string clientId = builder.Configuration[AuthorizationConstant.AZURE_AD_CONFIGURATION_CLIENT_ID];
            string tenantId = builder.Configuration[AuthorizationConstant.AZURE_AD_CONFIGURATION_TENANT_ID];

            options.Authority = string.Format(AuthorizationConstant.JWT_BEARER_AUTHORITY_TEMPLATE, tenantId);
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
        string adminGroupId = builder.Configuration[AuthorizationConstant.CONFIGURATION_AZURE_AD_ADMIN_GROUP_ID];

        options.AddPolicy(AuthorizationConstant.AUTHORIZATION_ADMIN_POLICY_NAME, policy => policy.RequireClaim(AuthorizationConstant.GROUPS_CLAIM_TYPE_NAME,
                                                                                                                                 adminGroupId));
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
    logger.Error(exception, NLogConstant.NLOG_GLOBAL_ERROR_MESSAGE);
    throw;
}
finally
{
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    LogManager.Shutdown();
}