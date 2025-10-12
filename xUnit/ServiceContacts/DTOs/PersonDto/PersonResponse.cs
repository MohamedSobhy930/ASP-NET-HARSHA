using Entities;
using ServiceContacts.DTOs.CountryDto;
using ServiceContacts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContacts.DTOs.PersonDto
{
    public class PersonResponse
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public Guid? CountryId { get; set; }
        public string? Country {  get; set; }
        public string? Address { get; set; }
        public bool? ReceiveNewsletter { get; set; }
        public double? Age { get; set; }
        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != typeof(PersonResponse)) return false;
            PersonResponse other = obj as PersonResponse;
            return Id == other.Id &&
                Name == other.Name &&
                Email == other.Email && 
                DateOfBirth == other.DateOfBirth &&
                Gender == other.Gender &&
                CountryId == other.CountryId &&
                Address == other.Address &&
                ReceiveNewsletter == other.ReceiveNewsletter
                ;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public PersonUpdateRequest ToPersonUpdateRequest()
        {
            return new PersonUpdateRequest()
            { 
                Id = Id,
                Name = Name,
                Email = Email,
                CountryId = CountryId,
            };

        }
    }
        public static class PersonExtensions
        {
            public static PersonResponse ToPersonResponse(this Person person)
            {
                return new PersonResponse()
                {
                    Id = person.PersonId,
                    Name = person.Name,
                    Email = person.Email,
                    DateOfBirth = person.DateOfBirth,
                    Gender = person.Gender,
                    Address = person.Address,
                    ReceiveNewsletter = person.ReceiveNewsletter,
                    CountryId = person.CountryId,
                    Age = (person.DateOfBirth != null) ? Math.Round((DateTime.Now - person.DateOfBirth.Value).TotalDays / 365) : null,
                };
            }
        }
}
