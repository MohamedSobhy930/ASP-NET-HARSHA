using Entities;
using ServiceContacts;
using ServiceContacts.DTOs.CountryDto;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        private readonly AppDbContext _db ;
        public CountriesService(AppDbContext countries)
        {
            _db = countries;
            
        }
        public CountryResponse AddCountry(CountryAddRequest? countryAddRequest)
        {
            ArgumentNullException.ThrowIfNull(countryAddRequest);
            Country country = countryAddRequest.ToCountry();

            if (string.IsNullOrEmpty(country.Name))
            {
                throw new ArgumentException("CountryName cannot be null or empty.");
            }

            if (_db.Countries.Count(temp => temp.Name == countryAddRequest.CountryName) > 0)
            {
                throw new ArgumentException("A country with this name already exists.");
            }
            country.Id = new Guid();
            _db.Countries.Add(country);
            _db.SaveChanges();

            return country.ToCountryResponse();
        }
        public List<CountryResponse> GetAllCountries()
        {
            return _db.Countries.Select(c => c.ToCountryResponse()).ToList(); 
        }
        public CountryResponse? GetCountryById(Guid? id)
        {
            if (id == null)
                return null;
            var country = _db.Countries.FirstOrDefault(c => c.Id  == id);
            if (country == null)
                return null;
            return country.ToCountryResponse();
        }
    }
}
