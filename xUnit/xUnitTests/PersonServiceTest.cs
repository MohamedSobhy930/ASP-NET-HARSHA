using AutoFixture;
using Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ServiceContacts;
using ServiceContacts.DTOs.CountryDto;
using ServiceContacts.DTOs.PersonDto;
using ServiceContacts.Enums;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xUnitTests
{
    public class PersonServiceTest
    {
        private readonly IPersonService _personService;
        private readonly ICountriesService _countriesService;
        private readonly AppDbContext dbContext;
        private readonly IFixture fixture;
        public PersonServiceTest() 
        {
            fixture = new Fixture();
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            dbContext = new AppDbContext(options);

            _countriesService = new CountriesService(null);
            _personService = new PersonService(null);
        }
        #region AddPerson
        [Fact]
        public async Task AddPerson_ThrowNullReferenceException()
        {
            //arrange 
            PersonAddRequest? personAddRequest = null;
            //act 
            Func<Task> act = async () =>await _personService.AddPerson(personAddRequest);
            //assert
            await act.Should().ThrowAsync<ArgumentNullException>();
        }
        [Fact]
        public async Task AddPerson_PersonRequiredProperties_throwArgumentException()
        {
            //arrange
            PersonAddRequest personAddRequest = new PersonAddRequest()
            {
                Name = null,
                Email = null,
            };

            //act
            Func<Task> act = async () =>await _personService.AddPerson(personAddRequest);

            //assert
            await act.Should().ThrowAsync<ArgumentException>();
        }
        [Fact]
        public async Task AddPerson_ProperPersonDetails()
        {
            //arrange
            PersonAddRequest personAddRequest = fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "test@test.com")
                .Create();
            //act
            var result =await _personService.AddPerson(personAddRequest);
            var resultList =await _personService.GetAllPersons();
            //assert
            result.Should().NotBeNull();
            resultList.Should().Contain(result);
        }
        #endregion

        #region GetPersonById
        [Fact]
        public async Task GetPersonById_PersonExists()
        {
            // Arrange
            var countryRequest = fixture.Create<CountryAddRequest>();
            var countryResponse =await _countriesService.AddCountry(countryRequest);

            PersonAddRequest personToAdd = fixture.Build<PersonAddRequest>()
                .With(temp => temp.Name , "Ahmed")
                .With(temp => temp.Email , "test@test.com")
                .With(temp => temp.CountryId , countryResponse.Id)
                .Create();
            var personResponse =await _personService.AddPerson(personToAdd); 
            
            // Act
            var result =await _personService.GetPersonById(personResponse.Id);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(personResponse.Id);
            result.CountryId.Should().Be(countryResponse.Id);
            result.Name.Should().Be("Ahmed");
        }
        [Fact]
        public async Task GetPersonByName_NonExistedId()
        {
            // arrange 
            Guid? Id = null;

            // act
            var result =await _personService.GetPersonById(Id);

            // assert
            result.Should().BeNull();

        }
        #endregion

        #region GetAllPersons
        [Fact]
        public async Task GetAll_WhenListIsEmpty_ShouldReturnEmptyList()
        {
            // Arrange
            var emptyPersonList = new List<Person>();

            // Act
            var result =await _personService.GetAllPersons();

            // Assert
            result.Should().BeEmpty();
        }
        [Fact]
        public async Task GetAllPersons_WhenPersonsExist_ShouldReturnListOfPersonResponses()
        {
            // Arrange
            var Country1 = fixture.Create<CountryAddRequest>();
            var Country2 = fixture.Create<CountryAddRequest>();

            var countryResponse1 =await _countriesService.AddCountry(Country1);
            var countryResponse2 =await _countriesService.AddCountry(Country2);

            var person1 = fixture.Build<PersonAddRequest>()
                .With(temp => temp.Name, "Ahmed")
                .With(temp => temp.Email, "test@test.com")
                .With(temp => temp.CountryId, countryResponse1.Id)
                .Create();

            var person2 = fixture.Build<PersonAddRequest>()
                .With(temp => temp.Name, "fatima")
                .With(temp => temp.Email, "test1@test.com")
                .With(temp => temp.CountryId, countryResponse2.Id)
                .Create();
            var PersonResponse1 =await _personService.AddPerson(person1);
            var PersonResponse2 =await _personService.AddPerson(person2);
            // Act
            var result =await _personService.GetAllPersons();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);

            // Verify the mapping is correct for one of the items
            var firstPersonResponse = result[0];
            firstPersonResponse.Id.Should().Be(PersonResponse1.Id);
            firstPersonResponse.Name.Should().Be(PersonResponse1.Name);
            firstPersonResponse.Email.Should().Be(PersonResponse1.Email);
            firstPersonResponse.CountryId.Should().Be(PersonResponse1.CountryId);
        }
        #endregion

        #region GetFilteredPerson
        [Fact]
        public void test()
        {
            // arrange 

            // act 

            // assert
        }
        public void test1()
        {
            // arrange 

            // act 

            // assert
        }
        #endregion

        #region UpdatePerson
        [Fact]
        public async Task UpdatePerson_NullPersonObject()
        {
            // arrange 
            PersonUpdateRequest? request = null;
            // act 
            Func<Task> act = async () =>await _personService.UpdatePerson(request);
            // assert
            await act.Should().ThrowAsync<ArgumentNullException>();
        }
        [Fact]
        public async Task UpdatePerson_InvalidPersonId()
        {
            // arrange 
            PersonUpdateRequest? request = new PersonUpdateRequest() {Id = new Guid() };
            // act 
            Func<Task> act = async () =>await _personService.UpdatePerson(request);
            // assert
            await act.Should().ThrowAsync<ArgumentException>();
        }
        [Fact]
        public async Task UpdatePerson_NullPersonName()
        {
            // arrange 
            CountryAddRequest countryAddRequest = fixture.Create<CountryAddRequest>();
            var countryResponse =await _countriesService.AddCountry(countryAddRequest);

            PersonAddRequest personAddRequest = fixture.Build<PersonAddRequest>()
                .With(temp => temp.Name, "Ahmed")
                .With(temp => temp.Email, "test@test.com")
                .With(temp => temp.CountryId, countryResponse.Id)
                .Create();
            var personResponse =await _personService.AddPerson(personAddRequest);

            PersonUpdateRequest personUpdateRequest = personResponse.ToPersonUpdateRequest();
            personUpdateRequest.Name = null;
            // act 
            Func<Task> act = async () => await _personService.UpdatePerson(personUpdateRequest);
            // assert
            await act.Should().ThrowAsync<ArgumentException>();
        }
        [Fact]
        public async Task UpdatePerson_ProperPersonUpdated()
        {
            // arrange 
            CountryAddRequest countryAddRequest = fixture.Create<CountryAddRequest>();
            CountryAddRequest countryAddRequest1 = fixture.Create<CountryAddRequest>();

            var countryResponse =await _countriesService.AddCountry(countryAddRequest);
            var countryResponse1 =await _countriesService.AddCountry(countryAddRequest1);


            PersonAddRequest personAddRequest = new PersonAddRequest() 
            { 
               Name = "Ali", Email = "test@test.com", CountryId = countryResponse.Id
            };
            var personResponse =await _personService.AddPerson(personAddRequest);

            PersonUpdateRequest personUpdateRequest = personResponse.ToPersonUpdateRequest();
            personUpdateRequest.Name = "Mahmoud";
            personUpdateRequest.Email = "test1@test.com";
            personUpdateRequest.CountryId = countryResponse1.Id;


            // act 
            var result =await _personService.UpdatePerson(personUpdateRequest);
            var resultFromGetById =await _personService.GetPersonById(result.Id);

            // assert
            resultFromGetById.Should().NotBeNull();
            resultFromGetById.Should().BeEquivalentTo(result);
        }
        #endregion

        #region DeletePerson
        [Fact]
        public async Task DeletePerson_InvalidId()
        {
            // arrange
            
            // act 
            bool isDeleted =await _personService.DeletePerson(Guid.NewGuid());
            // assert
            isDeleted.Should().BeFalse();
        }
        [Fact]
        public async Task DeletePerson_validId()
        {
            // arrange
            CountryAddRequest countryAddRequest = fixture.Create<CountryAddRequest>();
            var countryResponse =await _countriesService.AddCountry(countryAddRequest);

            PersonAddRequest personAddRequest = fixture.Build<PersonAddRequest>()
                .With(temp => temp.Name, "Ahmed")
                .With(temp => temp.Email, "test@test.com")
                .With(temp => temp.CountryId, countryResponse.Id)
                .Create();
            var personResponse =await _personService.AddPerson(personAddRequest);
            // act 
            bool isDeleted =await _personService.DeletePerson(personResponse.Id);
            // assert
            isDeleted.Should().BeTrue();
        }
        #endregion
    }
}
