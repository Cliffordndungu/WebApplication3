using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Stripe;
using Stripe.Checkout;
using WebApplication3.Data.Cart;
using WebApplication3.Migrations;
using static WebApplication3.Models.StripeModel;

namespace WebApplication3.Controllers
{
    public class SubscriptionController : Controller
    {
        private readonly ShoppingCart _shoppingCart;
        public SubscriptionController(ShoppingCart shoppingCart)
        {
            _shoppingCart = shoppingCart;
        }

        [HttpPost]
        public JsonResult SubscriptionCreateHandler(string Name, string Email, string PaymentMethodId, string PriceId, long Quantity)
        {
            try
            {
                var customer = CreateCustomer(Name, Email, PaymentMethodId);
                var subscription = CreateSubscription(PriceId, customer.Id, Quantity);
                return Json(JsonConvert.SerializeObject(subscription, Formatting.Indented));
            }
            catch (StripeException e)
            {
                return Json(new { Success = "False", responseText = e.Message });
            }
        }
        public static Customer CreateCustomer(string name, string email, string paymentMethodId)
        {
            var customerOptions = new CustomerCreateOptions
            {
                Name = name,
                Email = email,
            };
            var customerService = new CustomerService();
            var customer = customerService.Create(customerOptions);

            UpdateCustomer(customer.Id, paymentMethodId);

            return customer;
        }

        public static void UpdateCustomer(string customerId, string paymentMethodId)
        {
            var paymentMethodAttachOptions = new PaymentMethodAttachOptions
            {
                Customer = customerId,
            };
            var paymentMethodservice = new PaymentMethodService();
            paymentMethodservice.Attach(paymentMethodId, paymentMethodAttachOptions);

            var customerOptions = new CustomerUpdateOptions
            {
                InvoiceSettings = new CustomerInvoiceSettingsOptions
                {
                    DefaultPaymentMethod = paymentMethodId,
                },
            };
            var customerService = new CustomerService();
            customerService.Update(customerId, customerOptions);
        }

        public static Subscription CreateSubscription(string priceId, string customerId, long quantity)
        {
            var subscriptionOptions = new SubscriptionCreateOptions
            {
                Customer = customerId,
                Items = new List<SubscriptionItemOptions>
                    {
                        new SubscriptionItemOptions
                        {
                            Price = priceId,
                            Quantity = quantity,
                        },
                    },
            };
            subscriptionOptions.AddExpand("latest_invoice.payment_intent");

            var subscriptionService = new SubscriptionService();
            Subscription subscription = subscriptionService.Create(subscriptionOptions);
            return subscription;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetShoppingCartTotal()
        {
            try
            {
                double totalAmount = await _shoppingCart.GetShoppingCartTotalAsync(); // Call your existing method

                return Json(totalAmount); // Return the total amount as JSON
            }
            catch (Exception ex)
            {
                // Handle any exceptions and return an error response if necessary
                return BadRequest("Error fetching total amount: " + ex.Message);
            }
        }


        public IActionResult checkout()
        {
            return View();
        }

        public IActionResult success()
        {
            return View();
        }

        [HttpPost("create-checkout-session")]
        public async Task<ActionResult> CreateCheckoutSession()
        {

            var amount = await _shoppingCart.GetShoppingCartTotalAsync();

            // Convert the double to a long
            long amountInCents = (long)(amount * 100); // Assuming the amount is in dollars, convert it to cents.

            var quantity = _shoppingCart.GetShoppingCartItems().Count();

            long unitquantities = quantity;

            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>
                 {
                    new SessionLineItemOptions
                    {
                     PriceData = new SessionLineItemPriceDataOptions
                         {
                         UnitAmount = amountInCents,
                         Currency = "bwp",
                         ProductData = new SessionLineItemPriceDataProductDataOptions
                         {
                            Name = "Testing",
                         },
                    },
                    Quantity = unitquantities,
                },
            },
                Mode = "payment",
                SuccessUrl = "http://localhost:7218/Subscription/success",
                CancelUrl = "http://localhost:7218/cancel",
            };

            var service = new SessionService();
            Session session = service.Create(options);

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }
    }
}

