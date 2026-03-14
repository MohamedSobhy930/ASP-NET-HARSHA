using System.ComponentModel.DataAnnotations;

namespace CitiesManager.WebApi.Models
{
    public class City
    {
        [Key]
        public Guid Id { get; set; }
        public string? Name { get; set; }
    }
}
