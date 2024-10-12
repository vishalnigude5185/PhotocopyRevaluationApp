using System.ComponentModel.DataAnnotations;

namespace PhotocopyRevaluationAppMVC.Models.Attributes_Validations
{
    public class PasswordMustContainLowerCaseAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is string password && !HasLowerCase(password))
            {
                return new ValidationResult("Password must contain at least one lowercase letter.");
            }
            return ValidationResult.Success;
        }

        private bool HasLowerCase(string input) => input.Any(char.IsLower);
    }
}
