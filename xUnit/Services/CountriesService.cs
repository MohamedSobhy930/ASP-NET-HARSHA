using Entities;
using Microsoft.EntityFrameworkCore;
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
        public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
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
            await _db.Countries.AddAsync(country);
            await _db.SaveChangesAsync();

            return country.ToCountryResponse();
        }
        public async  Task<List<CountryResponse>> GetAllCountries()
        {
            return await _db.Countries.Select(c => c.ToCountryResponse()).ToListAsync();
        }
        public async Task<CountryResponse?> GetCountryById(Guid? id)
        {
            if (id == null)
                return null;
            var country =await _db.Countries.FirstOrDefaultAsync(c => c.Id  == id);
            if (country == null)
                return null;
            return country.ToCountryResponse();
        }
    }
}
