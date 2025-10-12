using ServiceContacts.DTOs.PersonDto;
using ServiceContacts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContacts
{
    public interface IPersonService
    {
        public PersonResponse AddPerson(PersonAddRequest request);
        public List<PersonResponse> GetAllPersons();
        public PersonResponse? GetPersonById(Guid? id);
        public List<PersonResponse> GetFilteredPersons(string searchBy, string? searchPhrase);
        public List<PersonResponse> GetSortedPersons(List<PersonResponse> persons,string SortBy,SortDirectionOptions sortDirection);
        public PersonResponse? UpdatePerson(PersonUpdateRequest? request);
        public bool DeletePerson(Guid? id);
        
    }
}
