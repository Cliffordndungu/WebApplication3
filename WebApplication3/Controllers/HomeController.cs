using Microsoft.AspNetCore.Mvc;
using Stripe;
using System.Diagnostics;
using WebApplication3.Data.Services;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserService _userservice;
        private readonly IEmailSender _emailservice;

        public HomeController(ILogger<HomeController> logger, IUserService userService, IEmailSender emailservice)
        {
            _logger = logger;
            _userservice = userService;
            _emailservice = emailservice;
        }

        public async Task<ViewResult> Index()
        {
            UserEmailOptions emailOptions = new UserEmailOptions()
            {
                ToEmails = new List<string>() { "cliffordndungu@hotmail.com" },
                Placeholders = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("{{UserName}}", "Clifford")
                }
            };
            


            await _emailservice.SendTestEmail(emailOptions);
            
            
            var isLoggedin = _userservice.IsAuthenticated();


            // get list of products and randomise and save to model
            List<Index_Products> products = new List<Index_Products>();
            var options = new ProductListOptions
            {
                Active = true,
            };
            var service = new ProductService();
            StripeList<Stripe.Product> productList = service.List(options);

            foreach (var stripeProduct in productList)
            {
               Index_Products product = new Index_Products
                {
                    id = stripeProduct.Id,
                    Name = stripeProduct.Name,
                    Description = stripeProduct.Description,
                    price = stripeProduct.Metadata.ContainsKey("price") ? Convert.ToDecimal(stripeProduct.Metadata["price"]) : 0,
                   // Extracting feature list from Metadata and converting it to an array
                   //FeatureList = stripeProduct.Metadata.ContainsKey("featurelist") ? stripeProduct.Metadata["featurelist"].Split(',') : new string[0]
                   FeatureList = stripeProduct.Features.Select(feature => feature.Name).ToArray()
                //ImageUrl = stripeProduct.Images.FirstOrDefault()
                //engine = stripeProduct.Metadata.ContainsKey("Engine") ? stripeProduct.Metadata["Engine"] : null,
                //compatibility = stripeProduct.Metadata.ContainsKey("Comp") ? stripeProduct.Metadata["Comp"] : null,
                //subscription = stripeProduct.Metadata.ContainsKey("Subscription") ? stripeProduct.Metadata["Subscription"] : null,
                //productcode = stripeProduct.Metadata.ContainsKey("colorcode") ? stripeProduct.Metadata["colorcode"] : null,
                //datacenterlocation = stripeProduct.Metadata.ContainsKey("Datacenterlocation") ? stripeProduct.Metadata["Datacenterlocation"] : null,
            };

                products.Add(product);
            }

            return View(products);
        }


    

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}