using System;
    using System.ComponentModel.DataAnnotations;
    using System.Web;
namespace PhotocopyRevaluationAppMVC.Models.Attributes_Validations
{
    //This encodes the property value and assigns it back to the property during model binding.
    public class HtmlEncodeAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is string stringValue)
            {
                // Encode the string value
                var encodedValue = HttpUtility.HtmlEncode(stringValue);

                // Set the encoded value back to the property
                var property = validationContext.ObjectType.GetProperty(validationContext.MemberName);
                property.SetValue(validationContext.ObjectInstance, encodedValue);
            }

            return ValidationResult.Success;
        }
    }
}
