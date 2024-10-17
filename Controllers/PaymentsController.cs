using Microsoft.AspNetCore.Mvc;
using PhotocopyRevaluationApp.Services;

namespace PhotocopyRevaluationApp.Controllers {
    public class PaymentsController : Controller {
        private readonly IPayPalService _payPalService;

        public PaymentsController(IPayPalService payPalService) {
            _payPalService = payPalService;

        }
        public IActionResult Index() {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreatePayment(decimal amount) {
            var approvalUrl = await _payPalService.CreateOrderAsync(amount, "USD");

            // Redirect user to PayPal approval URL
            return Redirect(approvalUrl);
        }
        //[HttpPost]
        //public IActionResult CreateOrder()
        //{
        //    // Create an instance of Razorpay client
        //    RazorpayClient client = new RazorpayClient(key, secret);

        //    // Define order parameters (amount in paisa)
        //    Dictionary<string, object> options = new Dictionary<string, object>();
        //    options.Add("amount", 50000); // Amount is in smallest unit of currency, i.e. 50000 means 500.00 INR
        //    options.Add("currency", "INR");
        //    options.Add("receipt", "order_rcptid_11"); // Receipt number

        //    // Create the Razorpay Order
        //    Order order = client.Order.Create(options);

        //    // Return the Order ID to the client-side
        //    return Json(new { orderId = order["id"].ToString() });
        //}
        //[HttpPost]
        //public async Task<IActionResult> CreatePaymentAsync(decimal amount)
        //{
        //    var paymentResponse = await _payPalService.CreatePaymentAsync(amount);
        //    // Extract approval URL from the response and redirect user to PayPal for approval
        //    // return Redirect(approvalUrl);
        //    return Ok(paymentResponse); // For testing, return the response
        //}
        public async Task<IActionResult> PaymentSuccess(string token, string PayerID) {
            // Using deconstruction to access both return values
            (bool success, string Id) = await _payPalService.CapturePaymentAsync(PayerID);

            // Handle success (e.g., save to database, show confirmation page, etc.)
            ViewBag.OrderId = Id;

            return View();
        }

        public IActionResult PaymentCancel() {
            // Handle cancel
            return View();
        }
    }
}
