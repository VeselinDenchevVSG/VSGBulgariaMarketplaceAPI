namespace VSGBulgariaMarketplace.Application.Helpers.Configurations
{
    using Microsoft.Extensions.DependencyInjection;

    using System.Reflection;

    using VSGBulgariaMarketplace.Application.Models.Item.Interfaces;
    using VSGBulgariaMarketplace.Application.Models.Order.Interfaces;
    using VSGBulgariaMarketplace.Application.Services;
    using VSGBulgariaMarketplace.Application.Services.HelpServices;
    using VSGBulgariaMarketplace.Application.Services.HelpServices.Interfaces;

    public static class ApplicationLayerConfiguration
    {
        public static IServiceCollection AddApplicationLayerConfiguration(this IServiceCollection services)
        {
            //services.AddControllers()
            //    .AddFluentValidation(validator => validator.RegisterValidatorsFromAssemblyContaining<AnswerDtoValidator>());

            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddScoped<IItemService, ItemService>();
            services.AddScoped<IOrderService, OrderService>();

            services.AddSingleton<IMemoryCacheAdapter, MemoryCacheAdapter>();

            return services;
        }
    }
}
