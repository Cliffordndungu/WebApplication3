using System.Net;
using System.Text;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.AspNetCore.Http;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static System.Net.Mime.MediaTypeNames;

namespace WebApplication3.Data.DPO
{
    public class dpo
    {
        private const string DPO_URL = "https://secure.3gdirectpay.com/API/v6/";
        private const string COMPANY_TOKEN = "8D3DA73D-9D7F-4E09-96D4-3D44E7A83EA3";
        private const string ACCOUNT_TYPE = "YourAccountType";
 
   
        private const string REDIRECT_URL = "http://localhost:5000/Order/success";
        private const string BACK_URL = "http://localhost:5000/Order/failed";

        string companyToken = "8D3DA73D-9D7F-4E09-96D4-3D44E7A83EA3";
        string serviceType = "5525";
        string apiUrl = "https://secure.3gdirectpay.com/API/v6/";


        public async Task<string> CreateToken()
        {
            //string postXml = $@"<?xml version='1.0' encoding='utf-8'?>
            //<API3G>
            //    <CompanyToken>{COMPANY_TOKEN}</CompanyToken>
            //    <Request>createToken</Request>
            //    <!-- Add other required fields here -->
            //</API3G>";

            var endpoint = "https://secure.3gdirectpay.com/API/v6/";

            var xmlRequest = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<API3G>
  <CompanyToken>{companyToken}</CompanyToken>
  <Request>createToken</Request>
  <Transaction>
    <PaymentAmount>40.00</PaymentAmount>
    <PaymentCurrency>BWP</PaymentCurrency>
    <CompanyRef>49FKEOA</CompanyRef>
 <RedirectURL>http://www.sucesse.com/payurl.php</RedirectURL>
    <BackURL>http://www.failed.com/backurl.php</BackURL>
    <CompanyRefUnique>0</CompanyRefUnique>
    <PTL>5</PTL>
  </Transaction>
  <Services>
    <Service>
      <ServiceType>{serviceType}</ServiceType>
      <ServiceDescription>Cloud Backup - 400GB, EndPoint Protection</ServiceDescription>
      <ServiceDate>2023/05/21 19:00</ServiceDate>
    </Service>
  </Services>
</API3G>";


            var httpClient = new HttpClient();
            var stringContent = new StringContent(xmlRequest, Encoding.UTF8, "application/xml");
            var response = await httpClient.PostAsync(endpoint, stringContent);
            string responseContent = await response.Content.ReadAsStringAsync();

            // Parse the XML content
            XDocument xmlDoc = XDocument.Parse(responseContent);

            // Extract the values you need from the XML
            string resultCode = xmlDoc.Element("API3G").Element("Result").Value;
            string resultExplanation = xmlDoc.Element("API3G").Element("ResultExplanation").Value;
            string transToken = xmlDoc.Element("API3G").Element("TransToken").Value;

            return transToken;
        }

      
        public async Task<string> VerifyToken(string transToken)
        {
            var endpoint = "https://secure.3gdirectpay.com/API/v6/";
            string postXml = $@"<?xml version='1.0' encoding='utf-8'?>
            <API3G>
                <CompanyToken>{COMPANY_TOKEN}</CompanyToken>
                <Request>verifyToken</Request>
                <TransactionToken>{transToken}</TransactionToken>
            </API3G>";

            var httpClient = new HttpClient();
            var stringContent = new StringContent(postXml, Encoding.UTF8, "application/xml");
            var response = await httpClient.PostAsync(endpoint, stringContent);
            string responseContent = await response.Content.ReadAsStringAsync();

            // Parse the XML content
            XDocument xmlDoc = XDocument.Parse(responseContent);

            // Extract the values you need from the XML
            string resultCode = xmlDoc.Element("API3G").Element("Result").Value;
            string resultExplanation = xmlDoc.Element("API3G").Element("ResultExplanation").Value;
        

            return transToken;
        }

       
        public string GetPaymentUrl(string transToken)
        {
            //return $"https://secure1.sandbox.directpay.online/payv2.php?ID={transToken}";

            return $"https://secure.3gdirectpay.com/payv2.php?ID={transToken}";
        }


        //charge token {authorized token}
        //email token to client 

    }
}



