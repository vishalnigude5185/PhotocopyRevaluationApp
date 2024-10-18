    using System;
    using System.ComponentModel.DataAnnotations;

namespace PhotocopyRevaluationAppMVC.Models.Attributes_Validations
{
    public class EvenNumberAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || !(value is int))
            {
                return new ValidationResult("The field must be an integer.");
            }

            int intValue = (int)value;

            // Check if the number is even
            if (intValue % 2 == 0)
            {
                return ValidationResult.Success; // Validation passed
            }
            else
            {
                return new ValidationResult("The number must be even.");
            }
        }
    }

}
