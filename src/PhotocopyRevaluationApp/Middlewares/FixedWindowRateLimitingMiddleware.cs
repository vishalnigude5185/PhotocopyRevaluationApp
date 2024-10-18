// File: Middleware/RateLimitingMiddleware.cs
using Microsoft.Extensions.Caching.Memory;

namespace PhotocopyRevaluationApp.Middlewares {
    public class FixedWindowRateLimitingMiddleware {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;

        public FixedWindowRateLimitingMiddleware(RequestDelegate next, IMemoryCache cache) {
            _next = next;
            _cache = cache;
        }

        public async Task Invoke(HttpContext context) {
            var ipAddress = context.Connection.RemoteIpAddress?.ToString();
            var key = $"RateLimit_{ipAddress}";
            var currentCount = _cache.Get<int>(key);
            const int limit = 5; // Maximum number of requests
            const int seconds = 60; // Time window in seconds

            if (currentCount >= limit) {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests; // Too Many Requests
                await context.Response.WriteAsync("Rate limit exceeded. Please try again later.");
                return;
            }

            // Increment the request count
            _cache.Set(key, currentCount + 1, DateTimeOffset.UtcNow.AddSeconds(seconds));

            await _next(context);
        }
    }
}
