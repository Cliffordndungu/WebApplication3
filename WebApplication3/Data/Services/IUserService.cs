namespace WebApplication3.Data.Services
{
    public interface IUserService
    {
        string GetUserId();
        bool IsAuthenticated();

        string GetSTPCustomerId();
    }
}