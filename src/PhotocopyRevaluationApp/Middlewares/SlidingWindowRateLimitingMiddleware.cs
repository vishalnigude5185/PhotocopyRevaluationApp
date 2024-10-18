// File: Middleware/SlidingWindowRateLimitingMiddleware.cs
using Microsoft.Extensions.Caching.Memory;

namespace PhotocopyRevaluationApp.Middlewares {
    public class SlidingWindowRateLimitingMiddleware {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;

        public SlidingWindowRateLimitingMiddleware(RequestDelegate next, IMemoryCache cache) {
            _next = next;
            _cache = cache;
        }

        public async Task Invoke(HttpContext context) {
            var ipAddress = context.Connection.RemoteIpAddress?.ToString();
            var key = $"SlidingRateLimit_{ipAddress}";

            // Maximum number of requests allowed
            const int limit = 5;
            // Time window in seconds
            const int timeWindow = 60;

            // Get the request timestamps
            if (!_cache.TryGetValue(key, out List<DateTime> timestamps)) {
                timestamps = new List<DateTime>();
            }

            // Remove timestamps outside the time window
            timestamps.RemoveAll(timestamp => timestamp < DateTime.UtcNow.AddSeconds(-timeWindow));

            if (timestamps.Count >= limit) {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests; // Too Many Requests
                await context.Response.WriteAsync("Rate limit exceeded. Please try again later.");
                return;
            }

            // Add the current timestamp
            timestamps.Add(DateTime.UtcNow);
            _cache.Set(key, timestamps, DateTimeOffset.UtcNow.AddSeconds(timeWindow));

            await _next(context);
        }
    }

}
