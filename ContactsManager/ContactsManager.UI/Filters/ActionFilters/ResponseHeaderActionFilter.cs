using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDs.Filters.ActionFilters
{
    public class ResponseHeaderFilterFactory(string key, string value, int order) : Attribute , IFilterFactory
    {
        private readonly string _key = key;
        private readonly string _value = value;
        private readonly int _order = order;

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            var filter = serviceProvider.GetRequiredService<ResponseHeaderActionFilter>();
            filter.Key = _key;
            filter.Value = _value;
            filter.Order = _order;
            return filter;
        }
        public bool IsReusable => false;
    }
    public class ResponseHeaderActionFilter(ILogger<ResponseHeaderActionFilter> logger)
                : IAsyncActionFilter , IOrderedFilter
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public int Order { get; set; }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // before
            logger.LogInformation("{FilterName}.{MethodName} - Before Action Execution",
                nameof(ResponseHeaderActionFilter),
                nameof(OnActionExecutionAsync));
            await next();

            // after
            logger.LogInformation("{FilterName}.{MethodName} - after Action Execution",
                nameof(ResponseHeaderActionFilter),
                nameof(OnActionExecutionAsync));
            context.HttpContext.Response.Headers.Add(Key, Value);
        }
    }
}
