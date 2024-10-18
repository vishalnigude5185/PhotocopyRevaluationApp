// File: FeatureFilters/EnvironmentFilter.cs
using Microsoft.FeatureManagement;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace PhotocopyRevaluationApp.Services.FeatureManagement {
    //public class EnvironmentFilter : IFeatureFilter
    //{
    //    private readonly ILogger<EnvironmentFilter> _logger;
    //    private readonly string _currentEnvironment;

    //    public EnvironmentFilter(ILogger<EnvironmentFilter> logger, string currentEnvironment)
    //    {
    //        _logger = logger;
    //        _currentEnvironment = currentEnvironment;
    //    }

    //    public Task<bool> EvaluateAsync(FeatureFilterEvaluationContext context)
    //    {
    //        // Retrieve the allowed environments from the feature configuration
    //        if (context.Parameters.TryGetValue("Environments", out var environmentsValue) && environmentsValue is string environmentsString)
    //        {
    //            var allowedEnvironments = environmentsString.Split(',').Select(env => env.Trim()).ToArray();

    //            _logger.LogInformation("EnvironmentFilter: Checking if current environment {CurrentEnvironment} is in allowed environments", _currentEnvironment);

    //            // Check if the current environment is in the list of allowed environments
    //            return Task.FromResult(allowedEnvironments.Contains(_currentEnvironment));
    //        }

    //        _logger.LogWarning("EnvironmentFilter: Invalid or missing environments parameter.");
    //        return Task.FromResult(false);
    //    }
    //}
}
