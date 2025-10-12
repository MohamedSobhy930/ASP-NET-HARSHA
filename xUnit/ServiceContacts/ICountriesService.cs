using ServiceContacts.DTOs.CountryDto;

namespace ServiceContacts
{
    public interface ICountriesService
    {
        CountryResponse AddCountry(CountryAddRequest? request);
        List<CountryResponse> GetAllCountries(); 
        CountryResponse? GetCountryById(Guid? id);
    }
}
