using IServicesContracts;
using Microsoft.AspNetCore.Mvc;
using StockAppWithConfigurationAssignment.Models;

namespace StockAppAssignment.ViewComponents
{
    public class SelectedStockViewComponent(IFinnhubService finnhubService) : ViewComponent
    {
        private readonly IFinnhubService _finnhubService = finnhubService;

        public async Task<IViewComponentResult> InvokeAsync(string stockSymbol)
        {
            // 1. Fetch data for the selected stock
            var companyProfile = await _finnhubService.GetCompanyProfile(stockSymbol);
            var stockQuote = await _finnhubService.GetStockPriceQuote(stockSymbol);

            // 2. Handle not found
            if (companyProfile == null || stockQuote == null)
            {
                // Return a simple view with an error
                ViewData["ErrorMessage"] = $"Details for '{stockSymbol}' not found.";
                return View();
            }
            // 3. Map data to the model
            var stockTrade = new StockTrade
            {
                StockSymbol = stockSymbol,
                StockName = companyProfile["name"]?.ToString(),
                Price = Convert.ToDouble(stockQuote["c"]?.ToString()),

                // Map the new properties
                Logo = companyProfile["logo"]?.ToString(),
                Industry = companyProfile["finnhubIndustry"]?.ToString(),
                Exchange = companyProfile["exchange"]?.ToString()
            };
            // 4. Pass the model to the component's view
            return View(stockTrade);
        }
    }
}
