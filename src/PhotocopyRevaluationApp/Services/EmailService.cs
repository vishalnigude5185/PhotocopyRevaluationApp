using System.Net;
using System.Net.Mail;
using FluentEmail.Core;
using FluentEmail.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace PhotocopyRevaluationApp.Services {
    public class EmailService : IEmailSender, IEmailService {
        private readonly GenerateUidService _generateUidService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        private readonly IFluentEmail _email;

        private readonly SmtpSender _smtpSender;

        public EmailService(IFluentEmail email, GenerateUidService generateUidService, IConfiguration configuration, IHttpContextAccessor httpContextAccessor) {
            _email = email;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _generateUidService = generateUidService;
        }
        public EmailService() {
            _smtpSender = new SmtpSender(() => new SmtpClient("smtp.yourserver.com") {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("username", "password"),
                EnableSsl = true,
                Port = 587
            });

            FluentEmail.Core.Email.DefaultSender = _smtpSender;
        }

        public async Task FluentSmtpOrSendGridSendEmailAsync() {
            var result = await _email
            .To("recipient@example.com", "Recipient Name")
            .Subject("Test Email")
            .Body("This is a test email sent using FluentEmail in .NET Core.")
            .SendAsync();

            //Using razor template
            //var model = new { Name = "Recipient Name" };
            //_ = await _email
            //    .To("recipient@example.com", "Recipient Name")
            //    .Subject("Test Email with Razor Template")
            //    .UsingTemplateFromFile("Views/Emails/TestEmail.cshtml", model)
            //    .SendAsync();
        }
        public async Task SmtpSendEmailAsync() {
            await FluentEmail.Core.Email
                .From("your-email@example.com")
                .To("recipient@example.com")
                .Subject("Log Event")
                .Body("An error occurred")
                .SendAsync();
        }

        public async Task<bool> SmtpSendEmailAsync(string emailAddress, string subject, string body) {
            //Throw SMTP
            var smtpClient = new SmtpClient(_configuration["Smtp:Host"]) {
                Port = int.Parse(_configuration["Smtp:Port"]),
                Credentials = new NetworkCredential(_configuration["Smtp:Username"], _configuration["Smtp:Password"]),
                EnableSsl = bool.Parse(_configuration["Smtp:EnableSsl"])
            };

            var mailMessage = new MailMessage {
                From = new MailAddress(_configuration["Smtp:From"]),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(emailAddress);
            await smtpClient.SendMailAsync(mailMessage); //Or smtpClient.Send(mailMessage);
            return true;
        }
        public async Task<bool> SendGridSendEmailAsync(string emailAddress, string subject, string body) {
            string? _apiKey = Environment.GetEnvironmentVariable("SendGridAPIKey");

            if (_apiKey != null) {
                var client = new SendGridClient(_apiKey);
                var to = new EmailAddress(emailAddress);
                var msg = MailHelper.CreateSingleEmail(new EmailAddress(_configuration["SendGrid:from"], _configuration["SendGrid:emailSenderName"]), to, subject, body, null);

                await client.SendEmailAsync(msg);

                return true;
            }
            else {
                return false;
            }
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage) {
            throw new NotImplementedException();
        }
    }
}
