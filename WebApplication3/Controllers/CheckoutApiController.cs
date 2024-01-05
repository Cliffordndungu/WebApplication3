using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Elfie.Model.Strings;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Stripe;
using WebApplication3.Data.Cart;
using WebApplication3.Data.ViewModels;
using WebApplication3.Models;
using static WebApplication3.Models.StripeModel;

namespace WebApplication3.Controllers
{
   
    public class CheckoutApiController : Controller
    {
        private readonly WebApplication3.Data.Services.SubscriptionsService _subscriptionService;
        private readonly ShoppingCart _shoppingcart;
        private readonly AcronisTokenService _acronisservice;
        private readonly UserManager<ApplicationUser> _usermanager;


        public CheckoutApiController(UserManager<ApplicationUser> usermanager, WebApplication3.Data.Services.SubscriptionsService subscriptionService, ShoppingCart shoppingcart, AcronisTokenService acronistokenservice)
        {
            _subscriptionService = subscriptionService;
            _shoppingcart = shoppingcart;
            _acronisservice = acronistokenservice;
            _usermanager =  usermanager;
        }


        public async Task<IActionResult> checkout()
        {
            var items = _shoppingcart.GetShoppingCartItems();
            _shoppingcart.ShoppingCartItems = items;

            // Fetch product prices from Stripe for each item in the shopping cart
            var service = new ProductService();
            var priceService = new PriceService();

            var productPrices = new List<ProductPrice>();

            foreach (var item in items)
            {
                var product = service.Get(item.productid);
                var price = priceService.Get(product.DefaultPriceId);

                // Convert price to your desired format (e.g., currency)
                var formattedPrice = (price.UnitAmountDecimal ?? 0) / 100; // Convert to currency format


                productPrices.Add(new ProductPrice
                {
                    ProductId = item.productid,
                    Price = formattedPrice
                });
            }

            var viewModel = new ShoppingCartVM
            {
                ShoppingCart = _shoppingcart,
                ShoppingCartTotal = await _shoppingcart.GetShoppingCartTotalAsync(),
                ProductPrices = productPrices  // Pass the list of product prices to the view model
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Index()
        {
            var items = _shoppingcart.GetShoppingCartItems();
            _shoppingcart.ShoppingCartItems = items;

            // Fetch product prices from Stripe for each item in the shopping cart
            var service = new ProductService();
            var priceService = new PriceService();

            var productPrices = new List<ProductPrice>();

            foreach (var item in items)
            {
                var product = service.Get(item.productid);
                var price = priceService.Get(product.DefaultPriceId);

                // Convert price to your desired format (e.g., currency)
                var formattedPrice = (price.UnitAmountDecimal ?? 0) / 100; // Convert to currency format


                productPrices.Add(new ProductPrice
                {
                    ProductId = item.productid,
                    Price = formattedPrice
                });
            }

            var viewModel = new ShoppingCartVM
            {
                ShoppingCart = _shoppingcart,
                ShoppingCartTotal = await _shoppingcart.GetShoppingCartTotalAsync(),
                ProductPrices = productPrices  // Pass the list of product prices to the view model
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<JsonResult> TrialSubscriptionCreateHandler(string username, string Productid, long quantity)
        {
            try { 
            //get stripe customer id 
            ////get username 
            var user = await _usermanager.FindByNameAsync(username);
              
            var customer = CreateCust(username, user.Email);
            user.stpcustomerid = customer.Id;
            await _usermanager.UpdateAsync(user);
            
            var subscription = CreateTrialSubscription(Productid, customer, quantity);
            user.TrialEndDate = subscription.TrialEnd;
            await _usermanager.UpdateAsync(user);

            ////get username 
           
            if (user.Tenantid == null)
            {
                var newtenantid = await _acronisservice.CreateTenant(username);
                user.Tenantid = newtenantid;

                // Update user in the database
                var updateResult = await _usermanager.UpdateAsync(user);

                if (updateResult.Succeeded)
                {
                    // Update successful
                    // apply quota or offereing ?
                }
                else
                {
                    // Handle update failure
                }

            }

            //set offerings
            _acronisservice.ManageOfferings("subscribe", user.Tenantid, Productid, quantity);

            //set user
            _acronisservice.ManageUsers(user.Tenantid, user.Email);

            return Json(JsonConvert.SerializeObject(subscription, Formatting.Indented));
        }
            catch (StripeException e)
            {
                return Json(new { Success = "False", responseText = e.Message
    });
            }
        }

        [HttpPost]
        public async Task<JsonResult> SubscriptionCreateHandler(string Name, string Email, string PaymentMethodId, string PriceId, long Quantity, string username)
        {
            try
            {
                //get stripe customer id 
                var customerid = await GetCustomerId(Name, username, Email, PaymentMethodId);
              
                var subscription = CreateSubscription(PriceId, customerid, Quantity);

              
                ////
                ////get username 
                var user = await _usermanager.FindByNameAsync(username);
                if (user.Tenantid == null)
                {
                    var newtenantid = await _acronisservice.CreateTenant(username);
                    user.Tenantid = newtenantid;

                    // Update user in the database
                    var updateResult = await _usermanager.UpdateAsync(user);

                    if (updateResult.Succeeded)
                    {
                        // Update successful
                        // apply quota or offereing ?
                    }
                    else
                    {
                        // Handle update failure
                    }

                }

            

                //set offerings
                _acronisservice.ManageOfferings("subscribe", user.Tenantid, PriceId, Quantity);

                //set user
                _acronisservice.ManageUsers(user.Tenantid, user.Email);

                //return tenant id and customer id maybe
                //Apply tenant offering
                //apply quota 
                //create user


                return Json(JsonConvert.SerializeObject(subscription, Formatting.Indented));
            }
            catch (StripeException e)
            {
                return Json(new { Success = "False", responseText = e.Message });
            }
        }

        [HttpPost]
        public JsonResult SubscriptionRetrieveHandler(string subscriptionId)
        {
            // Disabling JsonIgnore in Subscription() for CustomerId and LatestInvoiceId
            var settings = new JsonSerializerSettings();
            settings.ContractResolver = new IncludeAllPropertiesContractResolver();
            // Disabling JsonIgnore in Subscription() for CustomerId and LatestInvoiceId

            try
            {
                var subscription = BaseClass.RetrieveSubscription(subscriptionId);
                return Json(JsonConvert.SerializeObject(subscription, settings));
            }
            catch (StripeException e)
            {
                return Json(new { Success = "False", responseText = e.Message });
            }
        }
        public class IncludeAllPropertiesContractResolver : DefaultContractResolver
        {
            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
            {
                IList<JsonProperty> properties = base.CreateProperties(type, memberSerialization);

                // Or other way to determine...
                foreach (var jsonProperty in properties)
                {
                    // Include all properties.
                    jsonProperty.Ignored = false;
                }
                return properties;
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

        public static Customer CreateCust(string name, string email)
        {
            var customerOptions = new CustomerCreateOptions
            {
                Name = name,
                Email = email,
            };
            var customerService = new CustomerService();
            var customer = customerService.Create(customerOptions);

            return customer;
        }

        public async Task<string> GetCustomerId(string Name, string username, string Email, string PaymentMethodId)
        {
            var user = await _usermanager.FindByNameAsync(username);
            if (user.stpcustomerid == null)
            {
                var customer = CreateCustomer(Name, Email, PaymentMethodId);
                user.stpcustomerid = customer.Id;
                var customerupdateresult = await _usermanager.UpdateAsync(user);
                return user.stpcustomerid;
            }
            else
            {
                UpdateCustomer(user.stpcustomerid, PaymentMethodId);
                return user.stpcustomerid;
            }

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
            //get price id here
            // Fetch product prices from Stripe for each item in the shopping cart
            var service = new ProductService();
            var priceService = new PriceService();
            var product = service.Get(priceId);
            var priceid = product.DefaultPriceId;

            var subscriptionOptions = new SubscriptionCreateOptions
            {
                Customer = customerId,
                Items = new List<SubscriptionItemOptions>
                    {
                        new SubscriptionItemOptions
                        {
                            Price = priceid,
                            Quantity = quantity,
                        },
                    },
                //TrialEnd = DateTimeOffset.FromUnixTimeSeconds(1610403705).UtcDateTime,
            };
            subscriptionOptions.AddExpand("latest_invoice.payment_intent");

            var subscriptionService = new SubscriptionService();
            Subscription subscription = subscriptionService.Create(subscriptionOptions);
            return subscription;
        }
        public static Subscription CreateTrialSubscription(string Productid, Customer customer, long quantity)
        {
            
            // Fetch product prices from Stripe for each item in the shopping cart
            var service = new ProductService();
            var priceService = new PriceService();
            var product = service.Get(Productid);
            var priceid = product.DefaultPriceId;

            DateTimeOffset thirtyDaysFromToday = DateTimeOffset.UtcNow.AddDays(30);
          
            long unixTimestamp = (long)(thirtyDaysFromToday.ToUnixTimeSeconds());
           
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(unixTimestamp);
            DateTime dateTimeUtc = dateTimeOffset.UtcDateTime;
            var subscriptionOptions = new SubscriptionCreateOptions
            {
                Customer = customer.Id,
                Items = new List<SubscriptionItemOptions>
                    {
                        new SubscriptionItemOptions
                        {
                            Price = priceid,
                            Quantity = quantity,
                        },
                    },
                TrialEnd = dateTimeUtc,
                TrialSettings = new SubscriptionTrialSettingsOptions
                {
                    EndBehavior = new SubscriptionTrialSettingsEndBehaviorOptions
                    {
                        MissingPaymentMethod = "pause",
                    },
                },
            };
            subscriptionOptions.AddExpand("latest_invoice.payment_intent");

            var subscriptionService = new SubscriptionService();
            Subscription subscription = subscriptionService.Create(subscriptionOptions);
            return subscription;
        }

    }
}
