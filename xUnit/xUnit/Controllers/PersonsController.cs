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
        public async Task<IActionResult> Index
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
            var filteredPersons =await _personService.GetFilteredPersons(searchBy,searchPhrase);

            var sortedPersons = _personService.GetSortedPersons(filteredPersons, sortBy, sortDirection);

            ViewBag.CurrentSearchBy = searchBy;
            ViewBag.CurrentSearchPhrase = searchPhrase;

            ViewBag.CurrentSortBy = sortBy;
            ViewBag.CurrentSortDirection = sortDirection;
            return View(sortedPersons);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var countries =await _countriesService.GetAllCountries();
            ViewBag.Countries = new SelectList(countries, nameof(CountryResponse.Id), nameof(CountryResponse.Name));
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(PersonAddRequest personAddRequest)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            PersonResponse person =await _personService.AddPerson(personAddRequest);
            return RedirectToAction("Index", person);
        }
        [HttpGet]
        public async Task<IActionResult> Update(Guid? id)
        {
            var person =await _personService.GetPersonById(id);
            if (person == null)
                return RedirectToAction("Index");
            PersonUpdateRequest personUpdateRequest = person.ToPersonUpdateRequest();
            var countries =await _countriesService.GetAllCountries();
            ViewBag.Countries = new SelectList(countries, nameof(CountryResponse.Id), nameof(CountryResponse.Name));
            return View(personUpdateRequest);
        }
        [HttpPost]
        public async Task<IActionResult> Update(PersonUpdateRequest request)
        {
            if(!ModelState.IsValid)
            {
                var countries =await _countriesService.GetAllCountries();
                ViewBag.Countries = new SelectList(countries, nameof(CountryResponse.Id), nameof(CountryResponse.Name));
                return View(request);
            }
            await _personService.UpdatePerson(request);
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Delete(Guid? id)
        {
            var person =await _personService.GetPersonById(id);
            if(person == null)
                return RedirectToAction("Index");
            return View(person);
        }
        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmation(Guid? Id)
        {
            await _personService.DeletePerson(Id);
            return RedirectToAction("Index");
        }
    }
}
