    using System;
    using System.ComponentModel.DataAnnotations;
namespace PhotocopyRevaluationAppMVC.Models.Attributes_Validations
{

    public class PasswordMustContainDigitAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is string password && !HasDigit(password))
            {
                return new ValidationResult("Password must contain at least one digit.");
            }
            return ValidationResult.Success;
        }

        private bool HasDigit(string input) => input.Any(char.IsDigit);
    }
}
