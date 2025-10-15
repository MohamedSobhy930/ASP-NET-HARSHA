using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceContacts;
using ServiceContacts.DTOs.CountryDto;
using ServiceContacts.DTOs.PersonDto;
using ServiceContacts.Enums;

namespace CRUDs.Controllers
{
    public class PersonsController(IPersonService personService, ICountriesService countriesService) : Controller
    {
        private readonly IPersonService _personService = personService;
        private readonly ICountriesService _countriesService = countriesService;

        [Route("/persons/index")]
        public IActionResult Index
            (string searchBy ,
            string? searchPhrase,
            string sortBy = "name",
            SortDirectionOptions sortDirection = SortDirectionOptions.ASC)
        {
            ViewBag.SearchFields = new Dictionary<string, string>()
            {
                { "Name", "Person Name" },
                { "DateOfBirth", "Date of Birth" },
                { "Country", "Country" },
                { "Address", "Address" }
            };
            var filteredPersons = _personService.GetFilteredPersons(searchBy,searchPhrase);

            var persons = _personService.GetSortedPersons(filteredPersons, sortBy, sortDirection);

            ViewBag.CurrentSearchBy = searchBy;
            ViewBag.CurrentSearchPhrase = searchPhrase;

            ViewBag.CurrentSortBy = sortBy;
            ViewBag.CurrentSortDirection = sortDirection;
            return View(persons);
        }
        [HttpGet]
        public IActionResult Create()
        {
            var countries = _countriesService.GetAllCountries();
            ViewBag.Countries = new SelectList(countries, nameof(CountryResponse.Id), nameof(CountryResponse.Name));
            return View();
        }
        [HttpPost]
        public IActionResult Create(PersonAddRequest personAddRequest)
        {
            _personService.AddPerson(personAddRequest);
            return RedirectToAction("Index");
        }
    }
}
