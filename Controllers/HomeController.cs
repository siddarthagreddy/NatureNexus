using Microsoft.AspNetCore.Mvc;
using NatureNexus.Models;

namespace NatureNexus.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Model()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

    }
}
