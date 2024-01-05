using Microsoft.AspNetCore.Identity;
using WebApplication3.Models;

namespace WebApplication3.Data.Repository
{
    public interface IAccountRepository
    {

        Task<IdentityResult> ChangePasswordAsync(ChangePasswordModel model);

        Task SendEmailConfirmationEmail(ApplicationUser user, string token);
        Task<IdentityResult> ConfirmEmailAsync(string uid, string token);
        Task<ApplicationUser> GetUserByEmailAsync(string email);
        Task GenerateEmailConfirmationTokenAsync(ApplicationUser user);

        Task GenerateForgotPasswordTokenAsync(ApplicationUser user);

        Task<IdentityResult> ResetPasswordAsync(ResetPasswordModel model);

    }
}
