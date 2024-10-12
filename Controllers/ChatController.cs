using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace PhotocopyRevaluationAppMVC.Controllers
{
    public class ChatController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public ChatController(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public IActionResult Chat()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetToken()
        {
            var directLineSecret = _configuration["DirectLineSecret"];
            var client = _httpClientFactory.CreateClient();

            var request = new HttpRequestMessage(HttpMethod.Post, "https://directline.botframework.com/v3/directline/tokens/generate")
            {
                Headers = { { "Authorization", $"Bearer {directLineSecret}" } }
            };

            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var tokenObj = JsonDocument.Parse(json).RootElement.GetProperty("token").GetString();

                return Ok(new { token = tokenObj });
            }

            return StatusCode(500, "Failed to generate token");
        }
    }
}
