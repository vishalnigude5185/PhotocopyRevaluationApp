// File: YourProject/FeatureFilters/RoleFilter.cs
using Microsoft.FeatureManagement;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhotocopyRevaluationApp.Services.FeatureManagement {
    //public class RoleFilter : IFeatureFilter
    //{
    //    private readonly ILogger<RoleFilter> _logger;
    //    private readonly IHttpContextAccessor _httpContextAccessor;

    //    public RoleFilter(ILogger<RoleFilter> logger, IHttpContextAccessor httpContextAccessor)
    //    {
    //        _logger = logger;
    //        _httpContextAccessor = httpContextAccessor;
    //    }
    //    //
    //    // summary
    //    // This is the method you need to implement
    //    public Task<bool> EvaluateAsync(FeatureFilterEvaluationContext context)
    //    {
    //        // Check the roles in the context parameters
    //        if (context.Parameters.ContainsKey("Roles")/*context.Parameters.TryGetValue("Roles", out var rolesValue)*/)
    //        {
    //        // Retrieve the roles from the context parameters
    //            var rolesValue = context.Parameters["Roles"];
    //            // Check if the retrieved value is a string
    //            if (rolesValue is string rolesString)
    //            {
    //                // Split the roles string into an array
    //                var roles = rolesString.Split(',').Select(role => role.Trim()).ToArray(); // Trim whitespace

    //                // Get the current user's claims from the HttpContext
    //                var userClaims = _httpContextAccessor.HttpContext?.User.Claims;

    //                // Check if the user has any of the specified roles
    //                bool hasRole = userClaims != null && roles.Any(role => userClaims.Any(claim => claim.Type == "role" && claim.Value == role));

    //                _logger.LogInformation("RoleFilter: User has role: {HasRole}", hasRole);
    //                return Task.FromResult(hasRole); // Return true if the user has the required role
    //            }
    //            else
    //            {
    //                _logger.LogWarning("Roles parameter is not a valid string.");
    //                return Task.FromResult(false);
    //            }
    //        }
    //        else
    //        {
    //            _logger.LogWarning("Roles parameter not found.");
    //            return Task.FromResult(false);
    //        }
    //    }
    //}
}
