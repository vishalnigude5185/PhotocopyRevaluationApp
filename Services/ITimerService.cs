namespace PhotocopyRevaluationAppMVC.Services
{
    public interface ITimerService
    {
        void StartAsync(TimeSpan interval, Func<Task> taskToRun);
        //Task StartAsync(TimeSpan interval, Func<Task> taskToRun, CancellationToken cancellationToken);
    }
}
