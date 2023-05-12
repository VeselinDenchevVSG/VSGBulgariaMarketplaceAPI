using DotNetEnv;

using VSGBulgariaMarketplace.Application.Helpers.Configurations;
using VSGBulgariaMarketplace.Persistence.Configurations;
using VSGBulgariaMarketplace.Persistence.Migrations;

var builder = WebApplication.CreateBuilder(args);

Env.Load(Directory.GetCurrentDirectory() +"\\.env"); // Load enviromental variables

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                     .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                     .AddEnvironmentVariables();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplicationLayerConfiguration();
builder.Services.AddRepositoriesConfiguration();
builder.Services.AddMigrationsConfiguration();

builder.Services.AddMemoryCache();

var app = builder.Build();

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

app.MapControllers();

app.Run();
