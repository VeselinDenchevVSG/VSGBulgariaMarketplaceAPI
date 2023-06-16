namespace VSGBulgariaMarketplace.Application.Helpers.ActionFilters.Validation
{
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.DependencyInjection;

    public class FormatValidationErrorMessagesFilterAttribute : Attribute, IFilterFactory, IOrderedFilter
    {
        public bool IsReusable => false;

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            return serviceProvider.GetRequiredService<FormatValidationErrorMessagesFilter>();
        }

        public int Order { get; set; } = 0; // change the order if needed
    }
}