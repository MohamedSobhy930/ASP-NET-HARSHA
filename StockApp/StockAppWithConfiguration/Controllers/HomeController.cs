using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StockAppWithConfiguration.Models;
using IServicesContracts;
using StockAppWithConfigurationAssignment.Models;
using System.Diagnostics;

namespace StockAppWithConfiguration.Controllers
{
    public class HomeController(ILogger<HomeController> logger,
        IOptions<TradingOptions> options,
        IConfiguration configuration,
        IFinnhubService finnhubService) : Controller
    {
        private readonly ILogger<HomeController> _logger = logger;
        private readonly TradingOptions _options = options.Value;
        private readonly IConfiguration _configuration = configuration;
        private IFinnhubService _finnhubService = finnhubService;

        public async Task<IActionResult> Index()
        {
            string? stockSymbol = _options.DefaultStockSymbol;
            if (stockSymbol == null)
                stockSymbol = "MSFT";
            var companyProfile = await _finnhubService.GetCompanyProfile(stockSymbol); 
            var stockQuote = await _finnhubService.GetStockPriceQuote(stockSymbol);

            var stockTrade = new StockTrade
            {
                StockSymbol = stockSymbol,
                FinnhubToken = _configuration["FinnhubToken"]
            };

            if (companyProfile != null)
            {
                stockTrade.StockName = companyProfile["name"]?.ToString();
            }

            if (stockQuote != null)
            {
                stockTrade.Price = Convert.ToDouble(stockQuote["c"]?.ToString());
            }
            return View(stockTrade);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
