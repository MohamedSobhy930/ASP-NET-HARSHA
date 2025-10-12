using Entities;
using FluentAssertions;
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
        private IPersonService _personService;
        private ICountriesService _countryService;
        public PersonServiceTest() 
        {
            _personService = new PersonService();
            _countryService = new CountriesService();
        }
        #region AddPerson
        [Fact]
        public void AddPerson_ThrowNullReferenceException()
        {
            //arrange 
            PersonAddRequest? personAddRequest = null;
            //act 
            Action act = () =>  _personService.AddPerson(personAddRequest);
            //assert
            act.Should().Throw<ArgumentNullException>();
        }
        [Fact]
        public void AddPerson_PersonRequiredProperties_throwArgumentException()
        {
            //arrange
            PersonAddRequest personAddRequest = new PersonAddRequest()
            {
                Name = null,
                Email = null,
            };
            //act
            Action act = () => _personService.AddPerson(personAddRequest);
            //assert
            act.Should().Throw<ArgumentException>();
        }
        [Fact]
        public void AddPerson_ProperPersonDetails()
        {
            //arrange
            PersonAddRequest personAddRequest = new PersonAddRequest()
            {
                Name = "test",
                Email = "test@test.com",
                Gender = GenderOptions.female,
                DateOfBirth = DateTime.Parse("2010-03-02"),
                CountryId = new Guid(),
                Address = "cairo",
                ReceiveNewsletter = true
            };
            //act
            var result = _personService.AddPerson(personAddRequest);
            var resultList = _personService.GetAllPersons();
            //assert
            result.Should().NotBeNull();
            result.Id.Should().Be(Guid.Empty);
            resultList.Should().Contain(result);
        }
        #endregion

        #region GetPersonById
        [Fact]
        public void GetPersonById_PersonExists()
        {
            // Arrange
            var countryRequest = new CountryAddRequest()
            { CountryName = "Egypt"};
            var countryResponse = _countryService.AddCountry(countryRequest);

            var personToAdd = new PersonAddRequest 
            { Name = "Ahmed",
                Email ="test@test.com",
                Gender = GenderOptions.female,
                DateOfBirth = DateTime.Parse("1990-02-03"),
                CountryId = countryResponse.Id,
                Address = "blabla",
                ReceiveNewsletter = true
            };
            var personResponse = _personService.AddPerson(personToAdd); 

            // Act
            var result = _personService.GetPersonById(personResponse.Id);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(personResponse.Id);
            result.CountryId.Should().Be(countryResponse.Id);
            result.Name.Should().Be("Ahmed");
        }
        [Fact]
        public void GetPersonByName_NonExistedId()
        {
            // arrange 
            Guid? Id = null;

            // act
            var result = _personService.GetPersonById(Id);

            // assert
            result.Should().BeNull();

        }
        #endregion

        #region GetAllPersons
        [Fact]
        public void GetAll_WhenListIsEmpty_ShouldReturnEmptyList()
        {
            // Arrange
            var emptyPersonList = new List<Person>();

            // Act
            var result = _personService.GetAllPersons();

            // Assert
            result.Should().BeEmpty();
        }
        [Fact]
        public void GetAllPersons_WhenPersonsExist_ShouldReturnListOfPersonResponses()
        {
            // Arrange
            var Country1 = new CountryAddRequest() { CountryName = "Egypt" };
            var Country2 = new CountryAddRequest() { CountryName = "USA" };

            var countryResponse1 = _countryService.AddCountry(Country1);
            var countryResponse2 = _countryService.AddCountry(Country2);

            var person1 = new PersonAddRequest() {
                Name = "Ahmed",
                Email = "ahmed@example.com",
                Address = "address1",
                CountryId = countryResponse1.Id,
                ReceiveNewsletter = true
            };
            var person2 = new PersonAddRequest()
            {
                Name = "Fatima",
                Email = "fatima@example.com",
                Address = "address2",
                CountryId = countryResponse2.Id,
                ReceiveNewsletter = false,
            };
            var PersonResponse1 = _personService.AddPerson(person1);
            var PersonResponse2 = _personService.AddPerson(person2);
            // Act
            var result = _personService.GetAllPersons();

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
        public void UpdatePerson_NullPersonObject()
        {
            // arrange 
            PersonUpdateRequest? request = null;
            // act 
            Action act = () => _personService.UpdatePerson(request);
            // assert
            act.Should().Throw<ArgumentNullException>();
        }
        [Fact]
        public void UpdatePerson_InvalidPersonId()
        {
            // arrange 
            PersonUpdateRequest? request = new PersonUpdateRequest() {Id = new Guid() };
            // act 
            Action act = () => _personService.UpdatePerson(request);
            // assert
            act.Should().Throw<ArgumentException>();
        }
        [Fact]
        public void UpdatePerson_NullPersonName()
        {
            // arrange 
            CountryAddRequest countryAddRequest = new CountryAddRequest() { CountryName = "Egypt" };
            var countryResponse = _countryService.AddCountry(countryAddRequest);

            PersonAddRequest personAddRequest = new PersonAddRequest() { Name = "Ali", Email = "test@test.com", CountryId = countryResponse.Id };
            var personResponse = _personService.AddPerson(personAddRequest);

            PersonUpdateRequest personUpdateRequest = personResponse.ToPersonUpdateRequest();
            personUpdateRequest.Name = null;
            // act 
            Action act = () => _personService.UpdatePerson(personUpdateRequest);
            // assert
            act.Should().Throw<ArgumentException>();
        }
        [Fact]
        public void UpdatePerson_ProperPersonUpdated()
        {
            // arrange 
            CountryAddRequest countryAddRequest = new CountryAddRequest() { CountryName = "Egypt" };
            CountryAddRequest countryAddRequest1 = new CountryAddRequest() { CountryName = "USA" };

            var countryResponse = _countryService.AddCountry(countryAddRequest);
            var countryResponse1 = _countryService.AddCountry(countryAddRequest1);


            PersonAddRequest personAddRequest = new PersonAddRequest() 
            { 
               Name = "Ali", Email = "test@test.com", CountryId = countryResponse.Id
            };
            var personResponse = _personService.AddPerson(personAddRequest);

            PersonUpdateRequest personUpdateRequest = personResponse.ToPersonUpdateRequest();
            personUpdateRequest.Name = "Mahmoud";
            personUpdateRequest.Email = "test1@test.com";
            personUpdateRequest.CountryId = countryResponse1.Id;


            // act 
            var result = _personService.UpdatePerson(personUpdateRequest);
            var resultFromGetById = _personService.GetPersonById(result.Id);

            // assert
            resultFromGetById.Should().NotBeNull();
            resultFromGetById.Should().BeEquivalentTo(result);
        }
        #endregion

        #region DeletePerson
        [Fact]
        public void DeletePerson_InvalidId()
        {
            // arrange
            
            // act 
            bool isDeleted = _personService.DeletePerson(Guid.NewGuid());
            // assert
            isDeleted.Should().BeFalse();
        }
        [Fact]
        public void DeletePerson_validId()
        {
            // arrange
            CountryAddRequest countryAddRequest = new CountryAddRequest() { CountryName = "Egypt" };
            var countryResponse = _countryService.AddCountry(countryAddRequest);

            PersonAddRequest personAddRequest = new PersonAddRequest() { Name = "Ali", Email = "test@test.com", CountryId = countryResponse.Id };
            var personResponse = _personService.AddPerson(personAddRequest);
            // act 
            bool isDeleted = _personService.DeletePerson(personResponse.Id);
            // assert
            isDeleted.Should().BeTrue();
        }
        #endregion
    }
}
