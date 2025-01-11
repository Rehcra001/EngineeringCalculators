using System.ComponentModel.DataAnnotations;

namespace EngineeringCalculators.Web.CustomValidations
{
    public class GreaterThanZeroDouble : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {

            if (value is double val)
            {
                if (val >= double.Epsilon)
                {
                    return ValidationResult.Success;
                }
            }

            return new ValidationResult($"The {validationContext.DisplayName} field must be greater than zero");
        }
    }
}
