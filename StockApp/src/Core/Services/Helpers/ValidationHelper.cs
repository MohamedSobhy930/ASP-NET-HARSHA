using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

public static class ValidationHelper
{
    /// <summary>
    /// Validates an object based on its DataAnnotations.
    /// </summary>
    public static void ModelValidation(object obj)
    {
        ValidationContext validationContext = new ValidationContext(obj);
        List<ValidationResult> validationResults = new List<ValidationResult>();

        // Context for the validation
        bool isValid = Validator.TryValidateObject(obj, validationContext,validationResults,true);

        if (!isValid)
            throw new ArgumentException(validationResults.FirstOrDefault()?.ErrorMessage);
        
    }
}
