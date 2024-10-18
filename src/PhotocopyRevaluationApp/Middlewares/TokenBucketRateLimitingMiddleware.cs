// File: Middleware/TokenBucketRateLimitingMiddleware.cs
using Microsoft.Extensions.Caching.Memory;

namespace PhotocopyRevaluationApp.Middlewares {
    public class TokenBucketRateLimitingMiddleware {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;

        public TokenBucketRateLimitingMiddleware(RequestDelegate next, IMemoryCache cache) {
            _next = next;
            _cache = cache;
        }

        public async Task Invoke(HttpContext context) {
            var ipAddress = context.Connection.RemoteIpAddress?.ToString();
            var key = $"TokenBucket_{ipAddress}";

            // Configuration parameters
            const int maxTokens = 5; // Maximum tokens in the bucket
            //const int refillTokens = 1; // Tokens added per interval
            const int refillIntervalInSeconds = 1; // Refill interval

            if (!_cache.TryGetValue(key, out TokenBucket? bucket)) {
                bucket = new TokenBucket(maxTokens, maxTokens, DateTime.UtcNow);
            }
            else if (bucket == null) {
                // Handle the case where bucket is still null
                // You could either throw an exception or initialize it again
                bucket = new TokenBucket(maxTokens, maxTokens, DateTime.UtcNow);
            }

            var elapsedTime = DateTime.UtcNow - bucket.LastRefillTime; // Now it's safe to access LastRefillTime

            var tokensToAdd = (int)(elapsedTime.TotalSeconds / refillIntervalInSeconds);
            bucket.CurrentTokens = Math.Min(bucket.MaxTokens, bucket.CurrentTokens + tokensToAdd);
            bucket.LastRefillTime = DateTime.UtcNow;

            if (bucket.CurrentTokens > 0) {
                // Consume a token
                bucket.CurrentTokens--;
                _cache.Set(key, bucket, TimeSpan.FromMinutes(1)); // Set expiration to 1 minute
                await _next(context);
            }
            else {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests; // Too Many Requests
                await context.Response.WriteAsync("Rate limit exceeded. Please try again later.");
            }
        }

        private class TokenBucket {
            public int CurrentTokens { get; set; }
            public int MaxTokens { get; set; }
            public DateTime LastRefillTime { get; set; }

            public TokenBucket(int currentTokens, int maxTokens, DateTime lastRefillTime) {
                CurrentTokens = currentTokens;
                MaxTokens = maxTokens;
                LastRefillTime = lastRefillTime;
            }
        }
    }

}
