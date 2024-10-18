namespace PhotocopyRevaluationApp.Services {
    public interface IPayPalService {
        Task<string> CreatePaymentAsync(decimal amount);
        Task<(bool, string)> CapturePaymentAsync(string orderId);
        Task<string/*Order*/> CreateOrderAsync(decimal amount, string currency = "USD");
    }
}