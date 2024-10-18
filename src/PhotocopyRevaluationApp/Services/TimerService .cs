using System.Timers;

namespace PhotocopyRevaluationApp.Services {
    // File: Services/TimerService.cs
    public class TimerService : ITimerService, IHostedService {
        private readonly ILogger<TimerService> _logger;
        private readonly IHttpContextAccessor _contextAccessor;
        private Func<Task> _taskToRun;
        private TimeSpan _interval;
        private System.Threading.Timer _timer;

        private bool isTimerRunning = false; // Class-level variable to track the timer state
        private System.Timers.Timer sessionCheckTimer;

        public TimerService(ILogger<TimerService> logger, IHttpContextAccessor contextAccessor) {
            _logger = logger;
            _contextAccessor = contextAccessor;
        }

        public async void StartAsync(TimeSpan interval, Func<Task> taskToRun) {
            if (!isTimerRunning) {
                // Initialize and start the timer if it's not running
                var sessionCheckTimer = new System.Timers.Timer(interval.TotalMilliseconds); // Use TotalMilliseconds for Timer // Check every minute
                sessionCheckTimer.Elapsed += async (sender, e) => await taskToRun();
                sessionCheckTimer.AutoReset = true; // Repeat every interval
                sessionCheckTimer.Start();
            }
            isTimerRunning = true; // Set the flag to indicate that the timer is running
        }
        public async Task StartAsync(TimeSpan interval, Func<Task> taskToRun, CancellationToken cancellationToken) {
            _interval = interval;
            _taskToRun = taskToRun;
            _timer = new System.Threading.Timer(ExecuteTask, null, TimeSpan.Zero, _interval);
            _logger.LogInformation("Timer service started with an interval of {Interval}", _interval);
        }
        public async Task StartAsync(CancellationToken cancellationToken) {
            _logger.LogInformation("Timer service is starting.");

            // You can set an initial interval for the timer.
            TimeSpan interval = TimeSpan.FromSeconds(10); // For example, every 10 seconds.

            // Initialize the timer and start executing the task at a specified interval.
            _timer = new System.Threading.Timer(ExecuteTask, null, TimeSpan.Zero, interval);

            // Optionally, wait for some initial setup or precondition
            // For example, loading configuration or validating prerequisites.
            await InitializeAsync(cancellationToken);
        }
        private async void ExecuteTask(object state) {
            // Here you might want to check if the cancellation token is triggered
            // before executing your task.
            //if (_cancellationTokenSource.Token.IsCancellationRequested)
            //{
            //    return; // Exit if the cancellation was requested.
            //}
            try {
                // Perform the actual work here
                DoWorkAsync();
                _logger.LogInformation("Executing scheduled task.");
                //await _taskToRun();
                _logger.LogInformation("Task executed successfully.");
            }
            catch (Exception ex) {
                _logger.LogError(ex, "An error occurred while executing the scheduled task.");
            }
        }
        // A placeholder method for your actual work
        private async void DoWorkAsync() {
            // Simulate work (like calling an API, processing data, etc.)
            await Task.Delay(1000); // Simulate a delay of 1 second for the work.

            // Your actual logic here
            _logger.LogInformation("Work completed successfully.");
        }
        public Task StopAsync(CancellationToken cancellationToken) {
            _logger.LogInformation("Stopping timer service.");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
        // Method to initialize resources or settings if needed
        private async Task InitializeAsync(CancellationToken cancellationToken) {
            // Simulate some initial setup
            await Task.Delay(500); // Simulate a setup delay of half a second.
            _logger.LogInformation("Initialization completed.");
        }
    }
}
