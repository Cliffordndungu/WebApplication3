using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Differencing;
using Newtonsoft.Json;
using Stripe;
using WebApplication3.Data.Services;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    public class SubController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserService _userService;
        private readonly ISubscriptionsService _subservice;

        public SubController(UserManager<ApplicationUser> userManager, IUserService userservice, ISubscriptionsService subservice)
        {
            _userManager = userManager;
            _userService = userservice;
            _subservice = subservice;
        }
        public async Task<IActionResult> IndexAsync()
        {

            var user = await _userManager.GetUserAsync(User);
            List<Subscriptions> subscriptions2 = new List<Subscriptions>();
            var options = new SubscriptionListOptions
            {
                Customer = user.stpcustomerid,
                Status = "all"
            };
            var service = new SubscriptionService();
            StripeList<Subscription> subscriptions = service.List(
              options);

            foreach (var subscription in subscriptions)
            {
                Subscriptions subscriptions1 = new Subscriptions
                {
                    id = subscription.Id,
                    product = subscription.Items?.Data.FirstOrDefault()?.Price?.Id,
                    interval = subscription.Items?.Data.FirstOrDefault()?.Price?.Recurring?.Interval,
                    quantity = subscription.Items?.Data.FirstOrDefault()?.Quantity,
                    name = subscription.Items?.Data.FirstOrDefault()?.Price?.Product?.Name ?? "Product Name Not Available", // Retrieve product name or set a default value
                    renewaldate = (DateTime)subscription.CurrentPeriodEnd, // Renewal date
                    status = subscription.Status, // Subscription status

                };
                subscriptions2.Add(subscriptions1);

            }

            return View(subscriptions2);
        }
        [HttpPost]
        public async Task<IActionResult> CancelSubscription(string subid)
        {
            var service = new SubscriptionService();
            service.Cancel(subid);
            //acronis disable tenant
            return RedirectToAction("IndexAsync");
        }

        public async Task<IActionResult> Manage(string id)
        {
            var service = new SubscriptionService();
            var subscription = service.Get(id);


            Subscriptions subscriptionitems = new Subscriptions
            {
                id = subscription.Id,
                product = subscription.Items?.Data.FirstOrDefault()?.Price?.Id,
                interval = subscription.Items?.Data.FirstOrDefault()?.Price?.Recurring?.Interval,
                quantity = subscription.Items?.Data.FirstOrDefault()?.Quantity,
                name = subscription.Items?.Data.FirstOrDefault()?.Price?.Product?.Name ?? "Product Name Not Available", // Retrieve product name or set a default value
                renewaldate = (DateTime)subscription.CurrentPeriodEnd, // Renewal date
                unitamount = subscription.Items?.Data.FirstOrDefault()?.Price?.UnitAmount, // Assuming this property holds the unit amount information
                startdate = (DateTime)subscription.Created,
                subitemid = subscription.Items?.Data.FirstOrDefault()?.Id,
                status = subscription.Status // Subscription status

            };

            return View(subscriptionitems);

        }

        [HttpPost]
       
        public JsonResult UpdateSubscription(string subitemid, string priceid)
        {

            string tenantid =  _userService.GettenandId();
            int quantity = 10;

            try
            {
                _subservice.CustomerLicenseIncrement(subitemid,  priceid, quantity, tenantid);

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

            

 public async Task<JsonResult> RemoveLicenseHandler(string Subid, long Quantity)
            {
                try
                {

                    var options = new SubscriptionItemUpdateOptions
                    {
                        Quantity = Quantity
                    };
                    var service = new SubscriptionItemService();
                    var response = service.Update(Subid, options);


                    return Json(JsonConvert.SerializeObject(response, Formatting.Indented));
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
