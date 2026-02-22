using Entities;

namespace ServiceContacts.DTOs.CountryDto
{
    public class CountryAddRequest
    {
        public string? CountryName { get; set; }

        public Country ToCountry()
        {
            return new Country
            {
                Name = CountryName,
            };
        }
    }
}
