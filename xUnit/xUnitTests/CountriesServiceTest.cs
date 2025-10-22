using Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
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
            _countriesService = new CountriesService(new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().Options));
        }
        #region AddCountry
        [Fact]
        public async Task AddCountry_CountryIsNull_ShouldThrowException()
        {
            //arrange
            CountryAddRequest? country = null;

            //act
            Func<Task> act =  async () =>await  _countriesService.AddCountry(country);
            //assert
            await act.Should().ThrowAsync<ArgumentNullException>();
        }
        [Fact]
        public async Task AddCountry_CountryNameIsNull_ShouldThrowException()
        {
            //arrange
            CountryAddRequest? country = new CountryAddRequest() { CountryName = null};

            //act
            Func<Task> act =async () =>await _countriesService.AddCountry(country);
            //assert
            await act.Should().ThrowAsync<ArgumentException>();
        }
        [Fact]
        public async Task AddCountry_CountryNameIsDuplicate_ShouldThrowException()
        {
            //arrange
            CountryAddRequest existingCountryRequest = new CountryAddRequest() { CountryName = "Egypt" };
            await _countriesService.AddCountry(existingCountryRequest); 

            CountryAddRequest duplicateRequest = new CountryAddRequest() { CountryName = "Egypt" };

            // Act
            Func<Task> act =async () =>await _countriesService.AddCountry(duplicateRequest);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
               .WithMessage("A country with this name already exists.");
        }
        [Fact]
        public async Task AddCountry_AddCountry()
        {
            //arrange
            CountryAddRequest country = new CountryAddRequest() { CountryName = "country" };

            //act
            var response =await _countriesService.AddCountry(country);
            //assert
            response.Should().NotBeNull();
            response.Name.Should().Be("country");
            response.Id.Should().Be(Guid.Empty);
        }
        #endregion
        #region GetAllCountries 
        [Fact]
        public async Task GetAllCountries_EmptyList()
        {
            // arrange 

            // act 
            var result =await _countriesService.GetAllCountries();
            // assert
            result.Should().BeEmpty();  
        }
        [Fact]
        public async Task GetAllCountries_ListOfAllCountries()
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
                var countryToAdd = await _countriesService.AddCountry(country);
                countryResponses.Add(countryToAdd);
            }
            var result =await _countriesService.GetAllCountries();

            // assert
            foreach (var country in countryResponses)
            {
                result.Should().Contain(country);
            }
        }
        #endregion
        #region GetCountryById
        [Fact]
        public async Task GetCountryById_ReturnsNull()
        {
            // arrange 
            Guid? Id = null;
            // act 
            var result =await _countriesService.GetCountryById(Id);
            // assert
            result.Should().BeNull();
        }
        [Fact]
        public async Task GetCountryById_ReturnsCountry()
        {
            // arrange
            Guid Id = Guid.NewGuid();
            CountryAddRequest country = new CountryAddRequest()
            {
                CountryName = "test"
            };
            var countryResponse =await _countriesService.AddCountry(country);
            // act 
            var result =await _countriesService.GetCountryById(countryResponse.Id);

            // assert
            result.Should().Be(countryResponse);
        }
        #endregion
    }
}
