using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace PhotocopyRevaluationApp.Models {
    public class ApplicationUser : IdentityUser, IValidatableObject /*IEquatable<ApplicationUser>*/
    {
        [Key]
        public int ApplicationUserId { get; set; }

        [Required(ErrorMessage = "ApplicationUser Name is required.")]
        [StringLength(100, ErrorMessage = "ApplicationUser Name cannot exceed 100 characters.")]
        [Display(Name = "User Name")]
#nullable enable
        public string? _userName;
        public override string? UserName {
            get => _userName;
            set {
                if (!string.IsNullOrWhiteSpace(value)) {
                    // Check if the username already exists in the database
                    //var existingUser = _userManager.FindByNameAsync(value).Result;
                    //if (existingUser != null) {
                    //    throw new InvalidOperationException("The username already exists.");
                    //}
                }
                _userName = value?.Trim(); // Example: Trimming whitespace
            }
        }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string PasswordHash { get; set; }  // Store hashed password

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Invalid Phone Number.")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        public List<Notification> Notifications { get; set; }

        [Range(0, 120, ErrorMessage = "Age must be between 0 and 120.")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Date of Birth is required.")]
        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "OTP is required.")]
        [MaxLength(6, ErrorMessage = "OTP cannot exceed 6 characters.")]
        public string? OTP { get; set; }

        //[Required(ErrorMessage = "Role is required.")]
        [Display(Name = "Role")]
        public string? Role { get; set; }

        [NotMapped]
        public bool RememberMe { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            // Ensure the Date of Birth is not in the future
            if (DateOfBirth > DateTime.UtcNow) {
                yield return new ValidationResult("Date of Birth cannot be in the future.", new[] { nameof(DateOfBirth) });
            }

            // Ensure PasswordHash contains at least one digit
            if (!string.IsNullOrEmpty(PasswordHash) && !HasDigit(PasswordHash)) {
                yield return new ValidationResult("Password must contain at least one digit.", new[] { nameof(PasswordHash) });
            }

            // Ensure PasswordHash contains at least one uppercase letter
            if (!string.IsNullOrEmpty(PasswordHash) && !HasUpperCase(PasswordHash)) {
                yield return new ValidationResult("Password must contain at least one uppercase letter.", new[] { nameof(PasswordHash) });
            }

            // Ensure PasswordHash contains at least one lowercase letter
            if (!string.IsNullOrEmpty(PasswordHash) && !HasLowerCase(PasswordHash)) {
                yield return new ValidationResult("Password must contain at least one lowercase letter.", new[] { nameof(PasswordHash) });
            }

            // Ensure Email is unique (pseudo-code)
            if (EmailAlreadyExists(Email)) {
                yield return new ValidationResult("Email is already in use.", new[] { nameof(Email) });
            }
        }

        private bool HasDigit(string input) {
            foreach (char c in input) {
                if (char.IsDigit(c)) {
                    return true;
                }
            }
            return false;
        }

        private bool HasUpperCase(string input) {
            foreach (char c in input) {
                if (char.IsUpper(c)) {
                    return true;
                }
            }
            return false;
        }

        private bool HasLowerCase(string input) {
            foreach (char c in input) {
                if (char.IsLower(c)) {
                    return true;
                }
            }
            return false;
        }

        // Pseudo-code for checking if email exists
        private bool EmailAlreadyExists(string email) {
            // Implement your logic to check email existence in the database
            // Query the database to check if the email is already in use
            // Example return value
            return false; //await _context.Users.AnyAsync(u => u.Email == email);
        }
    }
}
