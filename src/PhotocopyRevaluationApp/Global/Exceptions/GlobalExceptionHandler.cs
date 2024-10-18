namespace PhotocopyRevaluationApp.Global.Exceptions {
    using System;
    using System.Net;
    using System.Text.Json;
    using System.Threading.Tasks;
    // File: YourProject/Exceptions/GlobalExceptionHandler.cs
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;

    public class GlobalExceptionHandler : IGlobalExceptionHandler {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) {
            _logger = logger;
        }

        public async Task HandleExceptionAsync(HttpContext context, Exception exception) {
            // Log the exception
            _logger.LogError(exception, "An unhandled exception has occurred: {Message}", exception.Message);

            // Set the response details
            context.Response.ContentType = "application/json";

            // Determine the status code based on the exception type
            var response = new ErrorResponse {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Message = "An unexpected error occurred. Please try again later."
            };

            if (exception is NotFoundException) // Custom exception
            {
                response.StatusCode = (int)HttpStatusCode.NotFound;
                response.Message = exception.Message;
            }
            else if (exception is UnauthorizedAccessException) {
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                response.Message = "You are not authorized to access this resource.";
            }

            // Write the response
            context.Response.StatusCode = response.StatusCode;
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }

    public class ErrorResponse {
        public int StatusCode { get; set; }
        public string Message { get; set; }
    }

    // Custom exception for demonstration
    public class NotFoundException : Exception {
        public NotFoundException(string message) : base(message) { }
    }
}
