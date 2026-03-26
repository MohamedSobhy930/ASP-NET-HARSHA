using IServicesContracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Rotativa.AspNetCore;
using Rotativa.AspNetCore.Options;
using StockAppAssignment.Filters.ActionFilters;
using StockAppAssignment.Models;
using StockAppWithConfiguration.Models;
using StockAppWithConfigurationAssignment.Models;
using System.Diagnostics;

namespace StockAppAssignment.Controllers
{
    [Route("[controller]")]
    public class TradeController(ILogger<TradeController> logger,
        IOptions<TradingOptions> options,
        IConfiguration configuration,
        IFinnhubService finnhubService,
        IStockService stockService) : Controller
    {
        private readonly ILogger<TradeController> _logger = logger;
        private readonly TradingOptions _options = options.Value;
        private readonly IConfiguration _configuration = configuration;
        private IFinnhubService _finnhubService = finnhubService;
        private readonly IStockService _stockService = stockService;
    
        [Route("[action]")]
        public async Task<IActionResult> Index(string? stockSymbol)
        {
            _logger.LogInformation("{ControllerName}.{ActionMethod} called with stockSymbol: {stockSymbol}",nameof(TradeController),
                nameof(Index),
                stockSymbol);
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

        [HttpPost("[action]")]
        [ValidateOrderFilter]
        public async Task<IActionResult> BuyOrder(BuyOrderRequest order)
        {
            BuyOrderResponse response = await _stockService.CreateBuyOrder(order);
            return RedirectToAction("Orders");
        }

        [HttpPost("[action]")]
        [ValidateOrderFilter]
        public async Task<IActionResult> SellOrder(SellOrderRequest order)
        {
            SellOrderResponse response = await _stockService.CreateSellOrder(order);
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
        [Route("[action]")]
        public async Task<IActionResult> OrdersPDF()
        {
            _logger.LogInformation("{ControllerName}.{ActionMethod} create a PDF of Orders", nameof(TradeController),
                nameof(OrdersPDF));
            List<BuyOrderResponse> buyOrders = await _stockService.GetBuyOrders();
            List<SellOrderResponse> sellOrders = await _stockService.GetSellOrders();
            Orders orders = new Orders()
            {
                BuyOrders = buyOrders,
                SellOrders = sellOrders
            };
            return new ViewAsPdf("OrdersPDF", orders)
            {
                // Sets the name of the file the user will download
                FileName = $"Stock_Orders_{DateTime.Now.ToString("yyyyMMdd_HHmm")}.pdf",

                // Sets the page size (e.g., A4, Letter)
                PageSize = Size.A4,

                // Sets the orientation (Portrait or Landscape)
                PageOrientation = Orientation.Landscape,

                // Sets all margins in millimeters (top, right, bottom, left)
                PageMargins = new Margins(10, 10, 10, 10),

                // --- Advanced Properties ---
                // This is used to pass any other command-line arguments to wkhtmltopdf
                CustomSwitches = "--footer-right \"Page [page] of [topage]\" --footer-font-size 10"
            };

        }
    }
}
