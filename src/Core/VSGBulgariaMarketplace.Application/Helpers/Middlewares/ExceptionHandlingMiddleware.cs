namespace VSGBulgariaMarketplace.Application.Helpers.Middlewares
{
    using FluentValidation;

    using Microsoft.AspNetCore.Http;
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Logging;

    using System.Text.Json;

    using VSGBulgariaMarketplace.Application.Models.Exceptions;
    using VSGBulgariaMarketplace.Application.Models.UnitOfWork;

    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            this.logger = logger;
            this.next = next;
        }

        public async Task Invoke(HttpContext httpContext, IUnitOfWork unitOfWork)
        {
            try
            {
                unitOfWork.Begin();
                await this.next.Invoke(httpContext);
                unitOfWork.Commit();
            }
            catch (Exception e)
            {
                unitOfWork.Rollback();
                this.logger.LogError(e, e.Message);
                await ExceptionHandler(e, httpContext);
            }
        }

        private async Task ExceptionHandler(Exception exception, HttpContext httpContext)
        {
            List<ErrorModel> errors = this.GenerateErrors(exception, httpContext);

            var errorResponce = JsonSerializer.Serialize(errors);
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = errors[0].Code;
            await httpContext.Response.WriteAsync(errorResponce);
        }

        private List<ErrorModel> GenerateErrors(Exception exception, HttpContext httpContext)
        {
            List<ErrorModel> errors = new List<ErrorModel>();

            switch (exception)
            {
                case HttpException httpException:
                    errors.Add(new ErrorModel { Code = (int)httpException.StatusCode, ErrorMessage = httpException.Message });
                    break;

                case ValidationException validationException:
                    errors.AddRange(validationException.Errors.Select(e => new ErrorModel { Code = 400, ErrorMessage = e.ErrorMessage }));
                    break;

                case NotFoundException notFoundException:
                    errors.Add(new ErrorModel { Code = 404, ErrorMessage = notFoundException.Message });
                    break;

                case FileNotFoundException fileNotFoundException:
                    errors.Add(new ErrorModel { Code = 404, ErrorMessage = fileNotFoundException.Message });
                    break;

                case PrimaryKeyViolationException primaryKeyViolationException:
                    errors.Add(new ErrorModel { Code = 500, ErrorMessage = primaryKeyViolationException.Message });
                    break;

                case SqlException sqlException:
                    ErrorModel errorModel = new ErrorModel { Code = 500 };
                    errors.Add(new ErrorModel { Code = 500, ErrorMessage = "Database error!" });
                    break;

                default:
                    errors.Add(new ErrorModel { Code = 500, ErrorMessage = exception.Message });
                    break;
            }

            return errors;
        }
    }
}