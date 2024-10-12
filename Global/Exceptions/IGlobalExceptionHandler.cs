namespace PhotocopyRevaluationAppMVC.Global.Exceptions
{
    public interface IGlobalExceptionHandler
    {
        Task HandleExceptionAsync(HttpContext context, Exception exception);
    }
}