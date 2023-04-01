using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OAuch.ViewModels;

namespace OAuch.Controllers {
    public class HomeController : BaseController {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger) {
            _logger = logger;
        }

        public async Task<IActionResult> Index() {
            if (this.OAuchInternalId != null)
                return RedirectToAction("Index", "Dashboard");
            return View();
        }
        public IActionResult Faq() {
            return View();
        }
        public IActionResult About() {
            return View();
        }
        public IActionResult Terms() {
            return View();
        }
        public IActionResult Privacy() {
            return View();
        }
    }
}