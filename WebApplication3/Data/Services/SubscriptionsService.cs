using Microsoft.AspNetCore.Mvc;
using Stripe;
using WebApplication3.Data.Cart;
using WebApplication3.Migrations;
using WebApplication3.Models;

namespace WebApplication3.Data.Services
{
    public class SubscriptionsService : ISubscriptionsService

    {
        private readonly ShoppingCart _ShoppingCart;
        private readonly AcronisTokenService _acronisservice;
        public SubscriptionsService(ShoppingCart shoppingCart, AcronisTokenService acronisservice)      
        {
            _ShoppingCart = shoppingCart;
            _acronisservice = acronisservice;
        }
        public async Task CreateSubscriptionsAsync(string customerId, List<ShoppingCartItem> items)
        {

             foreach (var item in items)
                {
                    var options = new SubscriptionCreateOptions
                    {
                        Customer = customerId,
                        Items = new List<SubscriptionItemOptions>
                    {
                        new SubscriptionItemOptions
                        {
                            Price = item.productid, // Assuming your product ID is the price ID.
                        },
                    },
                    };

                    Subscription subscription = null;

                    try
                    {
                        var service = new SubscriptionService();
                        subscription = await service.CreateAsync(options);

                        // Handle the subscription response for this item.
                        if (subscription != null)
                        {
                            // Subscription was created successfully for this item.
                        }
                        else
                        {
                            // Handle the case where the subscription is null (creation failed).
                        }
                    }
                    catch (StripeException ex)
                    {
                        // Handle any Stripe API exception here.
                        // You can log the error or take appropriate action.
                        // ex.Message contains the error message from Stripe.
                    }
                }
            }

        public async Task<PaymentIntent> CreatePaymentintent()  
        {
            var amount = await _ShoppingCart.GetShoppingCartTotalAsync();

            // Convert the double to a long
            long amountInCents = (long)(amount * 100); // Assuming the amount is in dollars, convert it to cents.

            var options = new PaymentIntentCreateOptions
            {
                Amount = amountInCents,
                Currency = "bwp"
            };

            var service = new PaymentIntentService();
            var intent = await service.CreateAsync(options);

            return intent;
        }

        public void CustomerLicenseIncrement(string subitemid, string priceid, int quantity, string tenantid)
        {

            //update subscription
            var options = new SubscriptionItemUpdateOptions
            {
                PaymentBehavior = "allow_incomplete",
                ProrationBehavior = "always_invoice", 
                Quantity = quantity,
            };
            var service = new SubscriptionItemService();
            service.Update(subitemid, options);

            //update tenant 

            _acronisservice.licenseincrement(priceid, quantity, tenantid);  



        }
    }
}

