using IServicesContracts;
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
    public class HomeController() : Controller
    {
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
