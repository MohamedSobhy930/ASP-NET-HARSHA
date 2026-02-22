using CRUDs.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceContacts;
using ServiceContacts.DTOs.CountryDto;
using ServiceContacts.DTOs.PersonDto;

namespace CRUDs.Filters.ActionFilters
{
    public class PersonCreateAndEditActionFilter(ICountriesService countriesService,
        ILogger<PersonCreateAndEditActionFilter> logger) : IAsyncActionFilter
    {
        private readonly ICountriesService _countriesService = countriesService;
        private readonly ILogger<PersonCreateAndEditActionFilter> _logger = logger;
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // before
            //_logger.LogInformation("{FilterName}.{MethodName} - Before executing action.",nameof(PersonCreateAndEditActionFilter),nameof(OnActionExecutionAsync));
            if (context.Controller is PersonsController personsController)
            {
                if (!personsController.ModelState.IsValid)
                {
                    var countries = await _countriesService.GetAllCountries();
                    personsController.ViewBag.Countries = new SelectList(countries, nameof(CountryResponse.Id), nameof(CountryResponse.Name));
                    
                    personsController.ViewBag.ErrorMessage = personsController.ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    
                    var personRequest = context.ActionArguments["personRequest"];
                    context.Result = personsController.View(personRequest);
                }
                else
                {
                    await next();
                }
            }
            else
            {
            await next();
            }

            // after
            //_logger.LogInformation("{FilterName}.{MethodName} - after executing action.", nameof(PersonCreateAndEditActionFilter), nameof(OnActionExecutionAsync));

        }
    }
}
