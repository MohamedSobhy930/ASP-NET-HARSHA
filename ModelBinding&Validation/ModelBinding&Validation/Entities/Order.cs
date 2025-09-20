using System.ComponentModel.DataAnnotations;
using ModelBinding_Validation.CustomValidations;

namespace ModelBinding_Validation.Entities
{
    public class Order
    {
        public int? OrderNo { get; set; }
        [Required(ErrorMessage = "{0} can't be blank")]
        [OrderDateCheck]
        public DateTime OrderDate { get; set; }
        [Required]
        [InvoicePriceCheck]
        [Range(1, double.MaxValue, ErrorMessage = "{0} should be between a valid number")]
        public double? InvoicePrice { get; set; }
        public List<Product> Products { get; set; }
    }
}
