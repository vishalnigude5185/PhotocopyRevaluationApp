using System.ComponentModel.DataAnnotations;

namespace PhotocopyRevaluationAppMVC.Models.Attributes_Validations
{
    public class PasswordMustContainUpperCaseAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is string password && !HasUpperCase(password))
            {
                return new ValidationResult("Password must contain at least one uppercase letter.");
            }
            return ValidationResult.Success;
        }

        private bool HasUpperCase(string input) => input.Any(char.IsUpper);
    }
}
