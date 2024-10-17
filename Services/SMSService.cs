using Twilio.Types;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using static System.Net.WebRequestMethods;

namespace PhotocopyRevaluationApp.Services {
    public class SMSService {
        private readonly GenerateUidService _generateUidService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public SMSService(GenerateUidService generateUidService, IConfiguration configuration, IHttpContextAccessor httpContextAccessor) {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _generateUidService = generateUidService;
        }

        public async Task<bool> SendSmsToPhoneAsync(string toPhoneNumber, string body) {
            // Initialize Twilio client
            string? accountSid = Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID");
            string? authToken = Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN");

            if (accountSid != null & authToken != null) {
                TwilioClient.Init(accountSid, authToken);
                // Send OTP via Twilio
                var message = MessageResource.Create(
                    body: body,
                    from: new PhoneNumber("+19705572009"),
                    to: new PhoneNumber(toPhoneNumber)
                );

                return true;
            }
            else {
                return false;
            }
        }
    }
}
