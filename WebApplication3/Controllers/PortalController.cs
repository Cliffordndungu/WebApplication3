using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Twilio.Rest.Intelligence.V2;
using WebApplication3.Data.Services;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    public class PortalController : Controller

    {


    private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserService _userservice;
        private readonly ISubscriptionsService _subscriptionsService;

        public PortalController(UserManager<ApplicationUser> userManager, IUserService userservice, ISubscriptionsService subscriptionservice)
        {
            _userManager = userManager;
            _userservice = userservice;
            _subscriptionsService = subscriptionservice;
            
        }

        [Authorize]
        public IActionResult Home()
        {
            return View();
        }
        [Authorize]
        public async Task<IActionResult> Billing()
        {
            var productservice = new ProductService();
            var user = await _userManager.GetUserAsync(User);
            List<Subscriptions> subscriptions2 = new List<Subscriptions>();
            var options = new SubscriptionListOptions
            {
                Customer = user.stpcustomerid,
                Status = "all"
            };
            var service = new SubscriptionService();
            StripeList<Subscription> subscriptions = service.List(options);
            List<SubscriptionItems> subscriptionsList = new List<SubscriptionItems>();

            foreach (var subscription in subscriptions)
            {

                //get all subitemid 
                //put all items into a list 

                var subscriptionitemoptions = new SubscriptionItemListOptions
                {
                    Subscription = subscription.Id,
                };
                var subitemservice = new SubscriptionItemService();
                StripeList<SubscriptionItem> subscriptionItems = subitemservice.List(subscriptionitemoptions);


                // for each items in subscription list put it into the viewmodel object subscription items. 


                foreach (var subscriptionItem in subscriptionItems)
                {
             
                    var subcriptionproduct = productservice.Get(subscriptionItem.Price.ProductId);

                    SubscriptionItems subscriptionitemsModel = new SubscriptionItems
                    {
                        id = subscriptionItem.Id,
                        product = subscriptionItem.Price?.Id,
                        interval = subscriptionItem.Price?.Recurring?.Interval,
                        quantity = subscriptionItem.Quantity,
                        name = subcriptionproduct.Name ?? "Product Name Not Available",
                        renewaldate = (DateTime)subscription.CurrentPeriodEnd,
                        status = subscription.Status,
                       
                    };
                    subscriptionsList.Add(subscriptionitemsModel);
                }
            }

            // Pass subscriptionsList to the view
            return View(subscriptionsList);
        }


      
       
        [Authorize]
        public IActionResult Support()
        {
            return View();
        }
        [Authorize]
        public IActionResult Profile()
        {
            return View();
        }

        [Authorize]
        public IActionResult Manage(string id)
        {
           
            var subscriptionitemservice = new SubscriptionItemService();
            var subscriptionservice = new SubscriptionService();
            var subscriptionitem = subscriptionitemservice.Get(id);
            var subscription = subscriptionservice.Get(subscriptionitem.Subscription);
            var productservice = new ProductService();

            
            var product = productservice.Get(subscriptionitem.Price.ProductId);
            var monthlytotal = subscriptionitem.Quantity * subscriptionitem.Price.UnitAmount;

            SubscriptionItems subscriptionitemobject = new SubscriptionItems
            {

                id = subscriptionitem.Id,
                product = subscriptionitem.Price?.Id,
                interval = subscriptionitem.Price?.Recurring?.Interval,
                quantity = subscriptionitem.Quantity,
                name = product.Name ?? "Product Name Not Available",
                renewaldate = (DateTime)subscription.CurrentPeriodEnd,
                status = subscription.Status,
                startdate = (DateTime)subscription.Created,
                unitamount = (double)subscriptionitem.Price.UnitAmount,
                monthlyprice = monthlytotal

            };
           

            return View(subscriptionitemobject);
        
        }

        [HttpPost]

        public JsonResult UpdateSubscription(string subitemid, string priceid, int quantity)
        {

            string tenantid = _userservice.GettenandId();
         

            try
            {
                _subscriptionsService.CustomerLicenseIncrement(subitemid, priceid, quantity, tenantid);

                return Json(new
                {
                    Success = true,
                    Message = "Operation completed successfully."
                });
            }
            catch (StripeException e)
            {
                return Json(new
                {
                    Success = "False",
                    responseText = e.Message
                });
            }
        }

    }
}
