    using System;
    using System.ComponentModel.DataAnnotations;
namespace PhotocopyRevaluationAppMVC.Models.Attributes_Validations
{

    public class DateNotInFutureAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime date && date > DateTime.UtcNow)
            {
                return new ValidationResult("Date of Birth cannot be in the future.");
            }
            return ValidationResult.Success;
        }
    }
}
