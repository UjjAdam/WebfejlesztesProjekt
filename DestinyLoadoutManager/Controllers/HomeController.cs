using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using DestinyLoadoutManager.Models;

namespace DestinyLoadoutManager.Controllers
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

        public IActionResult Debug()
        {
            return View();
        }

        [HttpPost]
        public IActionResult DebugPost(string testInput)
        {
            _logger.LogInformation($"DebugPost received: {testInput}");
            return Json(new { success = true, message = $"Received: {testInput}", timestamp = DateTime.Now });
        }

        [HttpGet]
        public IActionResult DebugAjax()
        {
            _logger.LogInformation("DebugAjax called");
            return Content("AJAX Response OK - " + DateTime.Now.ToString());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
