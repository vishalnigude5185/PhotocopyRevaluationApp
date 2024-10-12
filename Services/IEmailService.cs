
namespace PhotocopyRevaluationAppMVC.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string htmlMessage);
        Task<bool> SendGridSendEmailAsync(string emailAddress, string from, string body);
        Task<bool> SmtpSendEmailAsync(string email, string subject, string htmlMessage);
    }
}