using ServiceContacts.DTOs.PersonDto;
using ServiceContacts.Enums;

namespace ServiceContacts
{
    public interface IPersonService
    {
        public Task<PersonResponse> AddPerson(PersonAddRequest request);
        public Task<List<PersonResponse>> GetAllPersons();
        public Task<PersonResponse?> GetPersonById(Guid? id);
        public Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchPhrase);
        public List<PersonResponse> GetSortedPersons(List<PersonResponse> persons,string SortBy,SortDirectionOptions sortDirection);
        public Task<PersonResponse?> UpdatePerson(PersonUpdateRequest? request);
        public Task<bool> DeletePerson(Guid? id);
        public Task<MemoryStream> GetPersonsCSV();
        Task<MemoryStream> GetPersonsExcel();
    }
}
