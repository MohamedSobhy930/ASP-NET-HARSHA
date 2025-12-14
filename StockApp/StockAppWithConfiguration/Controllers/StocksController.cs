using IServicesContracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StockAppAssignment.Models;
using StockAppWithConfigurationAssignment.Models;
using System.Threading.Tasks;

namespace StockAppAssignment.Controllers
{
    [Route("[controller]")]
    public class StocksController(IOptions<TradingOptions> options, IFinnhubService finnhubService) : Controller
    {
        private readonly TradingOptions _options = options.Value;
        private readonly IFinnhubService _finnhubService = finnhubService;
        [Route("/")]
        [Route("[action]/{stockSymbol}")]
        [HttpGet]
        public async Task<IActionResult> Explore(string stockSymbol)
        {
            List<Dictionary<string,string>>? stocksDictionary =await _finnhubService.GetStocks();

            List<Stock> stocks = new List<Stock>();
            if (string.IsNullOrEmpty(stockSymbol))
            {
                stockSymbol = "AAPL";
            }
            if(stocksDictionary != null)
            {
                var stocksToExplore = _options.Top25PopularStocks.Split(",").ToList();
                stocksDictionary = stocksDictionary
                    .Where(temp => stocksToExplore.Contains(Convert.ToString(temp["symbol"])))
                    .ToList();
                stocks = stocksDictionary
                    .Select(temp => new Stock() { StockName = Convert.ToString(temp["description"]), StockSymbol = Convert.ToString(temp["symbol"]) })
                    .ToList();
            }

            ViewBag.Stock = stockSymbol;
            return View(stocks);
        }
    }
}
