using System.ComponentModel.DataAnnotations;

namespace ModelBinding_Validation.Entities
{
    public class Product
    {
        [Required(ErrorMessage = "{0} can't be blank")]
        public int ProductCode { get; set; }
        [Required(ErrorMessage = "{0} can't be blank")]
        [Range(0, int.MaxValue , ErrorMessage ="Product Price should be between a valid number")]
        public double Price { get; set; }
        [Required(ErrorMessage = "{0} can't be blank")]
        [Range(0, int.MaxValue , ErrorMessage = "Product Quantity should be between a valid number")]
        public int Quantity { get; set; }
    }
}
