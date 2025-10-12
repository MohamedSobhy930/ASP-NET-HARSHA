using Entities;
using ServiceContacts;
using ServiceContacts.DTOs.PersonDto;
using ServiceContacts.Enums;
using Services.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class PersonService : IPersonService
    {
        private readonly List<Person> _persons;
        private readonly ICountriesService _countriesService;
        
        public PersonService() 
        {
            _persons = new List<Person>();
            _countriesService = new CountriesService();
        }

        public PersonResponse AddPerson(PersonAddRequest request)
        {
            if(request == null) 
                throw new ArgumentNullException(nameof(request));

            ValidationHelper.ModelValidation(request);

            Person person = request.ToPerson();
            person.PersonId = new Guid();
            _persons.Add(person);
            PersonResponse personResponse = person.ToPersonResponse();
            personResponse.Country = (_countriesService.GetCountryById(person.CountryId))?.Name;
            return personResponse;
        }

        public bool DeletePerson(Guid? id)
        {
            if(id == null) 
                throw new ArgumentNullException(nameof(id));
            var person = _persons.FirstOrDefault(p => p.PersonId == id);
            if (person == null)
                return false;
            return _persons.Remove(person);   
        }

        public List<PersonResponse> GetAllPersons()
        {
            if(_persons.Count == 0)
                return new List<PersonResponse>();
           var personResponses = new List<PersonResponse>();
            foreach (var person in _persons)
            {
                personResponses.Add(person.ToPersonResponse());
            }
            return personResponses;
        }

        public List<PersonResponse> GetFilteredPersons(string searchBy, string? searchPhrase)
        {
            List<PersonResponse> allPersons = GetAllPersons();
            List<PersonResponse> matchingPersons = allPersons;
            if(string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchPhrase))
                return matchingPersons;
            
            switch(searchBy.Trim().ToLower())
            {
                case "name":
                    matchingPersons = allPersons
                    .Where(p => p.Name != null && p.Name.Contains(searchPhrase, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                    break;
                case "address":
                    matchingPersons = allPersons
                    .Where(p => p.Address != null && p.Address.Contains(searchPhrase, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                    break;
                case "country":
                    matchingPersons = allPersons
                    .Where(p => p.Country != null && p.Country.Contains(searchPhrase, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                    break;
                case "dateofbirth":
                    if(DateTime.TryParse(searchPhrase , out DateTime searchDate))
                    {
                        matchingPersons = allPersons
                            .Where(p => p.DateOfBirth.HasValue && p.DateOfBirth.Value.Date ==  searchDate.Date)
                            .ToList();
                    }
                    else
                    {
                        matchingPersons = new List<PersonResponse>();
                    }
                    break;
                default:
                    matchingPersons = allPersons;
                    break;

            }
            return matchingPersons;
        }

        public PersonResponse? GetPersonById(Guid? id)
        {
            if (id == null)
                return null;
            var person = _persons.FirstOrDefault(p => p.PersonId == id);
            if (person == null)
                return null;
            return person.ToPersonResponse();

        }

        public List<PersonResponse> GetSortedPersons(List<PersonResponse> persons, string sortBy, SortDirectionOptions sortDirection)
        {
            if (string.IsNullOrEmpty(sortBy))
                return persons;
            var sortedPersons = persons;
            switch (sortBy.Trim().ToLower())
            {
                case "name":
                    sortedPersons = (sortDirection == SortDirectionOptions.ASC)
                        ? persons.OrderBy(p => p.Name, StringComparer.OrdinalIgnoreCase).ToList()
                        : persons.OrderByDescending(p => p.Name, StringComparer.OrdinalIgnoreCase).ToList();
                    break;

                case "age":
                    sortedPersons = (sortDirection == SortDirectionOptions.ASC)
                        ? persons.OrderBy(p => p.Age).ToList()
                        : persons.OrderByDescending(p => p.Age).ToList();
                    break;

                case "country":
                    sortedPersons = (sortDirection == SortDirectionOptions.ASC)
                        ? persons.OrderBy(p => p.Country).ToList()
                        : persons.OrderByDescending(p => p.Country).ToList();
                    break;

                case "dateofbirth":
                    sortedPersons = (sortDirection == SortDirectionOptions.ASC)
                        ? persons.OrderBy(p => p.DateOfBirth).ToList()
                        : persons.OrderByDescending(p => p.DateOfBirth).ToList();
                    break;

                default:
                    break;
            }
            return sortedPersons;
        }

        public PersonResponse? UpdatePerson(PersonUpdateRequest? request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            ValidationHelper.ModelValidation(request);
            var personFromDb = _persons.FirstOrDefault(p => p.PersonId == request.Id);
            if(personFromDb == null)
                throw new ArgumentException("person Id not existed");
            personFromDb.Name = request.Name;
            personFromDb.Email = request.Email;
            personFromDb.CountryId = request.CountryId;

            return personFromDb.ToPersonResponse();
        }
    }
}
