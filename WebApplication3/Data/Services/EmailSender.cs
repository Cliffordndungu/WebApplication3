using Microsoft.Extensions.Options;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;
using System.Net;
using System.Net.Mail;
using System.Text;
using WebApplication3.Models;

namespace WebApplication3.Data.Services
{
    public class EmailSender : IEmailSender
    {
        private const string templatepath = @"EmailTemplate/{0}.html";
        private readonly SMTPConfigModel _smtpConfig;

        public async Task SendTestEmail(UserEmailOptions userEmailOptions)
        {
            userEmailOptions.Subject = UpdatePlaceholders("Hello {{UserName}} This is a test" , userEmailOptions.Placeholders);
            userEmailOptions.Body = UpdatePlaceholders(GetEmailBody("TestEmail"), userEmailOptions.Placeholders);
            await SendEmail(userEmailOptions);
        }

        public async Task SendEmailForForgotPassword(UserEmailOptions userEmailOptions)
        {
            userEmailOptions.Subject = UpdatePlaceholders("Hello {{UserName}}, reset your password.", userEmailOptions.Placeholders);
            userEmailOptions.Body = UpdatePlaceholders(GetEmailBody("ForgotPassword"), userEmailOptions.Placeholders);
            await SendEmail(userEmailOptions);
        }

        public async Task SendEmailForEmailConfirmation(UserEmailOptions userEmailOptions)
        {
            userEmailOptions.Subject = UpdatePlaceholders("Hello {{UserName}}, Confirm your email id.", userEmailOptions.Placeholders);
            userEmailOptions.Body = UpdatePlaceholders(GetEmailBody("EmailConfirm"), userEmailOptions.Placeholders);
            await SendEmail(userEmailOptions);
        }
        public EmailSender(IOptions<SMTPConfigModel> smtpConfig)
        {
            _smtpConfig = smtpConfig.Value;
        }
        public async Task SendEmail(UserEmailOptions userEmailOptions)
        {
            MailMessage mail = new MailMessage()
            {
                Subject = userEmailOptions.Subject,
                Body = userEmailOptions.Body,
                From = new MailAddress(_smtpConfig.SenderAddress, _smtpConfig.SenderDisplayName),
                IsBodyHtml = _smtpConfig.IsBodyHTML
            };

            foreach (var toEmail in userEmailOptions.ToEmails)
            {
                mail.To.Add(toEmail);
            }

            NetworkCredential networkCredential = new NetworkCredential(_smtpConfig.UserName, _smtpConfig.Password);

            SmtpClient smtpclient = new SmtpClient
            {
                Host = _smtpConfig.Host,
                Port = _smtpConfig.Port,
                EnableSsl = _smtpConfig.EnableSSL,
                UseDefaultCredentials = _smtpConfig.UseDefaultCredentials,
                Credentials = networkCredential
            };
            mail.BodyEncoding = Encoding.Default;

            try
            {
                // Attempt to send the email
                await smtpclient.SendMailAsync(mail);
            }
            catch (Exception ex)
            {
                // Handle the exception (you can log it or perform other actions)
                Console.WriteLine($"An error occurred while sending email: {ex.Message}");
                // You might want to throw the exception again to propagate it or handle it accordingly
                throw;
            }
            
        }

        private string GetEmailBody(string templateName)
        {
            var body = File.ReadAllText(string.Format(templatepath, templateName));
            return body;    
        }


        private string UpdatePlaceholders(string text, List<KeyValuePair<string, string>> keyValuePairs) {
        
        if (!string.IsNullOrEmpty(text)  && keyValuePairs != null)
            {
                foreach (var keyValuePair in keyValuePairs)
                {
                    if (text.Contains(keyValuePair.Key))
                    {
                        text = text.Replace(keyValuePair.Key, keyValuePair.Value);
                    }
                }
            }

        return text;

        }

        public async Task SendEmailOrderConfirmation(UserEmailOptions userEmailOptions)
        {
            userEmailOptions.Subject = UpdatePlaceholders("Hello {{UserName}},We have recieved your order.", userEmailOptions.Placeholders);
            userEmailOptions.Body = UpdatePlaceholders(GetEmailBody("OrderSuccess"), userEmailOptions.Placeholders);
            await SendEmail(userEmailOptions);
        }


    }
}
