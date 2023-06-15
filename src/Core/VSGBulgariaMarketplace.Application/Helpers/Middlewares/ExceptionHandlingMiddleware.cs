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
        private const short HTTP_STATUS_CODE_BAD_REQUEST = 400;
        private const short HTTP_STATUS_CODE_NOT_FOUND = 404;
        private const short HTTP_STATUS_CODE_UNPROCESSABLE_CONTENT = 422;
        private const short HTTP_STATUS_CODE_INTERNAL_SERVER_ERROR = 500;

        private const string APPLICATION_JSON_CONTENT_TYPE = "application/json";

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
            List<ErrorModel> errors = this.GenerateErrors(exception);

            var errorResponce = JsonSerializer.Serialize(errors);
            httpContext.Response.ContentType = APPLICATION_JSON_CONTENT_TYPE;
            httpContext.Response.StatusCode = errors[0].Code;
            await httpContext.Response.WriteAsync(errorResponce);
        }

        private List<ErrorModel> GenerateErrors(Exception exception)
        {
            List<ErrorModel> errors = new List<ErrorModel>();

            switch (exception)
            {
                case HttpException httpException:
                    errors.Add(new ErrorModel { Code = (short) httpException.StatusCode, ErrorMessage = httpException.Message });
                    break;

                case ValidationException validationException:
                    errors.AddRange(validationException.Errors.Select(e => new ErrorModel { Code = HTTP_STATUS_CODE_BAD_REQUEST, ErrorMessage = e.ErrorMessage }));
                    break;

                case NotFoundException notFoundException:
                    errors.Add(new ErrorModel { Code = HTTP_STATUS_CODE_NOT_FOUND, ErrorMessage = notFoundException.Message });
                    break;

                case FileNotFoundException fileNotFoundException:
                    errors.Add(new ErrorModel { Code = HTTP_STATUS_CODE_NOT_FOUND, ErrorMessage = fileNotFoundException.Message });
                    break;

                case EntityAlreadyExistsException itemAlreadyExistsException:
                    errors.Add(new ErrorModel { Code = HTTP_STATUS_CODE_UNPROCESSABLE_CONTENT, ErrorMessage = itemAlreadyExistsException.Message });
                    break;

                case SqlException sqlException:
                    errors.Add(new ErrorModel { Code = HTTP_STATUS_CODE_INTERNAL_SERVER_ERROR, ErrorMessage = sqlException.Message });
                    break;

                default:
                    errors.Add(new ErrorModel { Code = HTTP_STATUS_CODE_INTERNAL_SERVER_ERROR, ErrorMessage = exception.Message });
                    break;
            }

            return errors;
        }
    }
}