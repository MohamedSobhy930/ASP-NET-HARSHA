using IServicesContracts;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Rotativa.AspNetCore;
using Rotativa.AspNetCore.Options;
using StockAppAssignment.Models;
using StockAppWithConfiguration.Models;
using StockAppWithConfigurationAssignment.Models;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;

namespace StockAppWithConfiguration.Controllers
{
    [Route("/[controller]/[action]")]
    public class HomeController(ILogger<HomeController> logger) : Controller
    {
        private readonly ILogger<HomeController> _logger = logger;

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            var errorViewModel = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                ErrorMessage = exceptionHandlerPathFeature?.Error.Message
            };
            return View(errorViewModel);
        }
    }
}
