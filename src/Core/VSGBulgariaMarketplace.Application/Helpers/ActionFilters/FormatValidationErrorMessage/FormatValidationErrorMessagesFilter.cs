namespace VSGBulgariaMarketplace.Application.Helpers.ActionFilters.Validation
{
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Mvc;

    using VSGBulgariaMarketplace.Application.Models.Exceptions;
    using Microsoft.AspNetCore.Http;
    using System.Text.Json;
    using VSGBulgariaMarketplace.Application.Constants;

    public class FormatValidationErrorMessagesFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                List<ErrorModel> errors = new List<ErrorModel>();

                foreach (var modelStateValues in context.ModelState.Values)
                {
                    foreach (var error in modelStateValues.Errors)
                    {
                        errors.Add(new ErrorModel(StatusCodes.Status400BadRequest, error.ErrorMessage));
                    }
                }

                var errorResponce = JsonSerializer.Serialize(errors);

                context.Result = new ContentResult()
                {
                    Content = errorResponce,
                    ContentType = JsonConstant.APPLICATION_JSON_CONTENT_TYPE,
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Not needed for validation filter
        }
    }
}