using ModelBinding_Validation.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ModelBinding_Validation.CustomValidations
{
    public class InvoicePriceCheckAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var order = (Order)validationContext.ObjectInstance;

            if (order.Products == null || !order.Products.Any())
            {
                return new ValidationResult("Order must contain at least one product.");
            }

            var total = order.Products.Sum(p => p.Price * p.Quantity);

            if (order.InvoicePrice != total)
            {
                return new ValidationResult(
                    $"InvoicePrice ({order.InvoicePrice}) must equal total product cost ({total})."
                );
            }

            return ValidationResult.Success;
        }
    }
}
