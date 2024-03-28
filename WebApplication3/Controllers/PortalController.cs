using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication3.Controllers
{
    public class PortalController : Controller
    {
        [Authorize]
        public IActionResult Home()
        {
            return View();
        }
        [Authorize]
        public IActionResult Billing()
        {
            return View();
        }
        [Authorize]
        public IActionResult Support()
        {
            return View();
        }
        [Authorize]
        public IActionResult Profile()
        {
            return View();
        }

    }
}
