using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplication3.Data.Services;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserService _userservice;
        private readonly IEmailSender _emailservice;

        public HomeController(ILogger<HomeController> logger, IUserService userService, IEmailSender emailservice)
        {
            _logger = logger;
            _userservice = userService;
            _emailservice = emailservice;
        }

        public async Task<ViewResult> Index()
        {
            UserEmailOptions emailOptions = new UserEmailOptions()
            {
                ToEmails = new List<string>() { "cliffordndungu@hotmail.com" },
                Placeholders = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("{{UserName}}", "Clifford")
                }
            };
            


            await _emailservice.SendTestEmail(emailOptions);
            
            
            var isLoggedin = _userservice.IsAuthenticated();
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