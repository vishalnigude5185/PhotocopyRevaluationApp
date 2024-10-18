using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Logging;
    using System;
using Umbraco.Core.Models.Membership;
namespace PhotocopyRevaluationAppMVC.Models.Attributes_Validations
{
    //Logging Action Filter
    //This attribute is used to log the entry and exit points of an action in a controller.It can be useful for debugging or auditing purposes
    public class LogActionFilterAttribute : ActionFilterAttribute
    {
        private readonly ILogger<LogActionFilterAttribute> _logger;
        public LogActionFilterAttribute(ILogger<LogActionFilterAttribute> logger)
        {
            _logger = logger;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            _logger.LogInformation($"Starting action {context.ActionDescriptor.DisplayName} at {DateTime.Now}");
            base.OnActionExecuting(context);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            _logger.LogInformation($"Finished action {context.ActionDescriptor.DisplayName} at {DateTime.Now}");
            base.OnActionExecuted(context);
        }
    }

}
