    // File: YourProject/FeatureFilters/PercentageFilter.cs
    using Microsoft.FeatureManagement;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Threading.Tasks;

namespace PhotocopyRevaluationAppMVC.Services.FeatureManagement
{
    //public class PercentageFilter : IFeatureFilter
    //{
    //    private readonly ILogger<PercentageFilter> _logger;

    //    public PercentageFilter(ILogger<PercentageFilter> logger)
    //    {
    //        _logger = logger;
    //    }

    //    // Implement the required EvaluateAsync method
    //    public async Task<bool> EvaluateAsync(FeatureFilterEvaluationContext context)
    //    {
    //        // Retrieve the percentage from the context parameters
    //        if (!context.Parameters.TryGetValue("Percentage", out var percentageValue) || !(percentageValue is int percentage))
    //        {
    //            _logger.LogWarning("Percentage value not found or invalid. Defaulting to 0.");
    //            return false; // Or handle as needed, depending on your logic
    //        }

    //        var randomValue = new Random().Next(0, 100);
    //        _logger.LogInformation("PercentageFilter: Checking feature with random value: {RandomValue} against percentage: {Percentage}", randomValue, percentage);

    //        // Determine if the feature should be enabled based on the random value
    //        return randomValue < percentage;
    //    }
    //}
}
