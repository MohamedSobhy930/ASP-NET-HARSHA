using Azure.Identity;
using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContacts;
using ServiceContacts.DTOs.CountryDto;
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
        private readonly AppDbContext _db;
        private readonly ICountriesService _countriesService;
        
        public PersonService(ICountriesService countriesService, AppDbContext db) 
        {
            _db = db;
            _countriesService = countriesService;
            
        }

        public async Task<PersonResponse> AddPerson(PersonAddRequest? request)
        {
            if(request == null)
                throw new ArgumentNullException();

            ValidationHelper.ModelValidation(request);

            Person person = request.ToPerson();
            person.PersonId = Guid.NewGuid();
            await _db.AddAsync(person);
            await _db.SaveChangesAsync();
            // using sp
            //_db.sp_InsertPerson(person);
            return person.ToPersonResponse();
        }

        public async Task<bool> DeletePerson(Guid? id)
        {
            if(id == null) 
                throw new ArgumentNullException(nameof(id));
            var person = _db.Persons.FirstOrDefault(p => p.PersonId == id);
            if (person == null)
                return false;
            _db.Persons.Remove(person);
            await _db.SaveChangesAsync();
            return true;   
        }

        public async Task<List<PersonResponse>> GetAllPersons()
        {
            var persons = await _db.Persons.Include(p => p.Country).ToListAsync();
            
            return persons
                .Select(person => person.ToPersonResponse())
                .ToList();
            
            /*
             * STORED PROCEDURE
             * 1 - add migration then create the sp then update database 
             * 2 - use it inside the service instead of traditional retrieving 
             */
            //return _db.Database
            //    .SqlQuery<PersonResponse>($"EXEC sp_GetAllPersons")
            //    .ToList();
        }

        public async Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchPhrase)
        {
            List<PersonResponse> allPersons = await GetAllPersons();
            List<PersonResponse> matchingPersons ;
            if(string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchPhrase))
                return allPersons;
            
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

        public async Task<PersonResponse?> GetPersonById(Guid? id)
        {
            if (id == null)
                return null;
            var person =await _db.Persons.Include(p => p.Country).FirstOrDefaultAsync(p => p.PersonId == id);
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

        public async Task<PersonResponse?> UpdatePerson(PersonUpdateRequest? request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            ValidationHelper.ModelValidation(request);
            var personFromDb =await _db.Persons.FirstOrDefaultAsync(p => p.PersonId == request.Id);
            if(personFromDb == null)
                throw new ArgumentException("person Id not existed");
            personFromDb.Name = request.Name;
            personFromDb.Email = request.Email;
            personFromDb.CountryId = request.CountryId;
            await _db.SaveChangesAsync();

            return personFromDb.ToPersonResponse();
        }
    }
}
