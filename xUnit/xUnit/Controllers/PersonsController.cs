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

            var sortedPersons = _personService.GetSortedPersons(filteredPersons, sortBy, sortDirection);

            ViewBag.CurrentSearchBy = searchBy;
            ViewBag.CurrentSearchPhrase = searchPhrase;

            ViewBag.CurrentSortBy = sortBy;
            ViewBag.CurrentSortDirection = sortDirection;
            return View(sortedPersons);
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
            if (!ModelState.IsValid)
            {
                return View();
            }
            PersonResponse person = _personService.AddPerson(personAddRequest);
            return RedirectToAction("Index", person);
        }
        [HttpGet]
        public IActionResult Update(Guid? id)
        {
            var person = _personService.GetPersonById(id);
            if (person == null)
                return RedirectToAction("Index");
            PersonUpdateRequest personUpdateRequest = person.ToPersonUpdateRequest();
            var countries = _countriesService.GetAllCountries();
            ViewBag.Countries = new SelectList(countries, nameof(CountryResponse.Id), nameof(CountryResponse.Name));
            return View(personUpdateRequest);
        }
        [HttpPost]
        public IActionResult Update(PersonUpdateRequest request)
        {
            if(!ModelState.IsValid)
            {
                var countries = _countriesService.GetAllCountries();
                ViewBag.Countries = new SelectList(countries, nameof(CountryResponse.Id), nameof(CountryResponse.Name));
                return View(request);
            }
            _personService.UpdatePerson(request);
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Delete(Guid? id)
        {
            var person = _personService.GetPersonById(id);
            if(person == null)
                return RedirectToAction("Index");
            return View(person);
        }
        [HttpPost]
        [ActionName("Delete")]
        public IActionResult DeleteConfirmation(Guid? Id)
        {
            _personService.DeletePerson(Id);
            return RedirectToAction("Index");
        }
    }
}
