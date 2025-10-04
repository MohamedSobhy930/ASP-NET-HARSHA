using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SocialMediaAssignment.Models;
using System.Diagnostics;

namespace SocialMediaAssignment.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SocialMediaLinksOptions _options;

        public HomeController(ILogger<HomeController> logger,
            IOptions<SocialMediaLinksOptions> options)
        {
            _logger = logger;
            _options = options.Value;
        }

        public IActionResult Index()
        {
            ViewBag.Facebook = _options.Facebook;
            ViewBag.Twitter = _options.Twitter;
            ViewBag.YouTube = _options.YouTube;
            ViewBag.Instagram = _options.Instagram;
            return View();
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
