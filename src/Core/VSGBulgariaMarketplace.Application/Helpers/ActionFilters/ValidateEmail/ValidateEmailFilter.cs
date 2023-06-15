namespace VSGBulgariaMarketplace.Application.Helpers.ActionFilters.ValidateEmail
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Configuration;

    using VSGBulgariaMarketplace.Application.Constants;

    public class ValidateEmailFilter : IActionFilter
    {
        private readonly IConfiguration configuration;

        public ValidateEmailFilter(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            bool isAdmin = context.HttpContext.User.Claims.Any(x => x.Value == this.configuration[AuthorizationConstant.CONFIGURATION_AZURE_AD_ADMIN_GROUP_ID]);
            if (!isAdmin)
            {
                var user = context.HttpContext.User;
                var routeEmail = context.RouteData.Values[AuthorizationConstant.EMAIL_ROUTE_DATA_KEY_NAME]?.ToString()?.ToLower();

                if (routeEmail != null && user.Identity?.IsAuthenticated == true)
                {
                    var tokenEmail = user.FindFirst(c => c.Type == AuthorizationConstant.PREFERRED_USERNAME_CLAIM_NAME)?.Value.ToLower();

                    if (routeEmail != tokenEmail)
                    {
                        context.HttpContext.Response.StatusCode = 403;
                        context.HttpContext.Response.WriteAsync(AuthorizationConstant.EMAIL_FROM_TOKEN_DOES_NOT_MATCH_EMAIL_FROM_ROUTE_PARAMETER_ERROR_MESSAGE);
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