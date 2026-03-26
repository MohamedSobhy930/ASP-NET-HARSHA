using System;
using System.ComponentModel.DataAnnotations;

public class DateMustBeAfter : ValidationAttribute
{
    private readonly DateTime _minDate;

    public DateMustBeAfter(int year, int month, int day)
    {
        _minDate = new DateTime(year, month, day);
        ErrorMessage = $"Date must be on or after {_minDate:yyyy-MM-dd}.";
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is DateTime dateValue)
        {
            if (dateValue < _minDate)
            {
                return new ValidationResult(ErrorMessage);
            }
        }
        return ValidationResult.Success;
    }
}
