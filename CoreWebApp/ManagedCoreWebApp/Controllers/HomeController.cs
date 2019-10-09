using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ManagedCoreWebApp.Models;
using Microsoft.Extensions.Options;

namespace ManagedCoreWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly string _secret;
        public HomeController(IOptions<Secret> secrets)
        {
            _secret = secrets.Value?.Sekretkey1;
        }
        public IActionResult Index()
        {
            return View((object)_secret);
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
