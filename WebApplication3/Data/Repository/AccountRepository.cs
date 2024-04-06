using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Stripe;
using System;
using WebApplication3.Data.Services;
using WebApplication3.Migrations;
using WebApplication3.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace WebApplication3.Data.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUserService _userservice;
        private readonly AcronisTokenService _acronisservice;
        private readonly IEmailSender _emailservice;
        private readonly IConfiguration _configuration;


        public AccountRepository(AcronisTokenService acronisservice, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IUserService userservice, IEmailSender emailservice, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userservice = userservice;
            _emailservice = emailservice;
            _configuration = configuration;
            _acronisservice = acronisservice;
        }

        public async Task<IdentityResult> ChangePasswordAsync (ChangePasswordModel model)
        {
            var userid = _userservice.GetUserId();
            var user = await _userManager.FindByIdAsync(userid);
            return await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
        }

        public async Task<IdentityResult> ConfirmEmailAsync(string uid, string token)
        {
            return await _userManager.ConfirmEmailAsync(await _userManager.FindByIdAsync(uid), token);
        }


        public async Task<IdentityResult> ResetPasswordAsync(ResetPasswordModel model)
        {
            return await _userManager.ResetPasswordAsync( await _userManager.FindByIdAsync(model.UserId), model.Token, model.NewPassword);
        }
        public async Task<ApplicationUser> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }
        public async Task GenerateEmailConfirmationTokenAsync(ApplicationUser user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            if (!string.IsNullOrEmpty(token))
            {
                await SendEmailConfirmationEmail(user, token);
            }
        }

        public async Task GenerateForgotPasswordTokenAsync(ApplicationUser user)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            if (!string.IsNullOrEmpty(token))
            {
                await SendForgotPasswordEmail(user, token);
            }
        }

        public async Task SendEmailConfirmationEmail(ApplicationUser user, string token)
        {
            string appdomain = _configuration.GetSection("Application:AppDomain").Value;
            string confirmationLink = _configuration.GetSection("Application:EmailConfirmation").Value;
            UserEmailOptions emailOptions = new UserEmailOptions()
            {
                ToEmails = new List<string>() { user.Email },
                Placeholders = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("{{UserName}}", user.FullName),
                    new KeyValuePair<string, string>("{{Link}}", string.Format(appdomain + confirmationLink, user.Id, token))
                }
            };

            await _emailservice.SendEmailForEmailConfirmation(emailOptions);
        }

        public async Task SendForgotPasswordEmail(ApplicationUser user, string token)
        {
            string appdomain = _configuration.GetSection("Application:AppDomain").Value;
            string confirmationLink = _configuration.GetSection("Application:ForgotPassword").Value;
            UserEmailOptions emailOptions = new UserEmailOptions()
            {
                ToEmails = new List<string>() { user.Email },
                Placeholders = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("{{UserName}}", user.FullName),
                    new KeyValuePair<string, string>("{{Link}}", string.Format(appdomain + confirmationLink, user.Id, token))
                }
            };

            await _emailservice.SendEmailForForgotPassword(emailOptions);
        }

        public async Task CreateCustomer(string name, string email,string userid)
        {

            // Get the user with the provided userid
            var user = await _userManager.FindByIdAsync(userid);

            var customerOptions = new CustomerCreateOptions
            {
                Name = name,
                Email = email,
            };
            var customerService = new CustomerService();
            var customer = customerService.Create(customerOptions);

            //get the user and update the user
            //get user with userid 

            user.stpcustomerid = customer.Id;
            await _userManager.UpdateAsync(user);


        }

        public async Task SendOrderConfirmation(string customerid)
        {

            //get email using usermANAGER for user with stripecustomerid 
            //get name 
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.stpcustomerid == customerid);



            UserEmailOptions emailOptions = new UserEmailOptions()
            {
                ToEmails = new List<string>() { user.Email },
                Placeholders = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("{{UserName}}", user.FullName),
                  
                }
            };

            await _emailservice.SendEmailOrderConfirmation(emailOptions);

        }
    
    public async Task<string> Gettenantid(string customerid)
        {
            //get email using usermANAGER for user with stripecustomerid 
            //get name 
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.stpcustomerid == customerid);

            if (user.Tenantid == null)
            {
                var newtenantid = await _acronisservice.CreateTenant(user.FullName);
                user.Tenantid = newtenantid;

                // Update user in the database
                var updateResult = await _userManager.UpdateAsync(user);

                if (updateResult.Succeeded)
                {
                    return user.Tenantid;
                }
                else
                {
                    return "failed";
                    // Handle update failure
                }

            }
            return "failed";

        }

    }




}
