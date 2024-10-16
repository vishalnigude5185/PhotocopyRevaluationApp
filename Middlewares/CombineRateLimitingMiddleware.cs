// File: Middleware/RateLimitingMiddleware.cs
using Microsoft.Extensions.Caching.Memory;

namespace PhotocopyRevaluationAppMVC.NewFolder
{
    public class CombineRateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;

        public CombineRateLimitingMiddleware(RequestDelegate next, IMemoryCache cache)
        {
            _next = next;
            _cache = cache;
        }

        public async Task Invoke(HttpContext context)
        {
            var ipAddress = context.Connection.RemoteIpAddress?.ToString();
            if (string.IsNullOrEmpty(ipAddress))
            {
                await _next(context);
                return;
            }

            // Sliding Window Strategy
            var slidingWindowResult = await HandleSlidingWindowRateLimiting(context, ipAddress);
            if (slidingWindowResult) return;

            // Token Bucket Strategy
            var tokenBucketResult = await HandleTokenBucketRateLimiting(context, ipAddress);
            if (tokenBucketResult) return;

            // Continue processing the request if no limits were reached
            await _next(context);
        }

        // Sliding Window Logic
        private async Task<bool> HandleSlidingWindowRateLimiting(HttpContext context, string ipAddress)
        {
            var key = $"SlidingRateLimit_{ipAddress}";
            const int limit = 5; // Maximum number of requests
            const int timeWindow = 60; // Time window in seconds

            if (!_cache.TryGetValue(key, out List<DateTime>? timestamps)) {
                timestamps = new List<DateTime>(); // This ensures timestamps is initialized if not found.
            }

            // Now you can safely use timestamps, knowing it's either the cached value or a new list.

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            timestamps.RemoveAll(timestamp => timestamp < DateTime.UtcNow.AddSeconds(-timeWindow));
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            if (timestamps.Count >= limit)
            {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.Response.WriteAsync("Sliding Window Rate limit exceeded. Please try again later.");
                return true; // Stop further processing
            }

            timestamps.Add(DateTime.UtcNow);
            _cache.Set(key, timestamps, DateTimeOffset.UtcNow.AddSeconds(timeWindow));
            return false;
        }

        // Token Bucket Logic
        private async Task<bool> HandleTokenBucketRateLimiting(HttpContext context, string ipAddress)
        {
            var key = $"TokenBucket_{ipAddress}";
            const int maxTokens = 5;
            //const int refillTokens = 1;
            const int refillIntervalInSeconds = 1;

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

            if (bucket.CurrentTokens > 0)
            {
                bucket.CurrentTokens--;
                _cache.Set(key, bucket, TimeSpan.FromMinutes(1));
                return false; // Allow the request
            }

            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            await context.Response.WriteAsync("Token Bucket Rate limit exceeded. Please try again later.");
            return true; // Stop further processing
        }

        // TokenBucket Class used for the Token Bucket algorithm
        private class TokenBucket
        {
            public int CurrentTokens { get; set; }
            public int MaxTokens { get; set; }
            public DateTime LastRefillTime { get; set; }

            public TokenBucket(int currentTokens, int maxTokens, DateTime lastRefillTime)
            {
                CurrentTokens = currentTokens;
                MaxTokens = maxTokens;
                LastRefillTime = lastRefillTime;
            }
        }
    }
}
