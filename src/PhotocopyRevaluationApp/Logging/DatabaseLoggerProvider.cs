using PhotocopyRevaluationApp.Data;

namespace PhotocopyRevaluationApp.Logging {
    public class DatabaseLoggerProvider : ILoggerProvider {
        private readonly LoggingContext _context;

        public DatabaseLoggerProvider(LoggingContext context) {
            _context = context;
        }

        public ILogger CreateLogger(string categoryName) {
            return new DatabaseLogger(_context);
        }

        public void Dispose() {
            // Implement any necessary cleanup logic here, if required
        }
    }
}
