using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;
using Rotativa.AspNetCore.Options;
using ServiceContacts;
using ServiceContacts.DTOs.CountryDto;
using ServiceContacts.DTOs.PersonDto;
using ServiceContacts.Enums;

namespace CRUDs.Controllers
{
    [Route("/[controller]/[action]")]
    public class PersonsController(IPersonService personService, ICountriesService countriesService) : Controller
    {
        private readonly IPersonService _personService = personService;
        private readonly ICountriesService _countriesService = countriesService;

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
        public async Task<IActionResult> PersonsPDF()
        {
            List<PersonResponse> persons = await _personService.GetAllPersons();
            return new ViewAsPdf(persons)
            {
                // Sets the name of the file the user will download
                FileName = "PersonsList.pdf",

                // Sets the page size (e.g., A4, Letter)
                PageSize = Size.A4,

                // Sets the orientation (Portrait or Landscape)
                PageOrientation = Orientation.Landscape,

                // Sets all margins in millimeters (top, right, bottom, left)
                PageMargins = new Margins(10, 10, 10, 10),

                // --- Advanced Properties ---

                // This is used to pass any other command-line arguments to wkhtmltopdf
                // For example, to add a footer with page numbers:
                CustomSwitches = "--footer-right \"Page [page] of [topage]\" --footer-font-size 10"
            };
        }
        public async Task<IActionResult> PersonsCSV()
        {
            MemoryStream memoryStream =await _personService.GetPersonsCSV();

            return File(memoryStream,"application/octet-stream","persons.csv");
        }
    }
}
