using Microsoft.AspNetCore.Mvc;
using Twilio;
using Twilio.Rest.Chat.V1;
using Twilio.Rest.Verify.V2.Service;

namespace WebApplication3.Data

{
    public class TwilioService
    {
        private readonly string SID;
        private readonly string Token;
        private readonly string Service;
        public TwilioService()
        {
            SID = "AC17b48992121680e847b148c04e81c838";
            Token = "6f7742e96cfcadaca3559eb7d079f058";
            Service = "VA2fba99ef745084ae5978fee18bc6562c";

        }

            //Generate code

        public string SendVerificationToken(string phonenumber)
        {
            TwilioClient.Init(SID, Token);
            CreateVerificationOptions options = new CreateVerificationOptions(Service, phonenumber, "sms");
            var verification = VerificationResource.Create(options);
            return verification.ServiceSid;
        }

        public string CheckVerificationToken(string phonenumber, string code, string serviceSid)
        {
            TwilioClient.Init(SID, Token);
          

            var verificationCheck = VerificationCheckResource.Create(
                to: phonenumber,
                code: code,
                pathServiceSid: serviceSid

                );

            return verificationCheck.Status;
        }

     


    }
}
