using PhotocopyRevaluationAppMVC.Services;
using System;
    using System.ComponentModel.DataAnnotations;
namespace PhotocopyRevaluationAppMVC.Models.Attributes_Validations
{

    public class EmailUniqueAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var email = value as string;
            // Assuming you have a UserService with a method to check email existence
            var userService = validationContext.GetService(typeof(IAccountService)) as IAccountService;
            if (userService.IsEmailAlreadyExistsAsync(email))
            {
                return new ValidationResult("Email is already in use.");
            }
            return ValidationResult.Success;
        }
    }

}
