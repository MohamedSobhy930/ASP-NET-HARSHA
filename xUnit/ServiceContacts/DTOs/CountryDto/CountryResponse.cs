using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContacts.DTOs.CountryDto
{
    public class CountryResponse
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != typeof(CountryResponse)) return false;
            CountryResponse other = obj as CountryResponse;
            return Id == other.Id && Name == other.Name;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
    public static class CountryExtensions
    {
        public static CountryResponse ToCountryResponse(this Country country)
        {
            return new CountryResponse { Id = country.Id, Name = country.Name };
        }
    }
    
}
