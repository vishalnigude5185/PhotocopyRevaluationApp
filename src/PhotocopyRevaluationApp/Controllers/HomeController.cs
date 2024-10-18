using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PhotocopyRevaluationApp.Data;
using System.Diagnostics;

namespace PhotocopyRevaluationApp.Controllers {
    ////Using Cookie Authentication
    //[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]

    ////Using JWT Authentication for API Calls
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    //[ApiController]
    //[Route("api/[controller]")]

    ////Use Both Schemes Simultaneously(Optional)
    //[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme + "," + JwtBearerDefaults.AuthenticationScheme)]

    //[Authorize]
    public class HomeController : Controller {
        private readonly PhotocopyRevaluationAppContext _context;

        private readonly ILogger<HomeController> _logger;

        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(IHttpClientFactory httpClientFactory, PhotocopyRevaluationAppContext context, ILogger<HomeController> logger) {
            _httpClientFactory = httpClientFactory;

            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> reCAPTCHA_demo() {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Submit(string recaptchaResponse) {
            var client = _httpClientFactory.CreateClient();
            var secretKey = "6LdkXb0pAAAAAJnN63ngdpoFXa_x5bXwj0bIUMUP"; // Replace with your Secret Key

            var response = await client.PostAsync($"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={recaptchaResponse}", null);
            var jsonResponse = await response.Content.ReadAsStringAsync();

            dynamic jsonData = JsonConvert.DeserializeObject(jsonResponse);
            var success = jsonData.success;

            if (success) {
                // The reCAPTCHA was successful; process the form data.
                return RedirectToAction("Success"); // Or do something else
            }
            else {
                // The reCAPTCHA verification failed.
                ModelState.AddModelError("", "reCAPTCHA verification failed. Please try again.");
                return View(); // Return to the form with an error
            }
        }

        public IActionResult Index() {
            return View();
        }
        [HttpPost]

        public IActionResult Privacy() {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
