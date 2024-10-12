using Serilog.Context;
using System.Security.Claims;

namespace PhotocopyRevaluationAppMVC.Middlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggingMiddleware> _logger;

        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            string? userId = "Anonymous";
            string action = "Anonymous";
            // Capture the start time of the request
            DateTime createdAt = DateTime.UtcNow;
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                // Extract values like IP address and Correlation ID from HttpContext
                string? ipAddress = context.Connection.RemoteIpAddress?.ToString();
                string correlationId = context.TraceIdentifier; // TraceIdentifier is used as Correlation ID in ASP.NET Core
                // Extract user information from the context (assuming JWT or Identity is being used)
                bool IsAuthenticated = context.User.Identity.IsAuthenticated;
                if (IsAuthenticated)
                {
                    userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value.ToString();
                }
                // Example: You can extract 'Location' from request path, headers, etc.
                string location = context.Request.Path;  // Could be more sophisticated
                // 'Context' and 'Action' could be inferred from the controller and action being invoked
                action = context.Request.Method + " " + context.Request.Path;
                string logContext = "WebRequest"; // Static or dynamic based on your app logic

                // Push these values into Serilog's LogContext
                // Push properties to the log context for the entire request
                using (LogContext.PushProperty("UserId", userId))
                using (LogContext.PushProperty("IpAddress", ipAddress))
                using (LogContext.PushProperty("CorrelationId", correlationId))
                using (LogContext.PushProperty("Context", logContext))
                using (LogContext.PushProperty("Location", location))
                using (LogContext.PushProperty("Action", action))
                using (LogContext.PushProperty("CreatedAt", createdAt))
                {
                    // Continue processing the request
                    await _next(context);

                    // After the request is completed, log the duration
                    stopwatch.Stop();
                    long durationMs = stopwatch.ElapsedMilliseconds;

                    // Push the duration into the log context
                    using (LogContext.PushProperty("DurationMs", durationMs))
                    {
                        _logger.LogInformation("Completed processing request for {UserId} with action {Action}, took {DurationMs} ms", userId, action, durationMs);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log errors with the same contextual information
                stopwatch.Stop();
                long durationMs = stopwatch.ElapsedMilliseconds;

                _logger.LogError(ex, "Error processing request for {UserId} with action {Action}, took {DurationMs} ms", userId, action, durationMs);
                throw;  // Re-throw the exception after logging
            }
        }
    }
}
