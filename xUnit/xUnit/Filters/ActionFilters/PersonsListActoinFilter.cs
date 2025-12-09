using CRUDs.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;

namespace CRUDs.Filters.ActionFilters
{
    public class PersonsListActoinFilter(ILogger<PersonsListActoinFilter> logger) : IActionFilter
    {
        private readonly ILogger<PersonsListActoinFilter> _logger = logger;
        public void OnActionExecuted(ActionExecutedContext context)
        {
            _logger.LogInformation
                ("{FilterName}.{MethodName} executed after action."
                ,nameof(PersonsListActoinFilter)
                ,nameof(OnActionExecuted));
            PersonsController personsController = context.Controller as PersonsController;
            var actionArguments =(IDictionary<string,object?>?) context.HttpContext.Items["arguments"];
            if(actionArguments != null)
            {
                if(actionArguments.ContainsKey("searchBy"))
                {
                    personsController.ViewData["CurrentSearchBy"] = Convert.ToString(actionArguments["searchBy"]);
                }
                if (actionArguments.ContainsKey("searchPhrase"))
                {
                    personsController.ViewData["CurrentSearchPhrase"] = Convert.ToString(actionArguments["searchPhrase"]);
                }
                if (actionArguments.ContainsKey("sortBy"))
                {
                    personsController.ViewData["CurrentsortBy"] = Convert.ToString(actionArguments["sortBy"]);
                }
                if (actionArguments.ContainsKey("sortDirection"))
                {
                    personsController.ViewData["CurrentsortDirection"] = (Enum) actionArguments["sortDirection"]; //Convert.ToString(actionArguments["sortDirection"]);
                }
            }
            personsController.ViewBag.SearchFields = new Dictionary<string, string>()
            {
                { "Name", "Person Name" },
                { "DateOfBirth", "Date of Birth" },
                { "Country", "Country" },
                { "Address", "Address" }
            };
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            context.HttpContext.Items["arguments"] = context.ActionArguments;
            _logger.LogInformation
                ("{FilterName}.{MethodName} executed before action."
                , nameof(PersonsListActoinFilter)
                , nameof(OnActionExecuting)); 
            if(context.ActionArguments.ContainsKey("searchBy"))
            {
                string? searchBy =Convert.ToString( context.ActionArguments["searchBy"]);
                if(!searchBy.IsNullOrEmpty())
                {
                    var allowedSearchByValues = new List<string>() { "Name", "DateOfBirth", "Country", "Address" };
                    if(!allowedSearchByValues.Contains(searchBy))
                    {
                        _logger.LogWarning($"Invalid searchBy value: \"{searchBy}\". Allowed values are: {string.Join("| ", allowedSearchByValues)}");
                        context.ActionArguments["searchBy"] = allowedSearchByValues[0];
                        _logger.LogInformation($"searchBy value set to default: {allowedSearchByValues[0]}");
                    }
                }
            }
        }
    }
}
