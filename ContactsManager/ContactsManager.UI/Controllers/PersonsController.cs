using CRUDs.Filters.ActionFilters;
using CRUDs.Filters.AuthorizationFilters;
using CRUDs.Filters.ExceptionFilters;
using CRUDs.Filters.ResourceFilters;
using CRUDs.Filters.ResultFilters;
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
    [ResponseHeaderFilterFactory("x-Custom_Controller" , "controller_value" , 3)]
    //[TypeFilter(typeof(HandleExceptionFilter))]
    public class PersonsController(IPersonService personService, ICountriesService countriesService, ILogger<PersonsController> logger) : Controller
    {
        private readonly IPersonService _personService = personService;
        private readonly ICountriesService _countriesService = countriesService;
        private readonly ILogger<PersonsController> _logger = logger;
        [Route("/")]
        [TypeFilter(typeof(PersonsListActoinFilter))]
        [ResponseHeaderFilterFactory("X-Custom_Action2" , "action_value2" , 1)] // action filter applied directly using attribute
        [TypeFilter(typeof(PersonsListResultFilter))]
        public async Task<IActionResult> Index
            (string searchBy ,
            string? searchPhrase,
            string sortBy = "name",
            SortDirectionOptions sortDirection = SortDirectionOptions.ASC)
        {
            _logger.LogInformation("This is index action method inside PersonsController");
            _logger.LogDebug
                ($"searchBy : {searchBy}, searchPhrase : {searchPhrase}, sortBy {sortBy}, sortDirection {sortDirection}");
            
            var filteredPersons =await _personService.GetFilteredPersons(searchBy,searchPhrase);

            var sortedPersons = _personService.GetSortedPersons(filteredPersons, sortBy, sortDirection);

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
        [TypeFilter(typeof(PersonCreateAndEditActionFilter))]
        [TypeFilter(typeof(FeatureDisabledResourceFilter) , Arguments = new object[] { false })] //)]
        public async Task<IActionResult> Create(PersonAddRequest personRequest)
        {
            PersonResponse person =await _personService.AddPerson(personRequest);
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
        [ServiceFilter(typeof(PersonCreateAndEditActionFilter))] // same as TypeFilter but ServiceFilter uses DI container to create the filter instance ( means register the filter in the Program.cs)
        public async Task<IActionResult> Update(PersonUpdateRequest personRequest)
        {
            await _personService.UpdatePerson(personRequest);
            return RedirectToAction("Index");
        }
        [HttpGet]
        [TypeFilter(typeof(TokenResultFilter))]
        public async Task<IActionResult> Delete(Guid? id)
        {
            var person =await _personService.GetPersonById(id);
            if(person == null)
                return RedirectToAction("Index");
            return View(person);
        }
        [HttpPost]
        [ActionName("Delete")]
        [TypeFilter(typeof(TokenAuthorizationFilter))]
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

            return File(memoryStream,"text/csv","persons.csv");
        }
        public async Task<IActionResult> PersonsExcel()
        {
            MemoryStream memoryStream = await _personService.GetPersonsExcel();

            string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(memoryStream, mimeType, "Persons.xlsx");
        }
    }
}
