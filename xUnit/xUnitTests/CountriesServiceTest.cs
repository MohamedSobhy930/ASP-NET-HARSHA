using FluentAssertions;
using ServiceContacts;
using ServiceContacts.DTOs.CountryDto;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xUnitTests
{
    public class CountriesServiceTest
    {
        private readonly ICountriesService _countriesService;
        public CountriesServiceTest()
        {
            _countriesService = new CountriesService(false);
        }
        #region AddCountry
        [Fact]
        public void AddCountry_CountryIsNull_ShouldThrowException()
        {
            //arrange
            CountryAddRequest? country = null;

            //act
            Action act =  () => _countriesService.AddCountry(country);
            //assert
            act.Should().Throw<ArgumentNullException>();
        }
        [Fact]
        public void AddCountry_CountryNameIsNull_ShouldThrowException()
        {
            //arrange
            CountryAddRequest? country = new CountryAddRequest() { CountryName = null};

            //act
            Action act = () => _countriesService.AddCountry(country);
            //assert
            act.Should().Throw<ArgumentException>();
        }
        [Fact]
        public void AddCountry_CountryNameIsDuplicate_ShouldThrowException()
        {
            //arrange
            CountryAddRequest existingCountryRequest = new CountryAddRequest() { CountryName = "Egypt" };
            _countriesService.AddCountry(existingCountryRequest); 

            CountryAddRequest duplicateRequest = new CountryAddRequest() { CountryName = "Egypt" };

            // Act
            Action act = () => _countriesService.AddCountry(duplicateRequest);

            // Assert
            act.Should().Throw<ArgumentException>()
               .WithMessage("A country with this name already exists.");
        }
        [Fact]
        public void AddCountry_AddCountry()
        {
            //arrange
            CountryAddRequest country = new CountryAddRequest() { CountryName = "country" };

            //act
            var response = _countriesService.AddCountry(country);
            //assert
            response.Should().NotBeNull();
            response.Name.Should().Be("country");
            response.Id.Should().Be(Guid.Empty);
        }
        #endregion
        #region GetAllCountries 
        [Fact]
        public void GetAllCountries_EmptyList()
        {
            // arrange 

            // act 
            var result = _countriesService.GetAllCountries();
            // assert
            result.Should().BeEmpty();  
        }
        [Fact]
        public void GetAllCountries_ListOfAllCountries()
        {
            // arrange
            List<CountryAddRequest> CountryRequestList = new List<CountryAddRequest>()
            {
                new CountryAddRequest() { CountryName = "Egypt"},
                new CountryAddRequest() { CountryName = "USA"},
                new CountryAddRequest() { CountryName = "Spain"},
            };
            // act 
            List<CountryResponse> countryResponses = new List<CountryResponse>();
            foreach (var country in CountryRequestList)
            {
                countryResponses.Add(_countriesService.AddCountry(country));
            }
            var result = _countriesService.GetAllCountries();

            // assert
            foreach (var country in countryResponses)
            {
                result.Should().Contain(country);
            }
        }
        #endregion
        #region GetCountryById
        [Fact]
        public void GetCountryById_ReturnsNull()
        {
            // arrange 
            Guid? Id = null;
            // act 
            var result = _countriesService.GetCountryById(Id);
            // assert
            result.Should().BeNull();
        }
        [Fact]
        public void GetCountryById_ResturnsCountry()
        {
            // arrange
            Guid Id = Guid.NewGuid();
            CountryAddRequest country = new CountryAddRequest()
            {
                CountryName = "test"
            };
            var countryResponse = _countriesService.AddCountry(country);
            // act 
            var result = _countriesService.GetCountryById(countryResponse.Id);

            // assert
            result.Should().Be(countryResponse);
        }
        #endregion
    }
}
