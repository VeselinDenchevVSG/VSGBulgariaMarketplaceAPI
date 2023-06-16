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
            httpContext.Response.StatusCode = (int) errors[0].Code;
            await httpContext.Response.WriteAsync(errorResponce);
        }

        private List<ErrorModel> GenerateErrors(Exception exception)
        {
            List<ErrorModel> errors = new List<ErrorModel>();

            switch (exception)
            {
                case HttpException httpException:
                    errors.Add(new ErrorModel((int) httpException.StatusCode, httpException.Message));
                    break;

                case ValidationException validationException:
                    errors.AddRange(validationException.Errors.Select(e => new ErrorModel(StatusCodes.Status400BadRequest, e.ErrorMessage)));
                    break;

                case NotFoundException notFoundException:
                    errors.Add(new ErrorModel(StatusCodes.Status404NotFound, notFoundException.Message));
                    break;

                case FileNotFoundException fileNotFoundException:
                    errors.Add(new ErrorModel(StatusCodes.Status404NotFound,  fileNotFoundException.Message));
                    break;

                case EntityAlreadyExistsException itemAlreadyExistsException:
                    errors.Add(new ErrorModel(StatusCodes.Status422UnprocessableEntity, itemAlreadyExistsException.Message));
                    break;

                case SqlException sqlException:
                    errors.Add(new ErrorModel(StatusCodes.Status500InternalServerError,  sqlException.Message));
                    break;

                default:
                    errors.Add(new ErrorModel(StatusCodes.Status500InternalServerError, exception.Message));
                    break;
            }

            return errors;
        }
    }
}