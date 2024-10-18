using SendGrid.Helpers.Errors.Model;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace PhotocopyRevaluationAppMVC.NewFolder
{
    // File: Middleware/ExceptionHandlingMiddleware.cs
    namespace YourAppNamespace.Middleware
    {
        public class ExceptionHandlingMiddleware
        {
            private readonly RequestDelegate _next;
            private readonly ILogger<ExceptionHandlingMiddleware> _logger;

            public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
            {
                _next = next;
                _logger = logger;
            }
            public async Task Invoke(HttpContext context)
            {
                try
                {
                    await _next(context);
                }
                catch (Exception ex)
                {
                    await HandleExceptionAsync(context, ex);
                }
            }

            private Task HandleExceptionAsync(HttpContext context, Exception ex)
            {
                // Log the exception details
                _logger.LogError(ex, "An unhandled exception has occurred: {Message}", ex.Message);

                // Set the response status code and content type
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                // Create a response object
                var result = new
                {
                    message = "An unexpected error occurred. Please try again later.",
                    statusCode = context.Response.StatusCode
                };

                return context.Response.WriteAsJsonAsync(result);

                //context.Response.ContentType = "application/json";
                //var response = new ErrorResponse();

                //switch (ex)
                //{
                //    case NotFoundException notFoundException:
                //        context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                //        response = new ErrorResponse { Message = notFoundException.Message, StatusCode = HttpStatusCode.NotFound };
                //        break;

                //    case ValidationException validationException:
                //        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                //        response = new ErrorResponse { Message = validationException.Message, StatusCode = HttpStatusCode.BadRequest };
                //        break;

                //    case UnauthorizedAccessException unauthorizedAccessException:
                //        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                //        response = new ErrorResponse { Message = unauthorizedAccessException.Message, StatusCode = HttpStatusCode.Unauthorized };
                //        break;

                //    default:
                //        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                //        response = new ErrorResponse { Message = "An unexpected error occurred. Please try again later.", StatusCode = HttpStatusCode.InternalServerError };
                //        _logger.LogError(ex, "An unexpected error occurred.");
                //        break;
                //}

                //return context.Response.WriteAsJsonAsync(response);
            }

            private class ErrorResponse
            {
                public string Message { get; set; }
                public HttpStatusCode StatusCode { get; set; }
            }
        }
    }
}
