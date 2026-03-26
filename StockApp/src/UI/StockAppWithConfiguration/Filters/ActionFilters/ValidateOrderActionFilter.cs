using IServicesContracts.DTOs;
using Microsoft.AspNetCore.Mvc.Filters;
using StockAppAssignment.Controllers;
using StockAppWithConfigurationAssignment.Models;

namespace StockAppAssignment.Filters.ActionFilters
{
    public class ValidateOrderFilter : Attribute, IFilterFactory
    {
        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            var filter = serviceProvider.GetRequiredService<ValidateOrderActionFilter>();
            return filter;
        }
        public bool IsReusable => false;
    }
    public class ValidateOrderActionFilter(ILogger<ValidateOrderActionFilter> logger,
        IConfiguration configuration) : IAsyncActionFilter
    {
        private readonly ILogger<ValidateOrderActionFilter> _logger = logger;
        private readonly IConfiguration _configuration = configuration;
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // before 
            _logger.LogInformation("{FilterName} - Before executing action {ActionName}",
                nameof(ValidateOrderActionFilter),
                context.ActionDescriptor.DisplayName);
            if (context.Controller is TradeController tradeController)
            {
                var order = context.ActionArguments["order"] as IOrderRequest;
                if(order != null)
                {
                    order.DateAndTimeOfOrder = DateTime.Now;

                    tradeController.ModelState.Remove("DateAndTimeOfOrder");

                    if (!tradeController.ModelState.IsValid)
                    {
                        tradeController.ViewBag.ErrorMessage = tradeController.ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage)
                            .ToList();

                        StockTrade stock = new StockTrade 
                        { 
                            StockName = order.StockName,
                            Quantity = order.Quantity,
                            StockSymbol = order.StockSymbol,
                            Price = order.Price,
                            FinnhubToken = _configuration["FinnhubToken"]
                        };
                        context.Result = tradeController.View("Index", stock);
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
            }
            else
            {
                await next();
            }

            // after
            _logger.LogInformation("{FilterName} - After executing action {ActionName}",
                nameof(ValidateOrderActionFilter),
                context.ActionDescriptor.DisplayName);
        }
    }
}
