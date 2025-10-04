using System.Diagnostics;
using Configuration.IServices;
using Configuration.Models;
using Configuration.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Configuration.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        //private readonly IConfiguration _configuration;
        //private readonly WeatherApiOptions _options;
        private readonly MarketSymbolsOptions _options;
        private readonly FinnhubService _services;

        public HomeController(ILogger<HomeController> logger, FinnhubService myServices,
            IOptions<MarketSymbolsOptions> options)
            //IOptions<WeatherApiOptions> options)
            //(ILogger<HomeController> logger,IConfiguration configuration)
        {
            _logger = logger;
            _services = myServices;
            _options = options.Value;
            //_configuration = configuration;
            //_options = options.Value;
        }

        public async Task<IActionResult> Index()
        {
            if(_options.DefaultStockSymbol ==  null)
            {
                _options.DefaultStockSymbol = "MSFT";
            }
            Dictionary<string, object>? stockPrices = await _services.GetStockPrice(_options.DefaultStockSymbol);
            Stock stock = new Stock()
            { 
                StockSymbol = _options.DefaultStockSymbol,
                CurrentPrice = Convert.ToDouble(stockPrices["c"].ToString()),
                LowestPrice = Convert.ToDouble(stockPrices["l"].ToString()),
                HighestPrice = Convert.ToDouble(stockPrices["h"].ToString()),
                OpenPrice = Convert.ToDouble(stockPrices["o"].ToString()),

            };

            #region configuration
            // Get from appsetting.json direct
            //var weatherApi = _configuration.GetSection("weatherApi");
            //ViewBag.clientId = weatherApi["ClientID"];
            //ViewBag.secretKey = weatherApi["ClientSecretKey"];

            //using options pattern 
            //WeatherApiOptions? weatherApi = _configuration.GetSection("weatherApi").Get<WeatherApiOptions>();
            //ViewBag.clientId = weatherApi.ClientID;
            //ViewBag.secretKey = weatherApi.ClientSecretKey;

            // using IoC
            //ViewBag.clientId = _options.ClientID;
            //ViewBag.secretKey = _options.ClientSecretKey;
            #endregion
            return View(stock);
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
