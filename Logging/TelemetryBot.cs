using Microsoft.ApplicationInsights;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace PhotocopyRevaluationAppMVC.Logging
{
    public class TelemetryBot : IBot
    {
        private readonly TelemetryClient _telemetryClient;

        public TelemetryBot(TelemetryClient telemetryClient)
        {
            _telemetryClient = telemetryClient;
        }

        public Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default) {
            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                // Log user message
                _telemetryClient.TrackEvent("UserMessage", new Dictionary<string, string>
               {
                   { "UserId", turnContext.Activity.From.Id },
                   { "MessageText", turnContext.Activity.Text }
               });

                // Handle bot logic here...
            }

            return Task.CompletedTask;
        }
    }
}
