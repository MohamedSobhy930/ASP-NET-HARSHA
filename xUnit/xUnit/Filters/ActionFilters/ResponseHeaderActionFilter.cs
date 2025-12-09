using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDs.Filters.ActionFilters
{
    public class ResponseHeaderActionFilter
        (ILogger<ResponseHeaderActionFilter> logger,
        string key,
        string value) : IActionFilter
    {
        private readonly ILogger<ResponseHeaderActionFilter> _logger = logger;
        public void OnActionExecuted(ActionExecutedContext context)
        {
            _logger.LogInformation
                ("{FilterName}.{MethodName} executed after action."
                , nameof(ResponseHeaderActionFilter)
                , nameof(OnActionExecuted));
            context.HttpContext.Response.Headers.Add(key, value);
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            _logger.LogInformation
                ("{FilterName}.{MethodName} executed before action."
                , nameof(ResponseHeaderActionFilter)
                , nameof(OnActionExecuting));
            
        }
    }
}
