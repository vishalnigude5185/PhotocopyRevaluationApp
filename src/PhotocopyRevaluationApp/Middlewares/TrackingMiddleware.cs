using Microsoft.ApplicationInsights;

namespace PhotocopyRevaluationApp.Middlewares {
    public class TrackingMiddleware {
        private readonly RequestDelegate _next;
        private readonly TelemetryClient _telemetryClient;

        public TrackingMiddleware(RequestDelegate next, TelemetryClient telemetryClient) {
            _next = next;
            _telemetryClient = telemetryClient;
        }

        public async Task Invoke(HttpContext context) {
            try {
                // Track an event when an endpoint is accessed
                _telemetryClient.TrackEvent("Endpoint Accessed", new Dictionary<string, string>
            {
                { "Path", context.Request.Path },
                { "Method", context.Request.Method }
            });

                await _next(context);
            }
            catch (Exception ex) {
                // Track exceptions
                _telemetryClient.TrackException(ex);
                throw; // Re-throw the exception after logging
            }
        }
    }
}
