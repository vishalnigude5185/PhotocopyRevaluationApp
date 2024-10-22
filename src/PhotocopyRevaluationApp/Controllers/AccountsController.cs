using System.Data;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PhotocopyRevaluationApp.Data;
using PhotocopyRevaluationApp.Models;
using PhotocopyRevaluationApp.Services;

namespace PhotocopyRevaluationApp.Controllers {
    public class AccountsController : Controller {
        private readonly PhotocopyRevaluationAppContext _context;
        private readonly GenerateUidService _generateUidService;
        private readonly IOtpService _otpService;
        private readonly ILogger<AccountsController> _logger;
        private readonly ITimerService _timerService;
        private readonly INotificationService _notificationService;
        private readonly IConfiguration _configuration;
        private readonly IUserSessionService _userSessionService;
        private readonly IAccountService _accountService;
        //private readonly MySettings _mySettings;

        //private readonly IAccountService _accountService;

        public AccountsController(PhotocopyRevaluationAppContext context, IUserSessionService userSessionService,/*MySettings mySettings,*/ IConfiguration configuration, IAccountService accountService, GenerateUidService generateUidService, ILogger<AccountsController> logger, ITimerService timerService, INotificationService notificationService, IOtpService otpService) {
            _logger = logger;
            _notificationService = notificationService;
            _generateUidService = generateUidService;
            _context = context;
            _configuration = configuration;
            _userSessionService = userSessionService;
            //_mySettings = mySettings;
            _accountService = accountService;
            _timerService = timerService;
            _otpService = otpService;
        }

        //[HttpGet]
        //public IActionResult GetAuthStatus()
        //{
        //    var isAuthenticated = User.Identity.IsAuthenticated;
        //    if (isAuthenticated)
        //    {
        //        return Ok(new { isAuthenticated });

        //    }
        //    else
        //    {
        //        return BadRequest();
        //    }
        //}

        [HttpPost]
        public async Task<IActionResult> RegisterAsync(UserRegisterationDTO UserRegisterationDTO) {
            //    foreach (var error in result.Errors)
            //    {
            //        ModelState.AddModelError(string.Empty, error.Description);
            //    }

            //    return View(model);
            if (ModelState.IsValid) {
                // Check if the username already exists
                //var existingUser = await _accountService.FindByNameAsync(UserRegisterationDTO.UserName);
                //if (existingUser != null) {
                //    // Add a model error
                //    ModelState.AddModelError("UserName", "The username already exists.");
                //    return View(UserRegisterationDTO); // Return to the view with the error
                //}

                // Check if the email already exists
                if (await _accountService.EmailAlreadyExistsAsync(UserRegisterationDTO)) {
                    ModelState.AddModelError(nameof(UserRegisterationDTO.Email), "Email is already in use.");
                    return View(UserRegisterationDTO);
                }
                //    // If validation succeeds, proceed with user registration
                var result = await _accountService.SignupAsync(UserRegisterationDTO);
                if (result.Succeeded) {
                    // Redirect to a page instructing the user to check their email for confirmation
                    return RedirectToAction("ConfirmEmailNotice");
                }
                //    // Handle registration errors
                // Add errors to the model state
                foreach (var error in result.Errors) {
                    ModelState.AddModelError("", error.Description);
                }
            }

            // Return the view with validation errors
            return View(UserRegisterationDTO);
        }
        //[HttpPost]
        //public async Task<IActionResult> Register(UserRegisterationDTO model, [FromServices] IAccountService _accountService)
        //{
        //   
        //}

        // Action to confirm email
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token) {
            if (userId == null || token == null) {
                return View("Error");
            }

            var result = await _accountService.ConfirmEmailAsync(userId, token);
            if (result.Succeeded) {
                return View("EmailConfirmed");
            }

            // Handle errors if email confirmation fails
            return View("Error");
        }

        // Display a notice to check email
        public IActionResult ConfirmEmailNotice() {
            return View();
        }

        [HttpGet]
        public IActionResult Login() {
            var ApplicationUser = new ApplicationUser();
            return View(ApplicationUser);
        }

        // Standard login method
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password) {

            return RedirectToAction("Index", "Home");
        }

        [HttpPost("login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(ApplicationUser ApplicationUser) {
            TempData["UserName"] = ApplicationUser.UserName;
            TempData["PasswordHash"] = ApplicationUser.PasswordHash;

            // Clear the validation state for the unused fields
            ModelState.Remove("Email");
            ModelState.Remove("DateOfBirth");
            ModelState.Remove("OTP");
            ModelState.Remove("PhoneNumber");
            ModelState.Remove("Notifications");

            //// Define the fields you want to check for [Required] validation errors
            //var fieldsToCheck = new[] { "Email", "PhoneNumber", "DateOfBirth", "OTP", "Notifications" };

            //foreach (var field in fieldsToCheck)
            //{
            //    // Check if the ModelState contains any errors for the current field
            //    if (ModelState[field] != null && ModelState[field].Errors.Count > 0)
            //    {
            //        // Find and remove the error related to the [Required] validation
            //        var requiredError = ModelState[field].Errors
            //                             .FirstOrDefault(e => e.ErrorMessage.Contains("required"));

            //        if (requiredError != null)
            //        {
            //            // Remove only the [Required] validation error for the current field
            //            ModelState[field].Errors.Remove(requiredError);
            //        }
            //    }
            //}

            //// Check if the ModelState contains an error for the Email field
            //if (ModelState["Email"] != null && ModelState["Email"].Errors.Count > 0)
            //{
            //    // Find and remove the error related to the [Required] validation
            //    var requiredError = ModelState["Email"].Errors
            //                        .FirstOrDefault(e => e.ErrorMessage.Contains("required"));

            //    if (requiredError != null)
            //    {
            //        // Remove only the [Required] validation error for the Email field
            //        ModelState["Email"].Errors.Remove(requiredError);
            //    }
            //}

            if (ModelState.IsValid) {
                // Using PasswordHasher to hash the new password before saving
                var hashedPassword = "";

                // Normally, you would validate the user's credentials from a database here.
                //var record = _context.Photocopies.Find(id);
                // Get the count of records that match the condition
                //_context.Users.Where(p => p.UserName == ApplicationUser.UserName)
                //    .Select(s => hashedPassword = s.PasswordHash);

                //var passwordHasher = new PasswordHasher<ApplicationUser>();
                //var result = passwordHasher.VerifyHashedPassword(null, hashedPassword, ApplicationUser.PasswordHash);

                var count = _context.Users
                    .Count(p => p.UserName == ApplicationUser.UserName && p.PasswordHash == ApplicationUser.PasswordHash);
                if (count == 1) {
                    //var UserRecord = _context.Users
                    //    .FirstOrDefault(p => p.UserName == ApplicationUser.UserName && p.PasswordHash == ApplicationUser.PasswordHash);
                    var userRecord = _context.Users
                           .Where(p => p.UserName == ApplicationUser.UserName && p.PasswordHash == ApplicationUser.PasswordHash)
                           .Select(p => new {
                               p.ApplicationUserId,
                               p.Email,
                               p.PhoneNumber
                           })
                           .FirstOrDefault();

                    //To access in ResendOTP action
                    TempData["PhoneNumber"] = userRecord.PhoneNumber;
                    TempData["Email"] = userRecord.Email;
                    string UsserId = userRecord.ApplicationUserId.ToString();
                    TempData["UsserId"] = UsserId;

                    if (string.IsNullOrEmpty(UsserId)) {
                        UsserId = Guid.NewGuid().ToString();
                    }
                    string otp = await _otpService.GenerateOtpAsync(UsserId, TimeSpan.FromSeconds(30));

                    //if (await _otpService.SendOtpToPhoneAsync(userRecord.PhoneNumber, otp))
                    //{
                    if (await _otpService.SendOTPtoEmailAsync(userRecord.Email, otp)) {
                        //_otpService.ClearOtpAfterDelay(userRecord.ApplicationUserId, TimeSpan.FromSeconds(30));// Asynchronously clear the OTP after 30 seconds
                    }
                    // Redirect to OTP view
                    return RedirectToAction("OTPVerification", "Accounts");
                    //}
                    //else
                    //{
                    //    // Invalid credentials, show error
                    //    ModelState.AddModelError("internal error occurred", "unable to send the otp.");
                    //    return Redirect("Accounts/OTPSendingError");
                    //}
                }
                else {
                    // Invalid credentials, show error
                    ModelState.AddModelError("", "invalid credentials, incorrect user id or password.");
                    return View(ApplicationUser);
                }
            }
            else {
                //Optionally, you can iterate through the ModelState to log the errors or handle them
                foreach (var state in ModelState) {
                    foreach (var error in state.Value.Errors) {
                        // You can log the error messages or debug them
                        Console.WriteLine(error.ErrorMessage);
                    }
                }
                return View(ApplicationUser);
            }
        }

        //OTP Action
        [HttpGet("otppage")]
        public IActionResult OTPVerification() {
            return View();
        }

        //OTP Verification Action
        [HttpPost("otpverify")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OTPVerifications([FromForm][Bind("OTP")] ApplicationUser ApplicationUser) {
            // Clear the validation state for the unused fields
            //ModelState.ClearValidationState("Email");
            //ModelState.ClearValidationState("DateOfBirth");
            //ModelState.ClearValidationState("UserName");
            //ModelState.ClearValidationState("PhoneNumber");
            //ModelState.ClearValidationState("Notifications");
            //ModelState.ClearValidationState("PasswordHash");
            ModelState.Remove("Email");
            ModelState.Remove("DateOfBirth");
            ModelState.Remove("UserName");
            ModelState.Remove("PhoneNumber");
            ModelState.Remove("Notifications");
            ModelState.Remove("PasswordHash");

            //// Define the fields you want to check for [Required] validation errors
            //var fieldsToCheck = new[] { "Email", "PhoneNumber", "DateOfBirth", "UserName", "Notifications", "PasswordHash" };

            //foreach (var field in fieldsToCheck)
            //{
            //    // Check if the ModelState contains any errors for the current field
            //    if (ModelState[field] != null && ModelState[field].Errors.Count > 0)
            //    {
            //        // Find and remove the error related to the [Required] validation
            //        var requiredError = ModelState[field].Errors
            //                             .FirstOrDefault(e => e.ErrorMessage.Contains("required"));

            //        if (requiredError != null)
            //        {
            //            // Remove only the [Required] validation error for the current field
            //            ModelState[field].Errors.Remove(requiredError);
            //        }
            //    }
            //}

            if (ModelState.IsValid) {

                string UserId = (string)TempData["UsserId"];

                // Retrieve the OTP stored in session (or database)
                string? storedOtp; //= HttpContext.Session.GetString(UserId); // Retrieve OTP from session
                storedOtp = await _otpService.GetOtpAsync(UserId);
                //if (string.IsNullOrEmpty(storedOtp))
                //{
                //    storedOtp = await _otpService.GetOtpAsync(UserId);
                //}

                //// Verify OTP
                if (storedOtp != null) {
                    if (ApplicationUser.OTP == storedOtp) {
                        // OTP is valid, proceed with login
                        HttpContext.Session.Remove("OTP"); // Clear OTP from session

                        // Getting User Id
                        var userRecord = _context.Users
                          .Where(p => p.UserName == TempData["UserName"] && p.PasswordHash == TempData["PasswordHash"])
                          .Select(p => new {
                              p.ApplicationUserId,
                              p.Role
                          })
                          .FirstOrDefault();

                        if (userRecord != null && TempData["UserName"] != null) {
                            // Claims for the authenticated user
                            var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.NameIdentifier, userRecord.ApplicationUserId.ToString()),
                            new Claim(ClaimTypes.Name, (string)TempData["UserName"]),
                            new Claim(ClaimTypes.Role, userRecord.Role) // or the actual user role
                        };

                            // Create cookie-based authentication identity
                            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                            var principal = new ClaimsPrincipal(identity);

                            // Authentication properties (remember me, expiration time)
                            var authProperties = new AuthenticationProperties {
                                IsPersistent = ApplicationUser.RememberMe, // Persist cookie across sessions if RememberMe is true
                                ExpiresUtc = ApplicationUser.RememberMe ? DateTimeOffset.UtcNow.AddDays(7) : DateTimeOffset.UtcNow.AddMinutes(30)
                            };

                            // Sign in the user with cookie authentication
                            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);

                            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                            if (!string.IsNullOrEmpty(userId)) {
                                var SessionData = new SessionData {
                                    ApplicationUserId = Convert.ToInt32(userId),
                                    ExpiryTime = DateTimeOffset.UtcNow.AddMinutes(2)
                                };

                                string SessionId = Guid.NewGuid().ToString();
                                // Track the user session using SignalR
                                _userSessionService.CreateUserSessionDataAsync(SessionId, SessionData);

                                // Assuming CheckAndSendSignOutNotificationByUserIdThreeSecondsBeforeAsync returns Task
                                _timerService.StartAsync(TimeSpan.FromMilliseconds(30000), () => _notificationService.CheckAndSendSignOutNotificationByUserIdThreeSecondsBeforeAsync());

                                //TimeSpan timeInterval = TimeSpan.FromSeconds(30); // Example interval
                                //await _timerService.StartAsync(timeInterval, DoWorkAsync, HttpContext.RequestAborted);
                                _timerService.StartAsync(TimeSpan.FromMilliseconds(60000), _notificationService.CheackAndSendSignOutNotificationByUserIdThreeMinutesBeforeAsync);

                                Console.WriteLine($"Session Data for {SessionId} is: {_userSessionService.GetSessionDataBySessionId<SessionData>(SessionId)}");
                                //// After signing in, store the user's session in cache
                                //await _userSessionService.CreateUserSessionAsync(userId, Guid.NewGuid().ToString());
                            }
                            var userName = principal.FindFirst(ClaimTypes.Name)?.Value; // or User.Identity.Name;  // Access Username
                            if (!string.IsNullOrEmpty(userName)) {
                                // Optionally add a separate cookie for other information (e.g., username)
                                var cookieOptions = new CookieOptions {
                                    Expires = DateTime.Now.AddDays(1)
                                };
                                HttpContext.Response.Cookies.Append("userId", userId, cookieOptions);

                                // Generate JWT token
                                var token = _generateUidService.GenerateJwtToken(userName);
                                // Send the token in the Authorization header
                                Response.Headers.Add("Authorization", "Bearer " + token);
                            }
                            else {
                                Console.WriteLine("can not create the JWT Token or userId coockie userId or userName is null");
                                ModelState.AddModelError("", "can not create the JWT Token or userId coockie userId or userName is null");
                                return View(ApplicationUser);
                            }
                        }
                        else {
                            Console.WriteLine("can not create the JWT Token or userId coockie userId or userName is null");
                            ModelState.AddModelError("userRecord or UserName is null", "can't create the Claims");
                            return View(ApplicationUser);
                        }
                        return RedirectToAction("Index", "Home"); // Redirect to the home page
                                                                  // return Unauthorized(new { Message = "Invalid OTP" });
                    }
                    else {
                        // Invalid OTP, show error
                        ModelState.AddModelError("", "Invalid OTP.");
                        return View("OTPVerification");
                    }
                }
                else {
                    // Invalid OTP, show error
                    ModelState.AddModelError("Invalid OTP", "OTP has been expired.");
                    return View("OTPVerification");
                }
            }
            else {
                return View(ApplicationUser);
            }
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendOtpAsync(ITempDataService _tempDataService) {
            await _otpService.ResendOtpAsync(_tempDataService);

            return Ok(); // Return a response that indicates success
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ForgotPasswordOnGet([FromForm] ApplicationUser ApplicationUser) {
            if (!string.IsNullOrEmpty(ApplicationUser.UserName)) {
                HttpContext.Session.SetString("username", ApplicationUser.UserName);
                TempData["UserName"] = ApplicationUser.UserName;

                return View("ForgotPassword");
            }
            else {
                ModelState.AddModelError("required", "To forgot password user name is required");
                return View("Login");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(RegisteredUserDTO RegisteredUserDTO, [FromServices] IAccountService _accountService) {
            HttpContext.Session.SetString("mobileno", RegisteredUserDTO.PhoneNumber);
            HttpContext.Session.SetString("email", RegisteredUserDTO.Email);

            TempData["mobileno"] = RegisteredUserDTO.PhoneNumber;
            TempData["email"] = RegisteredUserDTO.Email;

            ModelState.Remove("DateOfBirth");
            ModelState.Remove("OTP");
            ModelState.Remove("NewPassword");
            ModelState.Remove("ConfirmPassword");
            ModelState.Remove("Notifications");

            if (ModelState.IsValid) {
                if (await _accountService.IsValidPhoneNumberAsync(RegisteredUserDTO.PhoneNumber) || await _accountService.IsValidEmailAsync(RegisteredUserDTO.Email)) {
                    var count = _context.Users.Count(p => p.UserName == TempData["UserName"] && p.PhoneNumber == RegisteredUserDTO.PhoneNumber && p.Email == RegisteredUserDTO.Email);

                    if (count == 1) {
                        string UsserId = Guid.NewGuid().ToString();
                        if (string.IsNullOrEmpty(UsserId)) {
                            UsserId = Guid.NewGuid().ToString();
                        }
                        string otp = await _otpService.GenerateOtpAsync(UsserId, TimeSpan.FromSeconds(30));
                        // Save OTP to session or database for later verification
                        if (await _otpService.SendOtpToPhoneAsync(RegisteredUserDTO.PhoneNumber, otp)) {
                            if (await _otpService.SendOTPtoEmailAsync(RegisteredUserDTO.Email, otp)) {
                                return RedirectToAction("OTPVerificationsForForgotPassword", "Accounts");
                            }
                            // Redirect to OTP view
                            return RedirectToAction("OTPVerificationsForForgotPassword", "Accounts");
                        }
                        //else
                        //{
                        //    // Invalid credentials, show error
                        //    ModelState.AddModelError("", "to send otp accountSid & authToken are not accesible from envirnoment variables.");
                        //    return View(RegisteredUserDTO);

                        //}
                        return Redirect("Accounts/OTPSendingError");
                    }
                    else {
                        // Invalid credentials, show error
                        ModelState.AddModelError("", "invalid, incorrect user name, mobile no or email.");
                        return View(RegisteredUserDTO);
                    }
                }
                else {
                    // Invalid credentials, show error
                    ModelState.AddModelError("", "invalid format, incorrect mobile no or email.");
                    return View(RegisteredUserDTO);
                }
            }
            else {
                //Optionally, you can iterate through the ModelState to log the errors or handle them
                foreach (var state in ModelState) {
                    foreach (var error in state.Value.Errors) {
                        // You can log the error messages or debug them
                        Console.WriteLine(error.ErrorMessage);
                    }
                }
                return View(RegisteredUserDTO);
            }
        }

        [HttpGet]
        public IActionResult OTPVerificationsForForgotPassword() {
            return View("OTPVerificationsForForgotPassword");
        }

        //OTP Verification Action
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult OTPVerificationsForForgotPassword([Bind("OTP")] RegisteredUserDTO RegisteredUserDTO) {
            //// Clear the validation state for the unused fields
            //ModelState.ClearValidationState("Email");
            //ModelState.ClearValidationState("DateOfBirth");
            //ModelState.ClearValidationState("PhoneNumber");
            //ModelState.ClearValidationState("NewPassword");
            //ModelState.ClearValidationState("ConfirmPassword");
            //ModelState.ClearValidationState("Notifications");
            ModelState.Remove("Email");
            ModelState.Remove("DateOfBirth");
            ModelState.Remove("PhoneNumber");
            ModelState.Remove("NewPassword");
            ModelState.Remove("ConfirmPassword");
            ModelState.Remove("Notifications");

            if (ModelState.IsValid) {
                //// Verify OTP
                // Retrieve the OTP stored in session (or database)
                string? storedOtp = HttpContext.Session.GetString("OTP"); // Retrieve OTP from session
                if (storedOtp != null) {
                    if (RegisteredUserDTO.OTP == storedOtp) {
                        // OTP is valid, proceed with login
                        HttpContext.Session.Remove("OTP"); // Clear OTP from session


                        return RedirectToAction("ChangePassword", "Accounts"); // Redirect to the home page
                                                                               // return Unauthorized(new { Message = "Invalid OTP" });
                    }
                    else {
                        // Invalid OTP, show error
                        ModelState.AddModelError("", "Invalid OTP.");
                        return View(RegisteredUserDTO);
                    }
                }
                else {
                    // Invalid OTP, show error
                    ModelState.AddModelError("", "Unable to get OTP from Sessions collection.");
                    return View(RegisteredUserDTO);
                }
            }
            else {
                // Optionally, you can iterate through the ModelState to log the errors or handle them
                foreach (var state in ModelState) {
                    foreach (var error in state.Value.Errors) {
                        // You can log the error messages or debug them
                        Console.WriteLine(error.ErrorMessage);
                    }
                }
                // Return the model with validation errors to the view
                return View(RegisteredUserDTO);
            }
        }

        [HttpGet]
        public IActionResult ChangePassword() {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword([Bind("NewPassword,ConfirmPassword")] RegisteredUserDTO RegisteredUserDTO, [FromServices] IAccountService _accountService) {
            ModelState.Remove("UserName");
            ModelState.Remove("Email");
            ModelState.Remove("DateOfBirth");
            ModelState.Remove("PhoneNumber");
            ModelState.Remove("OTP");
            ModelState.Remove("Notifications");

            if (ModelState.IsValid) {
                // Claims for the authenticated user

                if (RegisteredUserDTO.NewPassword == RegisteredUserDTO.ConfirmPassword) {
                    var UserName = HttpContext.Session.GetString("username");
                    var PhoneNumber = HttpContext.Session.GetString("mobileno");
                    var Email = HttpContext.Session.GetString("email");

                    HttpContext.Session.Remove("username");
                    HttpContext.Session.Remove("mobileno");
                    HttpContext.Session.Remove("email");

                    // Using PasswordHasher to hash the new password before saving
                    //var passwordHasher = new PasswordHasher<ApplicationUser>();
                    //var PasswordHash = passwordHasher.HashPassword(null, RegisteredUserDTO.NewPassword);

                    var PasswordHash = RegisteredUserDTO.NewPassword;

                    // Initialize the output parameter
                    var rowsAffected = new SqlParameter("@RowsAffected", SqlDbType.Int) {
                        Direction = ParameterDirection.Output
                    };

                    await _context.Database.ExecuteSqlRawAsync(
                        "EXEC UpdateUserPasswordHash @PasswordHash, @UserName, @PhoneNumber, @Email, @RowsAffected OUTPUT",
                        new SqlParameter("@PasswordHash", PasswordHash),
                        new SqlParameter("@UserName", UserName),
                        new SqlParameter("@PhoneNumber", PhoneNumber),
                        new SqlParameter("@Email", Email),
                        rowsAffected // Include the output parameter here
                    );
                    // Access the output value and cast it properly
                    // Correct way to access the output parameter's value
                    if (Convert.ToInt32(rowsAffected.Value) > 0) {
                        // Getting User Id
                        var userRecord = _context.Users
                          .Where(p => p.UserName == UserName && p.PasswordHash == PasswordHash)
                          .Select(p => new {
                              p.ApplicationUserId,
                              p.Role
                          })
                          .FirstOrDefault();

                        if (!(userRecord == null)) {
                            // Claims for the authenticated user
                            var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.NameIdentifier, userRecord.ApplicationUserId.ToString()),
                            new Claim(ClaimTypes.Name, UserName),
                            new Claim(ClaimTypes.Role, userRecord.Role) // or the actual user role
                        };

                            // Create cookie-based authentication identity
                            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                            var principal = new ClaimsPrincipal(identity);

                            // Authentication properties (remember me, expiration time)
                            //var authProperties = new AuthenticationProperties
                            //{
                            //    IsPersistent = ApplicationUser.RememberMe, // Persist cookie across sessions if RememberMe is true
                            //    ExpiresUtc = ApplicationUser.RememberMe ? DateTimeOffset.UtcNow.AddDays(7) : DateTimeOffset.UtcNow.AddMinutes(30)
                            //};

                            // Sign in the user with cookie authentication
                            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal /*, authProperties*/);

                            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                            if (!string.IsNullOrEmpty(userId)) {
                                var SessionData = new SessionData {
                                    ApplicationUserId = Convert.ToInt32(userId),
                                    ExpiryTime = DateTimeOffset.UtcNow.AddMinutes(4)
                                };
                                // Track the user session using SignalR
                                //// After signing in, store the user's session in cache
                                string SessionId = Guid.NewGuid().ToString();
                                // Track the user session using SignalR
                                _userSessionService.CreateUserSessionDataAsync(SessionId, SessionData);

                                // Assuming CheckAndSendSignOutNotificationByUserIdThreeSecondsBeforeAsync returns Task
                                _timerService.StartAsync(TimeSpan.FromMilliseconds(30000), () => _notificationService.CheckAndSendSignOutNotificationByUserIdThreeSecondsBeforeAsync());

                                //TimeSpan timeInterval = TimeSpan.FromSeconds(30); // Example interval
                                //await _timerService.StartAsync(timeInterval, DoWorkAsync, HttpContext.RequestAborted);
                                _timerService.StartAsync(TimeSpan.FromMilliseconds(60000), _notificationService.CheackAndSendSignOutNotificationByUserIdThreeMinutesBeforeAsync);

                                Console.WriteLine($"Session Data for {SessionId} is: {_userSessionService.GetSessionDataBySessionId<SessionData>(SessionId)}");
                            }

                            var userName = principal.FindFirst(ClaimTypes.Name)?.Value; // or User.Identity.Name;  // Access Username
                            if (!string.IsNullOrEmpty(userName)) {
                                // Optionally add a separate cookie for other information (e.g., username)
                                var cookieOptions = new CookieOptions {
                                    Expires = DateTime.Now.AddDays(1)
                                };
                                HttpContext.Response.Cookies.Append("username", userName, cookieOptions);

                                // Generate JWT token
                                var token = _generateUidService.GenerateJwtToken(userName);
                            }
                            // Set a message in TempData
                            TempData["SuccessMessage"] = "Your password has been updated successfully!";

                            return Redirect("/Home/Index");
                        }
                        else {
                            ModelState.AddModelError("error: ", "unable to update the password at database.");
                            return View(RegisteredUserDTO);
                        }
                    }
                    else {
                        // Invalid credentials, show error
                        ModelState.AddModelError("can not match: ", "new password and cinfirm password.");
                        return View(RegisteredUserDTO);
                    }
                }
                else {
                    // Invalid credentials, show error
                    //ModelState.AddModelError("", "incorrect .");
                    return View(RegisteredUserDTO);
                }
            }
            else {
                // Optionally, you can iterate through the ModelState to log the errors or handle them
                foreach (var state in ModelState) {
                    foreach (var error in state.Value.Errors) {
                        // You can log the error messages or debug them
                        Console.WriteLine(error.ErrorMessage);
                    }
                }
                return View(RegisteredUserDTO);
            }
        }

        [HttpPost]
        public string? GetCurrentUserId() {
            return HttpContext?.User?.Identity?.Name; // Returns the UserName or ApplicationUserId of the logged-in user
        }

        public async Task<IActionResult> Logout() {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return /*RedirectToAction*/Redirect("/Accounts/Login");
        }

        // Find the user by username or id
        //var user = _context.Users.FirstOrDefault(u => u.UserName == UserName && u.PhoneNumber == PhoneNumber && u.Email == Email);

        //// Find the user by username or id
        //var user = _context.Users.Where(u => u.UserName == TempData["username"] && u.PhoneNumber == TempData["mobileno"] && u.Email == TempData["email"])
        //    .Select(user => new ApplicationUser
        //    {
        //        PasswordHash = user.PasswordHash
        //    })
        //    .FirstOrDefault();

        //// Find the user by username or id
        //var user = _context.Users.Where(u => u.UserName == UserName && u.PhoneNumber == PhoneNumber && u.Email == Email)
        //    .Select(user => new ApplicationUser
        //    {
        //        PasswordHash = user.PasswordHash
        //    })

        //Find the user by username or id
        //var user = _context.Users.FirstOrDefault(u => u.UserName == TempData["username"] && u.PhoneNumber == TempData["mobileno"] && u.Email == TempData["email"]);

        //            var user = _context.Users
        //.FirstOrDefault(u => u.UserName.Equals(UserName.Trim(), StringComparison.OrdinalIgnoreCase)
        //                  && u.PhoneNumber.Equals(PhoneNumber.Trim(), StringComparison.OrdinalIgnoreCase)
        //                  && u.Email.Equals(Email.Trim(), StringComparison.OrdinalIgnoreCase));

        //            var user = _context.Users
        //.FirstOrDefault(u => EF.Functions.Like(u.UserName.Trim(), UserName.Trim())
        //                  && EF.Functions.Like(u.PhoneNumber.Trim(), PhoneNumber.Trim())
        //                  && EF.Functions.Like(u.Email.Trim(), Email.Trim()));

        //            var user = _context.Users
        //.FromSqlRaw("SELECT * FROM Users WHERE UserName = {0} AND PhoneNumber = {1} AND Email = {2}", UserName.Trim(), PhoneNumber.Trim(), Email.Trim())
        //.FirstOrDefault();

        // Find the user based on UserName, PhoneNumber, and Email
        //var user = await _context.Users
        //    .FirstOrDefaultAsync(u => u.UserName == TempData["UserName"] && u.PhoneNumber == PhoneNumber && u.Email == Email);

        //if (user != null)
        //{
        //    // Using PasswordHasher to hash the new password before saving
        //    var passwordHasher = new PasswordHasher<ApplicationUser>();
        //    user.PasswordHash = passwordHasher.HashPassword(user, RegisteredUser.NewPassword);

        //    // Save changes to the database
        //    _context.SaveChanges();

        //    return Redirect("/Home/Index");
        //}
        //else
        //{
        //    // Redirect to ChangePassword view
        //    ModelState.AddModelError("internal server error", "unable to update the new password");
        //    return View(RegisteredUser);
        //}

        // Retrieve the authentication cookie (usually named ".AspNetCore.Identity.Application" or similar)
        //if (HttpContext.Request.Cookies.ContainsKey(".AspNetCore.Identity.Application"))
        //{
        //    var cookieValue = HttpContext.Request.Cookies[".AspNetCore.Identity.Application"];
        //    // Do something with the cookie value
        //    //If you're not in a controller or page (where User is directly available), you can access it via HttpContext:
        //    //var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);  // User ID
        //    //var userName = HttpContext.User.Identity.Name;  // Username
        //    var user = HttpContext.User;  // Get the current user

        //    if (user.Identity.IsAuthenticated)
        //    {
        //        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value; // Get the user ID (or other claims)
        //        var userName = user.FindFirst(ClaimTypes.Name)?.Value; // Get the username
        //        var userRole = user.FindFirst(ClaimTypes.Role)?.Value; // Get the role

        //        // You can iterate through all claims like this:
        //        foreach (var claim in user.Claims)
        //        {
        //            claim.Type;
        //            claim.Value;
        //        }
        //    }
        //}
    }
}
