    using System;
    using System.ComponentModel.DataAnnotations;
namespace PhotocopyRevaluationAppMVC.Models.Attributes_Validations
{

    public class PasswordValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string password = value as string;

            if (string.IsNullOrEmpty(password))
            {
                return new ValidationResult("Password cannot be empty.");
            }

            if (!HasDigit(password))
            {
                return new ValidationResult("Password must contain at least one digit.");
            }

            if (!HasUpperCase(password))
            {
                return new ValidationResult("Password must contain at least one uppercase letter.");
            }

            if (!HasLowerCase(password))
            {
                return new ValidationResult("Password must contain at least one lowercase letter.");
            }

            return ValidationResult.Success;
        }

        private bool HasDigit(string input) => input.Any(char.IsDigit);

        private bool HasUpperCase(string input) => input.Any(char.IsUpper);

        private bool HasLowerCase(string input) => input.Any(char.IsLower);
    }

}
