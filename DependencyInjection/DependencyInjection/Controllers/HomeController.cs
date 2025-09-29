using System.Diagnostics;
using DependencyInjection.Models;
using IServices;
using Microsoft.AspNetCore.Mvc;

namespace DependencyInjection.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICitiesService _citiesService;

        public HomeController(ILogger<HomeController> logger, 
            ICitiesService citiesService)
        {
            _logger = logger;
            _citiesService = citiesService;
        }

        public IActionResult Index()
        {
            var cities = _citiesService.GetCityWeathers();
            return View(cities);
        }
        [HttpGet("/weather/{cityCode?}")]
        public IActionResult Details(string? cityCode)
        {
            if (cityCode == null) 
                return View();
            var city = _citiesService.GetCityWeather(cityCode);
            return View(city);
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
