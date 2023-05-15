namespace VSGBulgariaMarketplace.Application.Helpers.Configurations
{
    using FluentValidation.AspNetCore;

    using Microsoft.Extensions.DependencyInjection;

    using System.Reflection;

    using VSGBulgariaMarketplace.Application.Models.Item.Interfaces;
    using VSGBulgariaMarketplace.Application.Models.Order.Interfaces;
    using VSGBulgariaMarketplace.Application.Services;
    using VSGBulgariaMarketplace.Application.Services.HelpServices.Cache;
    using VSGBulgariaMarketplace.Application.Services.HelpServices.Cache.Interfaces;
    using VSGBulgariaMarketplace.Application.Services.HelpServices.Image;
    using VSGBulgariaMarketplace.Application.Services.HelpServices.Image.Interfaces;

    public static class ApplicationLayerConfiguration
    {
        public static IServiceCollection AddApplicationLayerConfiguration(this IServiceCollection services)
        {
            services.AddControllers()
                    .AddFluentValidation(options =>
                    {
                        // Validate child properties and root collection elements
                        options.ImplicitlyValidateChildProperties = true;
                        options.ImplicitlyValidateRootCollectionElements = true;

                        // Automatic registration of validators in assembly
                        options.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly());
                    });

            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddScoped<IItemService, ItemService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IImageCloudService, ImageCloudinaryService>();

            services.AddSingleton<IMemoryCacheAdapter, MemoryCacheAdapter>();

            return services;
        }
    }
}
