using System.ComponentModel.DataAnnotations;

namespace PhotocopyRevaluationAppMVC.Models.Attributes_Validations
{
    //Email Domain Validation
    //This attribute checks if an email belongs to a specific domain.
    public class EmailDomainAttribute : ValidationAttribute
    {
        private readonly string _allowedDomain;

        public EmailDomainAttribute(string allowedDomain)
        {
            _allowedDomain = allowedDomain;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var email = value.ToString();
            if (!string.IsNullOrEmpty(email) && email.Contains("@"))
            {
                string domain = email.Split('@')[1];
                if (domain.ToLower() == _allowedDomain.ToLower())
                {
                    return ValidationResult.Success;
                }
            }
            return new ValidationResult($"Email domain must be {_allowedDomain}");
        }
    }
}
