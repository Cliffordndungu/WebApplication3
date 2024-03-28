using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication3.Controllers
{
    public class LearnController : Controller
    {
        [Authorize]
        public IActionResult Home()
        {
            return View();
        }
    }
}
