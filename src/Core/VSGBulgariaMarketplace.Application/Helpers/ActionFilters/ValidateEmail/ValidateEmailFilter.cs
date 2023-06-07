namespace VSGBulgariaMarketplace.Application.Helpers.ActionFilters.ValidateEmail
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Configuration;

    public class ValidateEmailFilter : IActionFilter
    {
        private readonly IConfiguration configuration;

        public ValidateEmailFilter(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            bool isAdmin = context.HttpContext.User.Claims.Any(x => x.Value == configuration["AzureAd:AdminGroupId"]);
            if (!isAdmin)
            {
                var user = context.HttpContext.User;
                var routeEmail = context.RouteData.Values["email"]?.ToString()?.ToLower();

                if (routeEmail != null && user.Identity?.IsAuthenticated == true)
                {
                    var tokenEmail = user.FindFirst(c => c.Type == "preferred_username")?.Value.ToLower();

                    if (routeEmail != tokenEmail)
                    {
                        context.HttpContext.Response.StatusCode = 403;
                        context.HttpContext.Response.WriteAsync("Email from token doesn't match email from route parameter.");
                        context.Result = new ContentResult();
                    }
                }
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Do nothing
        }
    }
}