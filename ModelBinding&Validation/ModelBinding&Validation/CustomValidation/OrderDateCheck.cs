using System.ComponentModel.DataAnnotations;

namespace ModelBinding_Validation.CustomValidations
{
    public class OrderDateCheck : ValidationAttribute
    {
        private readonly DateTime _minDate = new DateTime(2000, 1, 1);

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            if (value is DateTime date)
            {
                if (date < _minDate)
                {
                    return new ValidationResult(
                        $"Order date must be on or after {_minDate:yyyy-MM-dd}."
                    );
                }
            }
            else
            {
                return new ValidationResult("Invalid date format.");
            }

            return ValidationResult.Success;
        }
    }

}
