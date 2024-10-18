using System.Globalization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.FeatureManagement;
using Microsoft.ML;
using PhotocopyRevaluationApp.Global.Exceptions;

namespace PhotocopyRevaluationApp.Global {
    public static class GlobalVariables {
        public static ITransformer Pipeline { get; set; }
        // Error Handling
        public static IGlobalExceptionHandler GlobalExceptionHandler { get; set; }
        // Feature Management
        public static IFeatureManagementBuilder FeatureManagementBuilder { get; set; }
        //public static IFeatureManagement FeatureManagement { get; set; }

        // Application Settings
        public static AppSettings Settings { get; set; }

        // User Session Data
        public static UserSession CurrentUser { get; set; }

        // Configuration Settings
        public static LogLevel LoggingLevel { get; set; }
        public static CultureInfo CultureInfo { get; set; }
        public static string TimeZone { get; set; }

        // Database Configuration
        public static string DatabaseConnection { get; set; }
        public static int CacheDuration { get; set; }

        // Application State
        public static bool IsMaintenanceMode { get; set; }
        public static Dictionary<string, bool> FeatureToggle { get; set; }

        // External API Settings
        public static string ExternalApiUrl { get; set; }
        public static int ApiRateLimit { get; set; }

        // Caching Variables
        public static Dictionary<string, object> Cache { get; set; }
        public static TimeSpan CacheExpiration { get; set; }

        // Statistics and Metrics
        public static int RequestCount { get; set; }
        public static int ErrorCount { get; set; }

        // File Paths and URLs
        public static string UploadDirectory { get; set; }
        public static string StaticFileUrl { get; set; }

        // Security Settings
        public static string JwtSecretKey { get; set; }
        public static List<string> AllowedOrigins { get; set; }
        // Environment Settings
        public static string EnvironmentName { get; set; }
        public static bool IsDevelopment { get; set; }

        // Application Information
        public static string AppVersion { get; set; }
        public static string AppName { get; set; }

        // Dependency Injection Services
        public static IServiceProvider ServiceProvider { get; set; }

        // Configuration Settings
        public static IConfiguration Configuration { get; set; }
        public static IDictionary<string, string> ConnectionStrings { get; set; }

        public static ILogger ErrorLog { get; set; }

        // Caching Mechanism
        public static IMemoryCache MemoryCache { get; set; }
        public static IDistributedCache DistributedCache { get; set; }

        // Localization Settings
        public static List<CultureInfo> SupportedCultures { get; set; }
        public static CultureInfo DefaultCulture { get; set; }

        // User Settings
        public static string CurrentTheme { get; set; }
        public static UserPreferences UserPreferences { get; set; }

        // Authorization and Authentication
        public static JwtBearerOptions JwtBearerOptions { get; set; }
        public static List<string> AuthenticationSchemes { get; set; }

        // Logging Settings
        public static ILoggerFactory LoggerFactory { get; set; }
        public static string LogFilePath { get; set; }

        // Third-party Service Credentials
        public static Dictionary<string, string> ThirdPartyApiKeys { get; set; }
        public static Dictionary<string, string> ServiceEndpoints { get; set; }
    }
    public class AppSettings {
        public string ApiKey { get; set; }
        public string BaseUrl { get; set; }
        public Dictionary<string, string> FeatureFlags { get; set; }
    }

    public class UserSession {
        public string UserId { get; set; }
        public List<string> Roles { get; set; }
        // Add other user-specific properties as needed
    }
    public class UserPreferences {
        public bool ReceiveNotifications { get; set; }
        public string DisplayLanguage { get; set; }
        // Additional user preference properties can be added here
    }
}