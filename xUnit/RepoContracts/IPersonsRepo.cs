using Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;


namespace RepoContracts
{
    public interface IPersonsRepo
    {
        public Task<Person> AddPerson(Person request);
        public Task<List<Person>> GetAllPersons();
        public Task<Person?> GetPersonById(Guid id);
        public Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate);
        public Task<Person?> UpdatePerson(Person person);
        public Task<bool> DeletePerson(Guid id);

    }
}
