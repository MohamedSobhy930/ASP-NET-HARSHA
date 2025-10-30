using AutoFixture;
using Entities;
using EntityFrameworkCoreMock;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using RepoContracts;
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
        private readonly ICountriesRepo _countriesRepo;
        private readonly Mock<ICountriesRepo> _countriesRepoMock;
        private readonly IFixture _fixture; 
        public CountriesServiceTest()
        {
            _fixture = new Fixture();
            _countriesRepoMock = new Mock<ICountriesRepo>();
            _countriesRepo = _countriesRepoMock.Object;
            _countriesService = new CountriesService(_countriesRepo);
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
            // --- ARRANGE ---
            var countryRequest = _fixture.Build<CountryAddRequest>()
                 .With(temp => temp.CountryName, "Test name")
                 .Create();

            var existingCountry = countryRequest.ToCountry();

            _countriesRepoMock
                 .Setup(temp => temp.GetCountryByName(It.IsAny<string>()))
                 .ReturnsAsync(existingCountry);

            // --- ACT ---
            var action = async () =>
            {
                await _countriesService.AddCountry(countryRequest);
            };

            // --- ASSERT ---

            await action.Should().ThrowAsync<ArgumentException>()
                 .WithMessage("A country with this name already exists.");
        }
        [Fact]
        public async Task AddCountry_FullCountryProperties_Successful()
        {
            //arrange
            CountryAddRequest countryToAdd = _fixture.Create<CountryAddRequest>();
            Country country = countryToAdd.ToCountry();
            CountryResponse countryResponse = country.ToCountryResponse();

            _countriesRepoMock.Setup(temp => temp.AddCountry(It.IsAny<Country>())).ReturnsAsync(country);
            _countriesRepoMock.Setup(temp => temp.GetCountryByName(It.IsAny<string>())).ReturnsAsync(null as Country);

            //act
            var result =await _countriesService.AddCountry(countryToAdd);
            countryResponse.Id = result.Id;

            //assert
            result.Id.Should().NotBe(Guid.Empty);
            result.Should().BeEquivalentTo(countryResponse);
        }
        #endregion
        #region GetAllCountries 
        [Fact]
        public async Task GetAllCountries_EmptyList()
        {
            // arrange 
            List<Country> countries = new List<Country>();  
            _countriesRepoMock.Setup(temp => temp.GetAllCountries()).ReturnsAsync(countries);
            // act 
            var result =await _countriesService.GetAllCountries();
            // assert
            result.Should().BeEmpty();  
        }
        [Fact]
        public async Task GetAllCountries_ListOfAllCountries()
        {
            // arrange
            List<Country> countryList = new List<Country>()
            {
                _fixture.Create<Country>(),
                _fixture.Create<Country>()
            };
            List<CountryResponse> countryResponses = new List<CountryResponse>()
            {
                countryList[0].ToCountryResponse(),
                countryList[1].ToCountryResponse()
            };
            _countriesRepoMock.Setup(temp => temp.GetAllCountries()).ReturnsAsync(countryList);

            // act 
            var result =await _countriesService.GetAllCountries();

            // assert
            result.Should().BeEquivalentTo(countryResponses);
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
            Country country = _fixture.Create<Country>();
            _countriesRepoMock.Setup(temp => temp.GetCountryById(It.IsAny<Guid>())).ReturnsAsync(country);
            CountryResponse countryResponse = country.ToCountryResponse();
            // act 
            var result =await _countriesService.GetCountryById(country.Id);

            // assert
            result.Should().Be(countryResponse);
        }
        #endregion
    }
}
