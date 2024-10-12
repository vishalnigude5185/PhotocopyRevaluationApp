using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace PhotocopyRevaluationAppMVC.DependencyInjection
{
    public class DynamicServiceResolver
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DynamicServiceResolver> _logger;

        public DynamicServiceResolver(IServiceProvider serviceProvider, ILogger<DynamicServiceResolver> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        // Method to resolve a scoped service with logging and validation
        public T GetScoped<T>() where T : class
        {
            _logger.LogInformation($"Attempting to resolve scoped service: {typeof(T).Name}");

            // Example condition: Check if the service is not null in the container
            if (_serviceProvider.GetService(typeof(T)) == null)
            {
                _logger.LogWarning($"Scoped service {typeof(T).Name} not found in the container.");
                throw new InvalidOperationException($"Service {typeof(T).Name} is not registered in the DI container.");
            }

            using (var scope = _serviceProvider.CreateScope())
            {
                var service = scope.ServiceProvider.GetRequiredService<T>();
                _logger.LogInformation($"Successfully resolved scoped service: {typeof(T).Name}");
                return service;
            }
        }

        // Method to resolve a transient service with logging and validation
        public T GetTransient<T>() where T : class
        {
            _logger.LogInformation($"Attempting to resolve transient service: {typeof(T).Name}");

            // Example condition: Check if the service is registered as transient
            if (_serviceProvider.GetService(typeof(T)) == null)
            {
                _logger.LogWarning($"Transient service {typeof(T).Name} not found in the container.");
                throw new InvalidOperationException($"Service {typeof(T).Name} is not registered in the DI container.");
            }

            var service = _serviceProvider.GetRequiredService<T>();
            _logger.LogInformation($"Successfully resolved transient service: {typeof(T).Name}");
            return service;
        }

        // Method to resolve a singleton service with logging and validation
        public T GetSingleton<T>() where T : class
        {
            _logger.LogInformation($"Attempting to resolve singleton service: {typeof(T).Name}");

            // Example condition: Check if the service is a singleton (same instance)
            var singletonInstance = _serviceProvider.GetService(typeof(T));
            if (singletonInstance == null)
            {
                _logger.LogWarning($"Singleton service {typeof(T).Name} not found in the container.");
                throw new InvalidOperationException($"Service {typeof(T).Name} is not registered as a singleton in the DI container.");
            }

            _logger.LogInformation($"Successfully resolved singleton service: {typeof(T).Name}");
            return (T)singletonInstance;
        }
    }
}
