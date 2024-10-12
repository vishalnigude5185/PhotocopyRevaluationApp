using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using PhotocopyRevaluationAppMVC.Models;


namespace PhotocopyRevaluationAppMVC.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        private readonly IEmailSender _emailSender; // For sending confirmation emails
        private readonly ILogger<AccountService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public virtual ICollection<Notification> Notifications { get; set; }

        public AccountService(IHttpContextAccessor httpContextAccessor, IEmailSender emailSender, UserManager<ApplicationUser> userManager,
                       SignInManager<ApplicationUser> signInManager,
                       ILogger<AccountService> logger)
        {
            _emailSender = emailSender;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        // SignupAsync method with enhanced validation and email confirmation
        public async Task<IdentityResult> SignupAsync(UserRegisterationDTO UserRegisterationDTO)
        {
            var user = new ApplicationUser
            {
                UserName = UserRegisterationDTO.UserName,
                Email = UserRegisterationDTO.Email,
                PhoneNumber = UserRegisterationDTO.PhoneNumber // Optional, if needed
            };

            // Create the user
            var result = await _userManager.CreateAsync(user, UserRegisterationDTO.Password);

            if (result.Succeeded)
            {
                // Log successful signup
                _logger.LogInformation($"User {UserRegisterationDTO.UserName} successfully created.");

                // Generate email confirmation token
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                // Send email confirmation link
                var confirmationLink = $"https://yourdomain.com/confirm-email?userId={user.ApplicationUserId}&token={Uri.EscapeDataString(token)}";
                await _emailSender.SendEmailAsync(user.Email, "Confirm your email", $"Please confirm your email by clicking <a href='{confirmationLink}'>here</a>.");

                _logger.LogInformation($"Sent email confirmation link to {UserRegisterationDTO.Email}.");
            }
            else
            {
                // Log signup errors
                _logger.LogWarning($"Failed to create user {UserRegisterationDTO.UserName}. Errors: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            return result;
        }

        // Find user by email
        public async Task<ApplicationUser> FindByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }
        public async Task<bool> EmailAlreadyExistsAsync(UserRegisterationDTO UserRegisterationDTO)
        {
            // Normalize the email to match the same format as stored in the database
            var normalizedEmail = _userManager.NormalizeEmail(UserRegisterationDTO.Email);

            // Check if any user already has this email
            var user = await _userManager.FindByEmailAsync(normalizedEmail);

            // Return true if the user exists, false otherwise
            return user != null;
        }
        // Confirm email
        public async Task<IdentityResult> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            return result;
        }

        // Sign in user after successful signup or email confirmation
        public async Task<Microsoft.AspNetCore.Identity.SignInResult> SignInAsync(string email, string password, bool rememberMe)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return Microsoft.AspNetCore.Identity.SignInResult.Failed;
            }

            // Sign in the user
            return await _signInManager.PasswordSignInAsync(user, password, rememberMe, lockoutOnFailure: false);
        }

        // Reset password logic
        public async Task<IdentityResult> ResetPasswordAsync(string userId, string token, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            return await _userManager.ResetPasswordAsync(user, token, newPassword);
        }

        // Check if email is confirmed
        public async Task<bool> IsEmailConfirmedAsync(ApplicationUser user)
        {
            return await _userManager.IsEmailConfirmedAsync(user);
        }

        // Helper method to log errors
        private void LogErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                _logger.LogError($"Error: {error.Description}");
            }
        }
     //=============================================== Validation Related Logic ============================================

        ////To check mo no or email combine
        //public bool IsValidMonoOrEmail(string input)
        //{
        //    var combinedRegex = new Regex(@"^(?:[a-zA-Z0-9\s\-']+|[^\s@]+@[^\s@]+\.[^\s@]+)$");
        //    return combinedRegex.IsMatch(input);
        //}
        public async Task<bool> IsValidPhoneNumberAsync(string input)
        {
            var phoneRegex = new Regex(@"^\+?[1-9]\d{1,14}$");
            return phoneRegex.IsMatch(input);
        }

        public async Task<bool> IsValidEmailAsync(string email)
        {
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase);
            return emailRegex.IsMatch(email);
        }
    }
}
