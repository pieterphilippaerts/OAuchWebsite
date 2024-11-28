using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OAuch.Controllers {
    public class HomeController : BaseController {
        public HomeController() { }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<IActionResult> Index() {
            if (this.OAuchInternalId != null)
                return RedirectToAction("Index", "Dashboard");
            return View();
        }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        public IActionResult Faq() {
            return View();
        }
        public IActionResult About() {
            return View();
        }
        public IActionResult Privacy() {
            return View();
        }
        public IActionResult Terms() {
            return View();
        }

        public IActionResult Docker() {
            return View();
        }
    }
}