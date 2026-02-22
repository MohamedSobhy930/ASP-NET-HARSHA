using Entities;
using System.ComponentModel.DataAnnotations;

namespace ServiceContacts.DTOs.PersonDto
{
    public class PersonUpdateRequest
    {
        [Required(ErrorMessage ="Id can't be blank")]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Person Name can't be blank")]
        public string? Name { get; set; }
        [Required(ErrorMessage = "Email can't be blank")]
        [EmailAddress(ErrorMessage = "please Enter a Valid Email")]
        public string? Email { get; set; }
        [Required]
        public Guid? CountryId { get; set; }

        public Person ToPerson()
        {
            return new Person
            {
                PersonId = Id,
                Name = Name,
                Email = Email,
                CountryId = CountryId
            };
        }
    }
}
