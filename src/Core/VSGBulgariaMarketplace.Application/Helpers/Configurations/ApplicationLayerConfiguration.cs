namespace VSGBulgariaMarketplace.Application.Helpers.Configurations
{
    using FluentValidation.AspNetCore;

    using Microsoft.Extensions.DependencyInjection;

    using System.Reflection;

    using VSGBulgariaMarketplace.Application.Helpers.ActionFilters;
    using VSGBulgariaMarketplace.Application.Models.Image.Interfaces;
    using VSGBulgariaMarketplace.Application.Models.Item.Interfaces;
    using VSGBulgariaMarketplace.Application.Models.ItemLoan.Interfaces;
    using VSGBulgariaMarketplace.Application.Models.Order.Interfaces;
    using VSGBulgariaMarketplace.Application.Services;
    using VSGBulgariaMarketplace.Application.Services.HelpServices.Cache;
    using VSGBulgariaMarketplace.Application.Services.HelpServices.Cache.Interfaces;

    public static class ApplicationLayerConfiguration
    {
        public static IServiceCollection AddApplicationLayerConfiguration(this IServiceCollection services)
        {
            services.AddControllers(options => 
            {
                options.Filters.Add(new ValidateEmailFilter());
            })
                    .AddFluentValidation(options =>
                    {
                        // Validate child properties and root collection elements
                        options.ImplicitlyValidateChildProperties = true;
                        options.ImplicitlyValidateRootCollectionElements = true;

                        // Automatic registration of validators in assembly
                        options.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly());
                    });

            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddScoped<ValidateEmailFilter>();

            services.AddScoped<IItemService, ItemService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IItemLoanService, ItemLoanService>();

            services.AddScoped<ICloudImageService, CloudinaryImageService>();

            services.AddSingleton<IMemoryCacheAdapter, MemoryCacheAdapter>();

            return services;
        }
    }
}
