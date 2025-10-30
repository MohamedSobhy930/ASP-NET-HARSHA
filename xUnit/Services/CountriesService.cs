using Entities;
using Microsoft.EntityFrameworkCore;
using RepoContracts;
using ServiceContacts;
using ServiceContacts.DTOs.CountryDto;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        private readonly ICountriesRepo _countriesRepo;
        public CountriesService(ICountriesRepo countriesRepo)
        {
            _countriesRepo = countriesRepo;
            
        }
        public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
        {
            ArgumentNullException.ThrowIfNull(countryAddRequest);
            Country country = countryAddRequest.ToCountry();

            if (string.IsNullOrEmpty(country.Name))
            {
                throw new ArgumentException("CountryName cannot be null or empty.");
            }

            if (await _countriesRepo.GetCountryByName(country.Name)!= null)
            {
                throw new ArgumentException("A country with this name already exists.");
            }
            country.Id = Guid.NewGuid();
            await _countriesRepo.AddCountry(country);

            return country.ToCountryResponse();
        }
        public async  Task<List<CountryResponse>> GetAllCountries()
        {
            return (await _countriesRepo.GetAllCountries())
                .Select(temp => temp.ToCountryResponse())
                .ToList();
        }
        public async Task<CountryResponse?> GetCountryById(Guid? id)
        {
            if (id == null)
                return null;
            Country? country =await _countriesRepo.GetCountryById(id.Value);
            if (country == null)
                return null;
            return country.ToCountryResponse();
        }
    }
}
