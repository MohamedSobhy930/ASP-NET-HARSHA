using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDs.Filters.ResourceFilters
{
    public class FeatureDisabledResourceFilter(ILogger<FeatureDisabledResourceFilter> logger,
        bool isDisabled = true) : IAsyncResourceFilter
    {
        private readonly ILogger<FeatureDisabledResourceFilter> _logger = logger;
        private readonly bool _isDisabled = isDisabled;
        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            // before
            _logger.LogInformation("{FilterName}.{MethodName} - Before executing resource.", nameof(FeatureDisabledResourceFilter), nameof(OnResourceExecutionAsync));
            if(_isDisabled)
            {
                context.Result = new StatusCodeResult(501);


            }
            else
            {
            await next();
            }

            // after
            _logger.LogInformation("{FilterName}.{MethodName} - After executing resource.", nameof(FeatureDisabledResourceFilter), nameof(OnResourceExecutionAsync));

        }
    }
}
