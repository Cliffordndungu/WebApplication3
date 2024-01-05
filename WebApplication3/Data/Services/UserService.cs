using System.Security.Claims;

namespace WebApplication3.Data.Services
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpcontext;
        public UserService(IHttpContextAccessor httpContext)
        {
            _httpcontext = httpContext;
        }
        public string GetUserId()
        {
            return _httpcontext.HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        }

        public bool IsAuthenticated()
        {
            return _httpcontext.HttpContext.User.Identity.IsAuthenticated;

        }
    }
}
