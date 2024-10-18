using System.Text;
using Microsoft.Extensions.Caching.Memory;
using PhotocopyRevaluationApp.Controllers;

namespace PhotocopyRevaluationApp.Services {
    public class OtpService : IOtpService {
        private readonly SMSService _smsService;
        private readonly EmailService _emailService;

        private readonly IMemoryCache _memoryCache;

        private readonly ILogger<AccountsController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public OtpService(SMSService smsService, EmailService emailService, IHttpContextAccessor httpContextAccessor, ILogger<AccountsController> logger, IMemoryCache memoryCache) {
            _memoryCache = memoryCache;

            _smsService = smsService;
            _emailService = emailService;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async void SetOtpAsync(string key_userid, string otp, TimeSpan ttl) {
            var cacheEntryOptions = new MemoryCacheEntryOptions {
                AbsoluteExpirationRelativeToNow = ttl // Automatically remove the OTP after the specified TTL
            };

            _memoryCache.Set(key_userid, otp, ttl); // OTP will expire after `ttl`
        }

        public async Task<string?> GetOtpAsync(string key_userid) {
            _memoryCache.TryGetValue(key_userid, out string? otp);

            return otp;
        }

        public async void RemoveOtpAsync(string key_userid) {
            _memoryCache.Remove(key_userid);
        }
        public async Task<Task> ClearOtpAfterDelayAsync(string userid, TimeSpan delay) {
            // Wait for the specified delay (e.g., 30 seconds)
            await Task.Delay(delay);

            // Remove the OTP after the delay
            RemoveOtpAsync(userid);
            Console.WriteLine($"OTP for key {userid} has been cleared after {delay.TotalSeconds} seconds.");

            return Task.CompletedTask;
        }
        public async Task<bool> SendOtpToPhoneAsync(string toPhoneNumber, string otp) {
            if (string.IsNullOrEmpty(otp)) {
                return false;
            }
            else {
                string body = $"Your OTP code is: {otp}";
                bool result = await _smsService.SendSmsToPhoneAsync(toPhoneNumber, body);

                return result;
            }
        }
        public async Task<bool> SendOTPtoEmailAsync(string emailAddress, string otp) {
            if (string.IsNullOrEmpty(otp)) {
                return false;
            }
            else {
                string subject = "Your OTP Code for Secure Access";
                string body = $"Your One-Time Password (OTP) for PhotocopyRevaluationApp: {otp}";
                var result = await _emailService.SendGridSendEmailAsync(emailAddress, subject, body);
                if (!result) {
                    await _emailService.SmtpSendEmailAsync(emailAddress, subject, body);
                }
                return result;
            }
        }
        public async Task<string> GenerateOtpAsync(string userId, TimeSpan FromSeconds, int length = 6) {
            var random = new Random();
            var otp = new StringBuilder();

            for (int i = 0; i < length; i++) {
                otp.Append(random.Next(0, 10)); // Append a random digit
            }
            string gotp = otp.ToString();
            // Save OTP to session or database for later verification
            _httpContextAccessor.HttpContext!.Session.SetString(userId, gotp); // Store OTP as a string in session
            string ggotp = _httpContextAccessor.HttpContext!.Session.GetString("4");
            SetOtpAsync(userId, gotp, FromSeconds); // Store OTP with 30 seconds TTL
            ggotp = await GetOtpAsync(userId);

            //OR
            //Random random = new Random();
            //string otp = new string(Enumerable.Range(0, length).Select(x => random.Next(0, 10).ToString()[0]).ToArray());

            return gotp;
        }
        public async Task<(bool Success, string Message)> ResendOtpAsync(ITempDataService _tempDataService) {
            string? phoneNumber = (string?)_tempDataService.GetTempData("PhoneNumber");
            string? email = (string?)_tempDataService.GetTempData("Email");
            string? userId = (string?)_tempDataService.GetTempData("UserId");
            try {
                if (string.IsNullOrEmpty(phoneNumber)) {
                    throw new ArgumentNullException(nameof(phoneNumber), "Phone number cannot be null.");
                }
                else if (string.IsNullOrEmpty(email)) {
                    throw new ArgumentNullException(nameof(email), "Email cannot be null.");
                }
                else if (string.IsNullOrEmpty(userId)) {
                    throw new ArgumentNullException(nameof(userId), "Email cannot be null.");
                }
                string otp = await GenerateOtpAsync(userId, TimeSpan.FromSeconds(30));
                // Send OTP to phone
                bool phoneOtpSent = await SendOtpToPhoneAsync(phoneNumber, otp);

                // Send OTP to email
                bool emailOtpSent = await SendOTPtoEmailAsync(email, otp);

                if (phoneOtpSent && emailOtpSent) {
                    return (true, "OTP has been sent successfully. Please check your mobile phone and email address.");
                }
                else {
                    return (false, "Internal error occurred, unable to send the OTP.");
                }
            }
            catch (ArgumentNullException ex) {
                _logger.LogError(ex, "An error occurred while resending OTP: {Message}", ex.Message);
                return (false, ex.Message); // Return error message
            }
            catch (Exception ex) {
                _logger.LogError(ex, "An unexpected error occurred while resending OTP.");
                return (false, "An unexpected error occurred.");
            }
        }
    }
}
