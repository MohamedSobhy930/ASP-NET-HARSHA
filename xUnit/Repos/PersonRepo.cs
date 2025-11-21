using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RepoContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repos
{
    public class PersonRepo(AppDbContext db, ILogger<PersonRepo> logger) : IPersonsRepo
    {
        private readonly AppDbContext _db = db;
        private readonly ILogger<PersonRepo> _logger = logger;
        public async Task<Person> AddPerson(Person person)
        {
            _db.Persons.Add(person);
            await _db.SaveChangesAsync();
            return person;
        }

        public async Task<bool> DeletePerson(Guid id)
        {
            _db.RemoveRange(_db.Persons.Where(p => p.PersonId == id));
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<List<Person>> GetAllPersons()
        {
            return await _db.Persons
                .Include("Country")
                .ToListAsync();
        }

        public async Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate)
        {
            _logger.LogInformation("Get Filtered Persons in the PersonRepo");

            return await _db.Persons
                .Include("Country")
                .Where(predicate)
                .ToListAsync();
        }

        public async Task<Person?> GetPersonById(Guid id)
        {
            return await _db.Persons
                .Include("Country")
                .FirstOrDefaultAsync(p => p.PersonId == id);
        }

        public async Task<Person?> UpdatePerson(Person person)
        {
            Person? personFromDb = await _db.Persons
                .Include("Country")
                .FirstOrDefaultAsync(p => p.PersonId == person.PersonId);
            if (personFromDb == null)
            {
                return person;
            }
            personFromDb.Name = person.Name;
            personFromDb.Email = person.Email;
            personFromDb.DateOfBirth = person.DateOfBirth;
            personFromDb.Address = person.Address;
            personFromDb.Gender = person.Gender;
            personFromDb.CountryId = person.CountryId;
            personFromDb.ReceiveNewsletter = person.ReceiveNewsletter;

            await _db.SaveChangesAsync();
            return personFromDb; 
        }
    }
}
