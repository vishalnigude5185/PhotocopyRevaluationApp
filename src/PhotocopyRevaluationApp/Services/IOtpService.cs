namespace PhotocopyRevaluationApp.Services {
    public interface IOtpService {
        void SetOtpAsync(string key, string otp, TimeSpan ttl);
        Task<string> GetOtpAsync(string key_userid);
        void RemoveOtpAsync(string key_userid);
        Task<Task> ClearOtpAfterDelayAsync(string userid, TimeSpan delay);
        Task<(bool Success, string Message)> ResendOtpAsync(ITempDataService _tempDataService);
        Task<string> GenerateOtpAsync(string userId, TimeSpan FromSeconds, int length = 6);
        Task<bool> SendOTPtoEmailAsync(string emailAddress, string otp);
        Task<bool> SendOtpToPhoneAsync(string toPhoneNumber, string otp);
    }
}