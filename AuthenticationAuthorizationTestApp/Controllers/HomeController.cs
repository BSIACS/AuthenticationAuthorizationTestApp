using AuthenticationAuthorizationTestApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TestDataLibrary.DAL.Context;

namespace AuthenticationAuthorizationTestApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly TestDataLibraryContext _dbContext;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, TestDataLibraryContext dbContext)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public IActionResult Index()
        {
            ViewBag.Name = User.Identity.Name;
            ViewBag.IsAuthenticated = HttpContext.User.Identity.IsAuthenticated;

            return View();
        }

        [Authorize(Roles = "administrator")]
        public IActionResult Privacy()
        {
            ViewBag.Name = User.Identity.Name;

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
