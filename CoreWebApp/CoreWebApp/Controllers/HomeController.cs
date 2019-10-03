using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CoreWebApp.Models;

namespace CoreWebApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
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

        // using ResponseCache for Action Method v1
        [ResponseCache(Duration = 3, Location = ResponseCacheLocation.Any, NoStore = false, VaryByHeader = "User-Agent")]
        public IActionResult Cached4Response()
        {
            return View(DateTime.UtcNow);
        }

        // using ResponseCache for Action Method v2
        [ResponseCache(CacheProfileName = "Default30")]
        public IActionResult Cached4Response(string v2)
        {
            return View(DateTime.UtcNow);
        }
    }
}
