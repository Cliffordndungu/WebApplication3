using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Stripe;
using System.IO;
using WebApplication3.Migrations;

namespace WebApplication3.Controllers
{
    public class ManagementController : Controller
    {
        [HttpPost]
        public async Task<JsonResult> StoragePricingHandler(int StorageQuantity, int DeviceQuantity, string Plan)
        {
            try
            {
                // Parse quantity from selected value
                int storage = StorageQuantity;
                int devices = DeviceQuantity;
                string ProductPlan = Plan;
                //// Assuming PriceService.Get returns some kind of pricing information based on the provided identifier
                //var service = new PriceService();
                //var priceInfo = service.Get("price_1OrakiDprfyvhQjoLmzI7dKC");


               // Upcoming for an Invoice | .NET SDK

// Copy snippet

                var options = new UpcomingInvoiceOptions
                {
                    Customer = "cus_PJnTI05Yw7lNhG",
                    SubscriptionItems = new List<InvoiceSubscriptionItemOptions>
                 {
                        new InvoiceSubscriptionItemOptions
                        {
                            Price = "price_1OrakiDprfyvhQjoLmzI7dKC",
                            Quantity = storage,
                        },
                        new  InvoiceSubscriptionItemOptions
                        {
                             Price = ProductPlan,
                            Quantity = devices,
                        }
                    },
                };
                var service = new InvoiceService();
               var UpcomingPrice =  service.Upcoming(options);
                int totalupcomingprice = (int)UpcomingPrice.AmountDue / 100;








                //var options = new UpcomingInvoiceOptions { Customer = "cus_NeZwdNtLEOXuvB", SubscriptionItems[0] };
                //var service1 = new InvoiceService();
                //service1.Upcoming(options);




                // Extract pricing tiers
                //var tiers = priceInfo.Tiers;

                //// Find the applicable tier for the given quantity
                //decimal totalPrice = 0;
                //foreach (var tier in tiers)
                //{
                //    if (quantity <= tier.UpTo )
                //    {
                //        // Calculate the total price based on the tier price and quantity
                //        totalPrice = quantity * (decimal)tier.UnitAmount;
                //        break;
                //    }
                //}

                //if (totalPrice == 0)
                //{
                //    // Handle cases where the quantity doesn't fall within any defined tiers
                //    throw new Exception("Quantity does not fall within any defined tiers.");
                //}

                return new JsonResult(new { totalupcomingprice });

            }
            catch (Exception ex)
            {
                // Handle exceptions
                return Json(new { error = ex.Message });
            }
        }

        

    }
}
