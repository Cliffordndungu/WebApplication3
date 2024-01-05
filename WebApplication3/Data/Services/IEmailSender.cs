using WebApplication3.Models;

namespace WebApplication3.Data.Services
{
    public interface IEmailSender
    {
        Task SendTestEmail(UserEmailOptions userEmailOptions);
        Task SendEmailForEmailConfirmation(UserEmailOptions userEmailOptions);

        Task SendEmailForForgotPassword(UserEmailOptions userEmailOptions);

    }
}
