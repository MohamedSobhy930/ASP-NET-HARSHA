using AutoFixture;
using CRUDs.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ServiceContacts;
using ServiceContacts.DTOs.CountryDto;
using ServiceContacts.DTOs.PersonDto;
using ServiceContacts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xUnitTests
{
    public class PersonsControllerTest
    {
        private readonly IPersonService _personService;
        private readonly ICountriesService _countryService;

        private readonly Mock<IPersonService> _personServiceMock;
        private readonly Mock<ICountriesService> _countryServiceMock;
        private readonly IFixture _fixture;

        public PersonsControllerTest() 
        { 
            _fixture = new Fixture();
            _personServiceMock = new Mock<IPersonService>();
            _personService = _personServiceMock.Object;
            _countryServiceMock = new Mock<ICountriesService>();
            _countryService = _countryServiceMock.Object;
        }
        #region Index
        [Fact]
        public async Task Index_ShouldReturnIndexViewWithPersonsList()
        {
            // arrange 
            List<PersonResponse> personResponses = _fixture.Create<List<PersonResponse>>();
            PersonsController personsController = new PersonsController(_personService, _countryService);

            _personServiceMock
                .Setup(temp => temp.GetFilteredPersons(It.IsAny<string>() , It.IsAny<string>()))
                .ReturnsAsync(personResponses);

            _personServiceMock
                .Setup(temp => temp.GetSortedPersons(It.IsAny<List<PersonResponse>>(), It.IsAny<string>(), It.IsAny<SortDirectionOptions>()))
                .Returns(personResponses);

            // act 
            var result = await personsController.Index(_fixture.Create<string>(), _fixture.Create<string>());

            // assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            viewResult.ViewData.Model.Should().BeAssignableTo<List<PersonResponse>>();
            viewResult.ViewData.Model.Should().Be(personResponses);
        }
        #endregion
        #region Create
        [Fact]
        public async Task Create_ModelErrors_ReturnCreateView()
        {
            // arrange 

            PersonAddRequest personAdd = _fixture.Create<PersonAddRequest>();
            PersonResponse personResponse = _fixture.Create<PersonResponse>();
            List<CountryResponse> countryResponses = _fixture.Create<List<CountryResponse>>();

            _countryServiceMock
                .Setup(temp => temp.GetAllCountries())
                .ReturnsAsync(countryResponses);

            _personServiceMock
                .Setup(temp => temp.AddPerson(It.IsAny<PersonAddRequest>()))
                .ReturnsAsync(personResponse);

            PersonsController personsController = new PersonsController(_personService, _countryService);

            // act 
            personsController.ModelState.AddModelError("PersonName", "Name can't be blank");
            var result = await personsController.Create(personAdd);

            // assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            viewResult.ViewData.Model.Should().BeAssignableTo<PersonAddRequest>();
            viewResult.ViewData.Model.Should().Be(personAdd);
        }
        [Fact]
        public async Task Create_NoModelErrors_ReturnIndexView()
        {
            // arrange 

            PersonAddRequest personAdd = _fixture.Create<PersonAddRequest>();
            PersonResponse personResponse = _fixture.Create<PersonResponse>();
            List<CountryResponse> countryResponses = _fixture.Create<List<CountryResponse>>();
            _countryServiceMock
                .Setup(temp => temp.GetAllCountries())
                .ReturnsAsync(countryResponses);

            _personServiceMock
                .Setup(temp => temp.AddPerson(It.IsAny<PersonAddRequest>()))
                .ReturnsAsync(personResponse);

            PersonsController personsController = new PersonsController(_personService, _countryService);

            // act 
            var result = await personsController.Create(personAdd);

            // assert
            RedirectToActionResult viewResult = Assert.IsType<RedirectToActionResult>(result);
            viewResult.ActionName.Should().Be("Index");
        }
        #endregion
    }
}
