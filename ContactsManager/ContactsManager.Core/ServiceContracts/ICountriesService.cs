using ServiceContacts.DTOs.CountryDto;

namespace ServiceContacts
{
    public interface ICountriesService
    {
        Task<CountryResponse> AddCountry(CountryAddRequest? request);
        Task<List<CountryResponse>> GetAllCountries(); 
        Task<CountryResponse?> GetCountryById(Guid? id);
    }
}
