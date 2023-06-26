namespace VSGBulgariaMarketplace.Application.Helpers.Middlewares
{
    using FluentValidation;

    using Microsoft.AspNetCore.Http;
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Logging;

    using System;
    using System.Text.Json;

    using VSGBulgariaMarketplace.Application.Constants;
    using VSGBulgariaMarketplace.Application.Models.Exceptions;
    using VSGBulgariaMarketplace.Application.Models.UnitOfWork;

    public class ExceptionHandlingMiddleware
    {
        private const string OPERATION_CANCELLED_BY_USER_EXCEPTION_MESSAGE = "Operation cancelled by user.";

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

            if (errors.Count > 0)
            {
                var errorResponce = JsonSerializer.Serialize(errors);
                httpContext.Response.ContentType = JsonConstant.APPLICATION_JSON_CONTENT_TYPE;
                httpContext.Response.StatusCode = (int)errors[0].Code;
                await httpContext.Response.WriteAsync(errorResponce);
            }
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

                case OperationCanceledException:
                case InvalidOperationException invalidOperationException when invalidOperationException.Message == OPERATION_CANCELLED_BY_USER_EXCEPTION_MESSAGE:
                    // Do nothing
                    break;

                default:
                    errors.Add(new ErrorModel(StatusCodes.Status500InternalServerError, exception.Message));
                    break;
            }

            return errors;
        }
    }
}