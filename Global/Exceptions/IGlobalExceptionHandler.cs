namespace PhotocopyRevaluationApp.Global.Exceptions {
    public interface IGlobalExceptionHandler {
        Task HandleExceptionAsync(HttpContext context, Exception exception);
    }
}