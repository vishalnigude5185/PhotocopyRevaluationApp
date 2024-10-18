using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using PayPal;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using PhotocopyRevaluationApp.Configurations;

namespace PhotocopyRevaluationApp.Services {
    public class PayPalService : IPayPalService {
        private readonly HttpClient _client;

        private readonly PayPalHttpClient _payPalclient;
        private readonly PayPalSettings _payPalSettings;

        public PayPalService(PayPalSettings payPalSettings) {
            _client = new HttpClient {
                BaseAddress = new Uri(payPalSettings.BaseUrl) // Sandbox environment
            };

            var environment = new SandboxEnvironment(payPalSettings.ClientId, payPalSettings.ClientSecret);
            _payPalclient = new PayPalHttpClient(environment);
            _payPalSettings = payPalSettings;
        }
        async Task<string> IPayPalService.CreateOrderAsync(decimal amount, string currency) {
            var orderRequest = new OrdersCreateRequest();
            orderRequest.Prefer("return=representation");
            orderRequest.RequestBody(new OrderRequest {
                CheckoutPaymentIntent = "CAPTURE",
                PurchaseUnits = new[]
                {
                    new PurchaseUnitRequest
                    {
                        AmountWithBreakdown = new AmountWithBreakdown
                        {
                            CurrencyCode = currency,
                            Value = amount.ToString()
                        }
                    }
                }.ToList(), // Convert the array to a List,
                ApplicationContext = new ApplicationContext {
                    ReturnUrl = "https://e521-115-246-255-180.ngrok-free.app/Payments/PaymentSuccess" /*"https://localhost:5001/Payments/PaymentSuccess"*/,
                    CancelUrl = "https://e521-115-246-255-180.ngrok-free.app/Payments/PaymentCancel" /*"https://localhost:5001/Home/PaymentCancel"*/
                }
            });

            try {
                // Execute the request
                var response = await _payPalclient.Execute(orderRequest);
                var result = response.Result<PayPalCheckoutSdk.Orders.Order>();

                // Extract the approval link and return it (so the user can approve the payment)
                var approvalLink = result.Links.FirstOrDefault(link => link.Rel == "approve")?.Href;
                //return result;

                return approvalLink;  // Return the URL where the user will approve the payment
            }
            catch (HttpException ex) {
                // Handle exception
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
            //// Send the payment creation request using PayPalHttpClient
            //var environment = new SandboxEnvironment(clientId, clientSecret); // Use your sandbox credentials
            //var client = new PayPalHttpClient(environment);

            //// Create payment request
            //var request = new PaymentCreateRequest();
            //request.RequestBody(payment);

            //// Execute the request
            //var response = await client.Execute(request);
        }

        public async Task<(bool, string)> CapturePaymentAsync(string orderId) {
            var request = new OrdersCaptureRequest(orderId);
            request.RequestBody(new OrderActionRequest());

            try {
                var response = await _payPalclient.Execute(request);
                var result = response.Result<PayPalCheckoutSdk.Orders.Order>();

                // Check if the capture was successful
                if (result.Status == "COMPLETED") {
                    return (true, result.Id);  // Payment captured successfully
                }
            }
            catch (HttpException ex) {
                // Handle exception
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }

            return (false, null);
        }

        public async Task<string> CreatePaymentAsync(decimal amount) {
            // Create payment request object
            var paymentRequest = new {
                intent = "sale",
                payer = new { payment_method = "paypal" },
                transactions = new[]
                {
                    new
                    {
                        amount = new { total = amount.ToString("0.00"), currency = "USD" },
                        description = "Payment description"
                    }
                },
                redirect_urls = new {
                    ReturnUrl = "https://localhost:7237/Home/PaymentSuccess" /*"https://localhost:5001/Home/PaymentSuccess"*/,
                    CancelUrl = "https://localhost:7237/Home/PaymentCancel" /*"https://localhost:5001/Home/PaymentCancel"*/
                }
            };

            var json = JsonConvert.SerializeObject(paymentRequest);
            var requestBody = new StringContent(json, Encoding.UTF8, "application/json");
            string endpoint = "/v1/payments/payment";
            // Send POST request to create payment
            var response = await PostAsync(endpoint, requestBody);
            if (response.IsSuccessStatusCode) {
                var responseBody = await response.Content.ReadAsStringAsync();
                return responseBody; // Handle successful payment response
            }
            else {
                var errorResponse = await response.Content.ReadAsStringAsync();
                throw new Exception($"Payment creation failed: {errorResponse}");
            }
        }
        private async Task<string> GetAccessTokenAsync() {
            var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_payPalSettings.ClientId}:{_payPalSettings.ClientSecret}"));

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

            var requestBody = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");

            // Send the request to PayPal OAuth2 token endpoint
            var tokenResponse = await _client.PostAsync("/v1/oauth2/token", requestBody);

            if (!tokenResponse.IsSuccessStatusCode) {
                throw new Exception("Failed to retrieve PayPal access token");
            }

            // Parse the JSON response
            var jsonResponse = await tokenResponse.Content.ReadAsStringAsync();
            var tokenData = JsonConvert.DeserializeObject<PayPalAccessTokenResponse>(jsonResponse);

            return tokenData.AccessToken; // Return the access token
        }

        // Helper class to deserialize the token response
        private class PayPalAccessTokenResponse {
            [JsonProperty("access_token")]
            public string AccessToken { get; set; }

            [JsonProperty("expires_in")]
            public int ExpiresIn { get; set; }
        }

        public async Task SetupPayPalClientAsync() {
            // Get access token
            string accessToken = await GetAccessTokenAsync();

            // Set the Authorization header with Bearer token
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        public async Task<HttpResponseMessage> GetAsync(string endpoint) {
            // Call SetupPayPalClientAsync to configure the client before making a request
            await SetupPayPalClientAsync();

            // Make a request to the PayPal API endpoint (e.g., creating a payment)
            HttpResponseMessage response = await _client.GetAsync(endpoint); // Example API call

            return response;
        }

        public async Task<HttpResponseMessage> PostAsync(string endpoint, StringContent requestBody) {
            // Call SetupPayPalClientAsync to configure the client before making a request
            await SetupPayPalClientAsync();

            // Make a request to the PayPal API endpoint (e.g., creating a payment)
            HttpResponseMessage response = await _client.PostAsync(endpoint, requestBody); // Example API call

            return response;
        }
    }
}
