// File: FeatureFilters/TimeWindowFilter.cs
using Microsoft.FeatureManagement;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace PhotocopyRevaluationApp.Services.FeatureManagement {
    //public class TimeWindowFilter : IFeatureFilter
    //{
    //    private readonly ILogger<TimeWindowFilter> _logger;

    //    public TimeWindowFilter(ILogger<TimeWindowFilter> logger)
    //    {
    //        _logger = logger;
    //    }

    //    public Task<bool> EvaluateAsync(FeatureFilterEvaluationContext context)
    //    {
    //        // Retrieve the start and end time for the feature
    //        if (context.Parameters.TryGetValue("StartTime", out var startTimeValue) && startTimeValue is DateTime startTime &&
    //            context.Parameters.TryGetValue("EndTime", out var endTimeValue) && endTimeValue is DateTime endTime)
    //        {
    //            var now = DateTime.UtcNow;
    //            _logger.LogInformation("TimeWindowFilter: Checking if current time {Now} is between {StartTime} and {EndTime}", now, startTime, endTime);

    //            // Feature is enabled if the current time is within the specified window
    //            return Task.FromResult(now >= startTime && now <= endTime);
    //        }

    //        _logger.LogWarning("TimeWindowFilter: Invalid or missing start/end time parameters.");
    //        return Task.FromResult(false); // Default to false if time parameters are missing or invalid
    //    }
    //}
}
