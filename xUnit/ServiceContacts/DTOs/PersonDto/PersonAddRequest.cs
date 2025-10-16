using Entities;
using ServiceContacts.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContacts.DTOs.PersonDto
{
    public class PersonAddRequest
    {
        [Required(ErrorMessage ="Person Name can't be blank")] 
        public string? Name { get; set; }
        [Required(ErrorMessage = "Email can't be blank")]
        [EmailAddress]
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public GenderOptions? Gender { get; set; }
        public Guid? CountryId { get; set; }
        public string? Address { get; set; }
        public bool ReceiveNewsletter { get; set; }

        public Person ToPerson()
        {
            return new Person
            {
                Name = this.Name,
                Email = this.Email,
                DateOfBirth = this.DateOfBirth,
                Gender = this.Gender.ToString(),
                CountryId = this.CountryId,
                Address = this.Address,
                ReceiveNewsletter = this.ReceiveNewsletter
            };
        }
    }
}
