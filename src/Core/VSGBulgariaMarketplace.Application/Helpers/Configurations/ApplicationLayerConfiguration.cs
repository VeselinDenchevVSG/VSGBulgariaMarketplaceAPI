namespace VSGBulgariaMarketplace.Application.Helpers.Configurations
{
    using Microsoft.Extensions.DependencyInjection;

    using System.Reflection;

    using VSGBulgariaMarketplace.Application.Models.Item.Interfaces;
    using VSGBulgariaMarketplace.Application.Services;

    public static class ApplicationLayerConfiguration
    {
        public static IServiceCollection AddApplicationLayerConfiguration(this IServiceCollection services)
        {
            //services.AddControllers()
            //    .AddFluentValidation(validator => validator.RegisterValidatorsFromAssemblyContaining<AnswerDtoValidator>());

            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddScoped<IItemService, ItemService>();

            return services;
        }
    }
}
