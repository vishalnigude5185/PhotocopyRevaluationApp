namespace PhotocopyRevaluationApp.Services.BackgroundTasks {
    public class EmailBackgroundService : BackgroundService {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            while (!stoppingToken.IsCancellationRequested) {

                // Wait for 5 minutes before sending the next email
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }
}
