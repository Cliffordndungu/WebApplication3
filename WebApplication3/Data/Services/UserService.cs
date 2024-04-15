using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using WebApplication3.Models;

namespace WebApplication3.Data.Services
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpcontext;
        private  readonly UserManager<ApplicationUser>  _userManager;

        public UserService(IHttpContextAccessor httpContext, UserManager<ApplicationUser> userManager)
        {
            _httpcontext = httpContext;
            _userManager = userManager;
        }
        public string GetUserId()
        {
            return _httpcontext.HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        }

        public bool IsAuthenticated()
        {
            return _httpcontext.HttpContext.User.Identity.IsAuthenticated;

        }

        public string GetSTPCustomerId()
        {
            var user = _userManager.GetUserAsync(_httpcontext.HttpContext.User).Result;

            // Check if the user exists and return the stopcustomerid property
            if (user != null)
            {
                return user.stpcustomerid;
            }

            // If the user is not found or if STPCustomerId is null, return null or an appropriate default value
            return null;
        }

        public string GettenandId()
        {
            var user = _userManager.GetUserAsync(_httpcontext.HttpContext.User).Result;

            // Check if the user exists and return the stopcustomerid property
            if (user != null)
            {
                return user.Tenantid;
            }

            // If the user is not found or if STPCustomerId is null, return null or an appropriate default value
            return null;
        }
    }
}
