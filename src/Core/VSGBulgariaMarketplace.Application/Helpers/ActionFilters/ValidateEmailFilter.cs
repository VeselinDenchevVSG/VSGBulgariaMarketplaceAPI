namespace VSGBulgariaMarketplace.Application.Helpers.ActionFilters
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    public class ValidateEmailFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
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

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Do nothing
        }
    }
}