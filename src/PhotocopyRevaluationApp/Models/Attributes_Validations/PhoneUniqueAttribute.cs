using PhotocopyRevaluationAppMVC.Services;
using System.ComponentModel.DataAnnotations;

namespace PhotocopyRevaluationAppMVC.Models.Attributes_Validations
{
    public class PhoneUniqueAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var phoneNumber = value as string;
            var accountService = validationContext.GetService(typeof(IAccountService)) as IAccountService;
            if (accountService.IsPhoneAlreadyExistsAsync(phoneNumber))
            {
                return new ValidationResult("Phone number is already in use.");
            }
            return ValidationResult.Success;
        }
    }

}
