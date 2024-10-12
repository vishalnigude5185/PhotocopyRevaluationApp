using System.Collections.Concurrent;
using System.Text.Json;

using PhotocopyRevaluationAppMVC.Data;
using PhotocopyRevaluationAppMVC.Models;

namespace PhotocopyRevaluationAppMVC.Logging
{
    public class DatabaseLogger : ILogger
    {
        private readonly LoggingContext _context;

        private readonly IHttpContextAccessor _httpContextAccessor;  // Access HTTP context for user and request details.
        private static readonly ConcurrentQueue<Log> _logQueue = new ConcurrentQueue<Log>();  // Queue for batching logs.
        private static readonly TimeSpan _flushInterval = TimeSpan.FromSeconds(5);  // Interval for batch flushing logs.
        private static bool _flushingInProgress = false;

        public DatabaseLogger(LoggingContext context)
        {
            _context = context;
        }
        public DatabaseLogger(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public IDisposable BeginScope<TState>(TState state) => null;

        public bool IsEnabled(LogLevel logLevel)
        {
            // Only log events above the Information level (adjust based on your needs)
            return logLevel >= LogLevel.Information;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel)) return;

            // Create log entry
            var logEntry = new Log
            {
                Timestamp = DateTime.UtcNow,
                Level = logLevel.ToString(),
                Message = formatter(state, exception),
                Exception = exception?.ToString(),
                Context = GetContextDetails(),  // Include user and request details
                CorrelationId = GetCorrelationId(),
                IpAddress = GetIpAddress(),
                CreatedAt = DateTime.UtcNow
            };

            // Add the log entry to the queue for batching
            _logQueue.Enqueue(logEntry);

            // Flush logs asynchronously if no flush is in progress
            if (!_flushingInProgress)
            {
                Task.Run(async () => await FlushLogsAsync());
            }
        }
        private string GetContextDetails()
        {
            // Capture user information and other context details (useful for debugging)
            var httpContext = _httpContextAccessor.HttpContext;
            var userId = httpContext?.User?.Identity?.Name ?? "Anonymous";
            var userAgent = httpContext?.Request?.Headers["User-Agent"].ToString();

            // Create structured context data (e.g., as a JSON string)
            var contextData = new
            {
                UserId = userId,
                UserAgent = userAgent,
                RequestPath = httpContext?.Request?.Path.ToString(),
                QueryString = httpContext?.Request?.QueryString.ToString()
            };

            return JsonSerializer.Serialize(contextData);
        }

        private string GetCorrelationId()
        {
            // Return a correlation ID (useful for tracking requests across services)
            return _httpContextAccessor.HttpContext?.TraceIdentifier ?? Guid.NewGuid().ToString();
        }

        private string GetIpAddress()
        {
            // Return the user's IP address
            return _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
        }

        private async Task FlushLogsAsync()
        {
            try
            {
                _flushingInProgress = true;
                while (_logQueue.TryDequeue(out var logEntry))
                {
                    _context.Logs.Add(logEntry);
                }

                await _context.SaveChangesAsync();
            }
            finally
            {
                _flushingInProgress = false;
            }
        }
    }
}

