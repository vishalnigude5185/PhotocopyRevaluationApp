using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Logging;
    using System;
namespace PhotocopyRevaluationAppMVC.Models.Attributes_Validations
{
    //Global Exception Handler Attribute
    //This custom attribute handles exceptions thrown by actions and logs them, returning a user-friendly message to the client.
    public class GlobalExceptionHandlerAttribute : ExceptionFilterAttribute
    {
        private readonly ILogger<GlobalExceptionHandlerAttribute> _logger;

        public GlobalExceptionHandlerAttribute(ILogger<GlobalExceptionHandlerAttribute> logger)
        {
            _logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            _logger.LogError(context.Exception, "An error occurred.");
            context.Result = new JsonResult(new { error = "An unexpected error occurred. Please try again later." })
            {
                StatusCode = 500
            };
        }
    }
}
