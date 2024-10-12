using System.ComponentModel.DataAnnotations;

namespace PhotocopyRevaluationAppMVC.Models
{
    public class RegisteredUserDTO
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        [MaxLength(255)] // Typically, 255 characters for email
        public string? Email { get; set; }

        [Phone(ErrorMessage = "Invalid Phone Number.")]
        [Display(Name = "Phone Number")]
        [MaxLength(15)] // Adjust according to the expected format of phone numbers
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }  // Store hashed password

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string? ConfirmPassword { get; set; }  // Store hashed password

        [Required(ErrorMessage = "OTP is required.")]
        [MaxLength(6, ErrorMessage = "OTP cannot exceed 6 characters.")]
        public string? OTP { get; set; }
    }
}
