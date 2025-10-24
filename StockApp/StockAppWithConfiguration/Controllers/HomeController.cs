using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StockAppWithConfiguration.Models;
using IServicesContracts;
using StockAppWithConfigurationAssignment.Models;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using StockAppAssignment.Models;
using Rotativa.AspNetCore.Options;
using Rotativa.AspNetCore;

namespace StockAppWithConfiguration.Controllers
{
    public class HomeController(ILogger<HomeController> logger,
        IOptions<TradingOptions> options,
        IConfiguration configuration,
        IFinnhubService finnhubService,
        IStockService stockService) : Controller
    {
        private readonly ILogger<HomeController> _logger = logger;
        private readonly TradingOptions _options = options.Value;
        private readonly IConfiguration _configuration = configuration;
        private IFinnhubService _finnhubService = finnhubService;
        private readonly IStockService _stockService = stockService;

        public async Task<IActionResult> Index(string? stockSymbol)
        {
            if (stockSymbol == null)
                stockSymbol = _options.DefaultStockSymbol;
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
        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> BuyOrder(BuyOrderRequest order)
        {
            order.DateAndTimeOfOrder = DateTime.Now;

            //revalidate after adding date
            ModelState.Clear();
            TryValidateModel(order);

            if(!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }
            BuyOrderResponse response = await _stockService.CreateBuyOrder(order);
            return RedirectToAction("Orders");
        }
        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> SellOrder(SellOrderRequest order)
        {
            order.DateAndTimeOfOrder = DateTime.Now;

            //revalidate after add date 
            ModelState.Clear();
            TryValidateModel(order);

            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }
            SellOrderResponse response =await _stockService.CreateSellOrder(order);
            return RedirectToAction("Orders");
        }
        [HttpGet]
        public async Task<IActionResult> Orders()
        {
            List<BuyOrderResponse> buyOrders = await _stockService.GetBuyOrders();
            List<SellOrderResponse> sellOrders = await _stockService.GetSellOrders();
            Orders orders = new Orders()
            {
                BuyOrders = buyOrders,
                SellOrders = sellOrders
            };

            ViewBag.TradingOptions = _options;
            return View(orders);

        }
        public async Task<IActionResult> OrdersPDF()
        {
            List<BuyOrderResponse> buyOrders = await _stockService.GetBuyOrders();
            List<SellOrderResponse> sellOrders = await _stockService.GetSellOrders();
            Orders orders = new Orders()
            {
                BuyOrders = buyOrders,
                SellOrders = sellOrders
            };
            return new ViewAsPdf("OrdersPDF",orders)
            {
                // Sets the name of the file the user will download
                FileName = $"Stock_Orders_{DateTime.Now.ToString("yyyyMMdd_HHmm")}.pdf",

                // Sets the page size (e.g., A4, Letter)
                PageSize = Size.A4,

                // Sets the orientation (Portrait or Landscape)
                PageOrientation =Orientation.Landscape,

                // Sets all margins in millimeters (top, right, bottom, left)
                PageMargins = new Margins(10, 10, 10, 10),

                // --- Advanced Properties ---

                // This is used to pass any other command-line arguments to wkhtmltopdf
                // For example, to add a footer with page numbers:
                CustomSwitches = "--footer-right \"Page [page] of [topage]\" --footer-font-size 10"
            };

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
