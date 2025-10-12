using Entities;
using ServiceContacts;
using ServiceContacts.DTOs.CountryDto;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        private readonly List<Country> _countries ;
        public CountriesService()
        {
            _countries = new List<Country>();
        }
        public CountryResponse AddCountry(CountryAddRequest? countryAddRequest)
        {
            if (countryAddRequest == null)
            {
                throw new ArgumentNullException(nameof(countryAddRequest));
            }
            Country country = countryAddRequest.ToCountry();

            if (string.IsNullOrEmpty(country.Name))
            {
                throw new ArgumentException("CountryName cannot be null or empty.");
            }

            if (_countries.Any(c => c.Name.Equals(country.Name, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ArgumentException("A country with this name already exists.");
            }
            country.Id = new Guid();
            _countries.Add(country);

            return country.ToCountryResponse();
        }
        public List<CountryResponse> GetAllCountries()
        {
            if (_countries.Count == 0)
                return new List<CountryResponse>();
            var response = new List<CountryResponse>();
            foreach (var country in _countries)
            {
                response.Add(country.ToCountryResponse());
            }
            return response;
        }
        public CountryResponse? GetCountryById(Guid? id)
        {
            if (id == null)
                return null;
            var country = _countries.FirstOrDefault(c => c.Id  == id);
            if (country == null)
                return null;
            return country.ToCountryResponse();
        }
    }
}
