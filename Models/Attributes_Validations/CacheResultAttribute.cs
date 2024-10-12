    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Caching.Memory;
    using System;
namespace PhotocopyRevaluationAppMVC.Models.Attributes_Validations
{
    //Time-Based Caching for an Action
    //This attribute caches the result of an action for a specified duration, improving performance by reducing redundant database calls or calculations.
    public class CacheResultAttribute : Attribute, IActionFilter
    {
        private readonly int _duration;
        private readonly IMemoryCache _cache;

        public CacheResultAttribute(int duration, IMemoryCache cache)
        {
            _duration = duration;
            _cache = cache;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            string cacheKey = context.HttpContext.Request.Path;
            if (_cache.TryGetValue(cacheKey, out object cachedResult))
            {
                context.Result = new ContentResult
                {
                    Content = cachedResult.ToString(),
                    ContentType = "application/json"
                };
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            string cacheKey = context.HttpContext.Request.Path;
            _cache.Set(cacheKey, context.Result, TimeSpan.FromSeconds(_duration));
        }
    }

}
