using AutoFixture;
using Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using RepoContracts;
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

        // mock repo
        private readonly IPersonsRepo _personsRepo;
        private readonly Mock<IPersonsRepo> _personRepoMock;
        private readonly IFixture fixture;
        public PersonServiceTest() 
        {
            fixture = new Fixture();
            
            _personRepoMock = new Mock<IPersonsRepo>();
            _personsRepo = _personRepoMock.Object;
            _personService = new PersonService(_personsRepo);
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
            PersonAddRequest? personAddRequest = fixture.Build<PersonAddRequest>()
                .With(temp => temp.Name, null as string)
                .Create();

            Person person = personAddRequest.ToPerson();
            _personRepoMock.Setup(temp => temp.AddPerson(It.IsAny<Person>())).ReturnsAsync(person);
            //act
            Func<Task> act = async () =>await _personService.AddPerson(personAddRequest);

            //assert
            await act.Should().ThrowAsync<ArgumentException>();
        }
        [Fact]
        public async Task AddPerson_FullPersonDetails_Successful()
        {
            //arrange
            PersonAddRequest personAddRequest = fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "test@test.com")
                .Create();
            Person person = personAddRequest.ToPerson();
            PersonResponse expectedResponse = person.ToPersonResponse();
            _personRepoMock.Setup(temp => temp.AddPerson(It.IsAny<Person>())).ReturnsAsync(person);

            //act
            var responseFromAdd =await _personService.AddPerson(personAddRequest);
            expectedResponse.Id = responseFromAdd.Id;
            //assert
            responseFromAdd.Should().NotBeNull();
            responseFromAdd.Should().Be(expectedResponse);

        }
        #endregion

        #region GetPersonById
        [Fact]
        public async Task GetPersonById_PersonExists()
        {
            // Arrange

            Person person = fixture.Build<Person>()
                .With(temp => temp.Name , "Ahmed")
                .With(temp => temp.Email , "test@test.com")
                .Create();
            var personResponse = person.ToPersonResponse();
            _personRepoMock.Setup(temp => temp.GetPersonById(It.IsAny<Guid>())).ReturnsAsync(person); 

            
            // Act
            var result =await _personService.GetPersonById(personResponse.Id);

            personResponse.Id = result.Id;
            // Assert
            result.Should().Be(personResponse);
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
            _personRepoMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(emptyPersonList);

            // Act
            var result =await _personService.GetAllPersons();

            // Assert
            result.Should().BeEmpty();
        }
        [Fact]
        public async Task GetAllPersons_WhenPersonsExist_ShouldReturnListOfPersonResponses()
        {
            // Arrange
            List<Person> persons = new List<Person>()
            {
                fixture.Build<Person>()
                .With(temp => temp.Name, "Ahmed")
                .With(temp => temp.Email, "test@test.com")
                .With(temp => temp.Country, null as Country)
                .Create(),
                fixture.Build<Person>()
                .With(temp => temp.Name, "fatima")
                .With(temp => temp.Email, "test1@test.com")
                .With(temp => temp.Country, null as Country)
                .Create()
            };
            List<PersonResponse> response = new List<PersonResponse>()
            {
                persons[0].ToPersonResponse(),
                persons[1].ToPersonResponse(),
            };

            _personRepoMock.Setup(temp =>temp.GetAllPersons()).ReturnsAsync(persons);
            // Act
            var result =await _personService.GetAllPersons();

            // Assert
            result.Should().BeEquivalentTo(response);

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
            PersonUpdateRequest request = new PersonUpdateRequest() { Id =  Guid.NewGuid() };
            
            // act 
            Func<Task> act = async () =>await _personService.UpdatePerson(request);
            // assert
            await act.Should().ThrowAsync<ArgumentException>();
        }
        [Fact]
        public async Task UpdatePerson_NullPersonName()
        {
            // arrange 

            Person person = fixture.Build<Person>()
                .With(temp => temp.Name, null as string)
                .With(temp => temp.Email, "test@test.com")
                .With(temp => temp.Country, null as Country)
                .Create();
            PersonResponse personResponse = person.ToPersonResponse();
            PersonUpdateRequest personUpdateRequest = personResponse.ToPersonUpdateRequest();

            // act 
            Func<Task> act = async () => await _personService.UpdatePerson(personUpdateRequest);

            // assert
            await act.Should().ThrowAsync<ArgumentException>();
        }
        [Fact]
        public async Task UpdatePerson_ProperPersonUpdated()
        {
            // arrange 
            Person person = fixture.Build<Person>()
                .With(temp => temp.Name, "Ali")
                .With(temp => temp.Email, "test@test.com")
                .With(temp => temp.Country, null as Country)
                .Create();

            PersonResponse personResponse = person.ToPersonResponse();
            PersonUpdateRequest personUpdateRequest = personResponse.ToPersonUpdateRequest();
   
            _personRepoMock.Setup(temp => temp.UpdatePerson(It.IsAny<Person>())).ReturnsAsync(person);
            _personRepoMock.Setup(temp => temp.GetPersonById(It.IsAny<Guid>())).ReturnsAsync(person);

            // act 
            var result =await _personService.UpdatePerson(personUpdateRequest);

            // assert
            result.Should().Be(personResponse);
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

            Person person = fixture.Build<Person>()
                .With(temp => temp.PersonId , Guid.NewGuid())
                .With(temp => temp.Name, "Ahmed")
                .With(temp => temp.Email, "test@test.com")
                .With(temp => temp.Country, null as Country)
                .Create();
            PersonResponse personResponse = person.ToPersonResponse();
            _personRepoMock.Setup(temp => temp.GetPersonById(It.IsAny<Guid>())).ReturnsAsync(person);
            _personRepoMock.Setup(temp => temp.DeletePerson(It.IsAny<Guid>())).ReturnsAsync(true);

            // act 
            bool isDeleted =await _personService.DeletePerson(personResponse.Id);
            // assert
            isDeleted.Should().BeTrue();
        }
        #endregion
    }
}
