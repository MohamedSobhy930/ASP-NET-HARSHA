using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDs.Filters.ResultFilters
{
    public class PersonsListResultFilter(ILogger<PersonsListResultFilter> logger) : IAsyncResultFilter 
    {
        private readonly ILogger<PersonsListResultFilter> _logger = logger;
        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            // before
            _logger.LogInformation("{FilterName}.{MethodName} - Before executing result.", nameof(PersonsListResultFilter), nameof(OnResultExecutionAsync));

            await next();

            // after
            _logger.LogInformation("{FilterName}.{MethodName} - After executing result.", nameof(PersonsListResultFilter), nameof(OnResultExecutionAsync));
            context.HttpContext.Response.Headers["Last_modified"] = DateTime.UtcNow.ToString("g");
        }
    }
}
