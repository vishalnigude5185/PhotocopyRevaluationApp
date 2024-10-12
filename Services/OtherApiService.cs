using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace PhotocopyRevaluationAppMVC.Services
{
    public class OtherApiService
    {
        private readonly HttpClient _client;
        public OtherApiService() 
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri("https://api.sandbox.paypal.com") // Sandbox environment
            };
        }

        public async void HttpClient()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://api.example.com/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await client.GetAsync("/api/resource");
            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                // Process the response
            }

        }
    }
}
