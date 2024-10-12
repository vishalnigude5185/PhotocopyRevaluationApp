using Serilog;
using Serilog.Context;

namespace PhotocopyRevaluationAppMVC.Logging
{
    public class LoggingHelper
    {
        private readonly ILogger<LoggingHelper> _logger;

        public LoggingHelper(ILogger<LoggingHelper> logger)
        {
            _logger = logger;
        }

        //Logging demo example
        public void LogActionDemo()
        {
            DateTime actionStartTime = DateTime.UtcNow;  // Capture when the action started
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();  // Start a stopwatch to measure duration

            try
            {
                // Simulate some operation
            }
            finally
            {
                stopwatch.Stop();  // Stop the stopwatch to get the elapsed time
                Log.Information($"Operation {actionStartTime} completed {stopwatch} successfully.");
            }
        }

        public void LogUserActivity(
            int userId = 0,  // Default value for userId if not provided
            string ipAddress = null,  // Optional parameter with default value
            string correlationId = null,  // Optional parameter with default value
            string context = null,  // Optional parameter with default value
            string location = null,  // Optional parameter with default value
            string customData = null,  // Optional parameter with default value
            string action = null,  // Optional parameter with default value
            DateTime actionTime = default,  // Default value for DateTime (it defaults to 0001-01-01T00:00:00)
            TimeSpan milliseconds = default  // Default value for TimeSpan
            )
        {
            // Example of using defaults
            actionTime = actionTime == default ? DateTime.UtcNow : actionTime;  // Set current time if not provided

            // Log the message with enriched properties
            using (LogContext.PushProperty("UserId", userId))
            using (LogContext.PushProperty("IpAddress", ipAddress ?? "Unknown IP"))  // If null, set to default
            using (LogContext.PushProperty("CorrelationId", correlationId ?? "No Correlation"))
            using (LogContext.PushProperty("Context", context ?? "General"))
            using (LogContext.PushProperty("Location", location ?? "Unknown Location"))
            using (LogContext.PushProperty("CustomData", customData ?? "No Custom Data"))
            using (LogContext.PushProperty("Action", action ?? "No Action"))
            using (LogContext.PushProperty("CreatedAt", actionTime))
            using (LogContext.PushProperty("DurationMs", milliseconds.TotalMilliseconds))
            {
                // Log the activity
                Log.Information("User activity logged. UserId: {UserId}, Action: {Action}, IP: {IpAddress}, Duration: {DurationMs} ms", userId, action, ipAddress, milliseconds.TotalMilliseconds);
            }
        }
    }
}
