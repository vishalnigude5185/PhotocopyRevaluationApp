    using System;
    using System.Linq;
    using System.ComponentModel.DataAnnotations;
using Humanizer;
using Razorpay.Api;
namespace PhotocopyRevaluationAppMVC.Models.Attributes_Validations
{
    //String Trimming Attribute
    //This custom attribute automatically trims string properties before they are saved to the database.
    public class TrimAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var stringValue = value as string;
            if (!string.IsNullOrEmpty(stringValue))
            {
                var property = validationContext.ObjectType.GetProperty(validationContext.MemberName);
                if (property != null && property.PropertyType == typeof(string))
                {
                    string trimmedValue = stringValue.Trim();
                    property.SetValue(validationContext.ObjectInstance, trimmedValue, null);
                }
            }
            return ValidationResult.Success;
        }
    }
}
