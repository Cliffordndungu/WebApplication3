using Azure.Core;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication3.Controllers
{
    public class AcronisController : Controller
    {
        private readonly AcronisTokenService _acronisTokenService;

        public AcronisController(AcronisTokenService acronisTokenService)
        {
            _acronisTokenService = acronisTokenService;
        }
        public async Task<IActionResult> Index()
        {
            var accessToken = await _acronisTokenService.GetAccessToken();
            if (accessToken != null)
            {
                ViewData["Message"] = "Access token reciveved";
                return View();
                // Use the access token as needed
                // Perform your Acronis API requests with the token
            }
            else
            {
                ViewData["Message"] = "issues ";
                return View();
                // Handle the case where token retrieval failed
            }
           
        }

        //public async Task<IActionResult> CreateTenant()
        //{
        //    string tenantid = await _acronisTokenService.CreateTenant();
        //    if (tenantid != null)
        //    {
        //        ViewData["Message"] = $"{tenantid}";
        //       return View();
        //        // Use the access token as needed
        //        // Perform your Acronis API requests with the token
        //    }
        //    else
        //    {
        //        ViewData["Message"] = "issues ";
        //        return View();
        //        // Handle the case where token retrieval failed
        //    }



        //}








    }
}
