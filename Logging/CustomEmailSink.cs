using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Serilog.Core;
using Serilog.Events;

namespace PhotocopyRevaluationAppMVC.Logging
{
    public class CustomEmailSink : ILogEventSink
    {
        private readonly IFormatProvider _formatProvider;

        public CustomEmailSink(IFormatProvider formatProvider)
        {
            _formatProvider = formatProvider;
        }

        public void Emit(LogEvent logEvent)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Logger", "your-email@example.com"));
            message.To.Add(new MailboxAddress("Recipient", "recipient@example.com"));
            message.Subject = "Log Event";

            var body = logEvent.RenderMessage(_formatProvider);
            message.Body = new TextPart("plain") { Text = body };

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.yourserver.com", 587, SecureSocketOptions.StartTls);
                client.Authenticate("username", "password");
                client.Send(message);
                client.Disconnect(true);
            }
        }
    }

    // Configure Serilog to use the custom sink
    //Log.Logger = new LoggerConfiguration()
    //.WriteTo.Sink(new CustomEmailSink(null))
    //.CreateLogger();

}
