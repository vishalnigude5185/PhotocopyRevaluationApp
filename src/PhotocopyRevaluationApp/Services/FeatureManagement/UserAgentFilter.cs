// File: FeatureFilters/UserAgentFilter.cs
namespace PhotocopyRevaluationApp.Services.FeatureManagement {
    //public class UserAgentFilter : IFeatureFilter
    //{
    //    private readonly ILogger<UserAgentFilter> _logger;
    //    private readonly IHttpContextAccessor _httpContextAccessor;

    //    public UserAgentFilter(ILogger<UserAgentFilter> logger, IHttpContextAccessor httpContextAccessor)
    //    {
    //        _logger = logger;
    //        _httpContextAccessor = httpContextAccessor;
    //    }

    //    public Task<bool> EvaluateAsync(FeatureFilterEvaluationContext context)
    //    {
    //        // Retrieve user-agent substrings from the feature configuration
    //        if (context.Parameters.TryGetValue("UserAgents", out var userAgentsValue) && userAgentsValue is string userAgentsString)
    //        {
    //            var allowedUserAgents = userAgentsString.Split(',').Select(ua => ua.Trim()).ToArray();
    //            var userAgent = _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString();

    //            if (!string.IsNullOrEmpty(userAgent))
    //            {
    //                // Check if the user-agent contains any of the allowed substrings
    //                bool isAllowed = allowedUserAgents.Any(ua => userAgent.Contains(ua));
    //                _logger.LogInformation("UserAgentFilter: User agent {UserAgent} is allowed: {IsAllowed}", userAgent, isAllowed);
    //                return Task.FromResult(isAllowed);
    //            }

    //            _logger.LogWarning("UserAgentFilter: User agent is null or empty.");
    //            return Task.FromResult(false);
    //        }

    //        _logger.LogWarning("UserAgentFilter: Invalid or missing UserAgents parameter.");
    //        return Task.FromResult(false);
    //    }
    //}
}
