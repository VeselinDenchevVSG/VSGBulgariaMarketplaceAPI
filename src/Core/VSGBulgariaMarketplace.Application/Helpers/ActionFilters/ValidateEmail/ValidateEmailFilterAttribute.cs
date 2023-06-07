namespace VSGBulgariaMarketplace.Application.Helpers.ActionFilters.ValidateEmail
{
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.DependencyInjection;

    public class ValidateEmailFilterAttribute : Attribute, IFilterFactory, IOrderedFilter
    {
        public bool IsReusable => false;

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            return serviceProvider.GetRequiredService<ValidateEmailFilter>();
        }

        public int Order { get; set; } = 0; // change the order if needed
    }
}
