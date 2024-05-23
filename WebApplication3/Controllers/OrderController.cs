
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;
using WebApplication3.Data.Cart;
using WebApplication3.Data.DPO;
using WebApplication3.Data.Services;
using WebApplication3.Data.ViewModels;
using WebApplication3.Models;


namespace WebApplication3.Controllers
{

    public class OrderController : Controller
    {
        private readonly ShoppingCart _shoppingCart;
        private readonly OrdersService _orderservice;
      
        private readonly IUserService _userService;
        private readonly dpo _dposervice;

        public OrderController(ShoppingCart shoppingCart, OrdersService orderService,  IUserService userservice, dpo dposervice)
        {
            _shoppingCart = shoppingCart;
            _orderservice = orderService;
    
            _userService = userservice;
            _dposervice = dposervice;
        }

        [Route("Order/ShoppingCart")]
        [Route("Order/Cart")]
        public async Task<IActionResult> Cart()

        {
            var items = _shoppingCart.GetShoppingCartItems();
            _shoppingCart.ShoppingCartItems = items;

            // Fetch product prices from Stripe for each item in the shopping cart
            var service = new ProductService();
            var priceService = new PriceService();

            var productPrices = new List<ProductPrice>();
            var productDetails = new List<ProductDetails>();

            foreach (var item in items)
            {

                string productId = item.productid;
                //if product id is ..

                //check if the productid is for cloud storage
                if (productId == "prod_PgyiINyEyBPyea")
                {

                    // Extract quantity from the shopping cart item
                    var storage = item.quanitity;

                    var options = new UpcomingInvoiceOptions
                    {
                        Customer = "cus_PJnTI05Yw7lNhG",
                        SubscriptionItems = new List<InvoiceSubscriptionItemOptions>
                                {
                                 new InvoiceSubscriptionItemOptions
                                 {
                                     Price = "price_1OrakiDprfyvhQjoLmzI7dKC",
                                      Quantity = storage,
                                  }
                                 }
                    };

                    var Invoiceservice = new InvoiceService();
                    var UpcomingPrice = Invoiceservice.Upcoming(options);
                    long totalupcomingprice = UpcomingPrice.AmountDue / 100;
                    long unitcost = totalupcomingprice / storage;


                    productPrices.Add(new ProductPrice
                    {
                        ProductId = item.productid,
                        Price = unitcost
                    });

                    //Get productid 
                    //var product = service.Get(productId);

                    var product = service.Get(productId);
                    if (product != null)
                    {
                        productDetails.Add(new ProductDetails

                        {
                            ProductId = item.productid,
                            Productname = product.Name,
                            Productdescription = product.Description,

                        });

                    }


                }
                else
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

                    if (product != null)
                    {
                        productDetails.Add(new ProductDetails

                        {
                            ProductId = item.productid,
                            Productname = product.Name,
                            Productdescription = product.Description,

                        });

                    }



                }
            }

            var viewModel = new ShoppingCartVM
            {
                ShoppingCart = _shoppingCart,
                ShoppingCartTotal = await _shoppingCart.GetShoppingCartTotalAsync(),
                ProductPrices = productPrices, // Pass the list of product prices to the view model
                ProductDetails = productDetails,
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Index()
        {
            string userId = "";


            var orders = await _orderservice.GetOrdersByUserIdAsync(userId);
            return View(orders);
        }


        public async Task<IActionResult> ShoppingCart()

        {
            var items = _shoppingCart.GetShoppingCartItems();
            _shoppingCart.ShoppingCartItems = items;

            // Fetch product prices from Stripe for each item in the shopping cart
            var service = new ProductService();
            var priceService = new PriceService();

            var productPrices = new List<ProductPrice>();
            var productDetails = new List<ProductDetails>();

            foreach (var item in items)
            {

                string productId = item.productid;
                //if product id is ..

                //check if the productid is for cloud storage
                if (productId == "prod_PgyiINyEyBPyea")
                {

                    // Extract quantity from the shopping cart item
                    var storage = item.quanitity;

                    var options = new UpcomingInvoiceOptions
                    {
                        Customer = "cus_PJnTI05Yw7lNhG",
                        SubscriptionItems = new List<InvoiceSubscriptionItemOptions>
                                {
                                 new InvoiceSubscriptionItemOptions
                                 {
                                     Price = "price_1OrakiDprfyvhQjoLmzI7dKC",
                                      Quantity = storage,
                                  }
                                 }
                    };

                    var Invoiceservice = new InvoiceService();
                    var UpcomingPrice = Invoiceservice.Upcoming(options);
                    long totalupcomingprice = UpcomingPrice.AmountDue / 100;
                    long unitcost = totalupcomingprice / storage;


                    productPrices.Add(new ProductPrice
                    {
                        ProductId = item.productid,
                        Price = unitcost
                    });

                    //Get productid 
                    //var product = service.Get(productId);

                    var product = service.Get(productId);
                    if (product != null)
                    {
                        productDetails.Add(new ProductDetails

                        {
                            ProductId = item.productid,
                            Productname = product.Name,
                            Productdescription = product.Description,

                        });

                    }


                }
                else
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

                    if (product != null)
                    {
                        productDetails.Add(new ProductDetails

                        {
                            ProductId = item.productid,
                            Productname = product.Name,
                            Productdescription = product.Description,

                        });

                    }



                }
            }

            var viewModel = new ShoppingCartVM
            {
                ShoppingCart = _shoppingCart,
                ShoppingCartTotal = await _shoppingCart.GetShoppingCartTotalAsync(),
                ProductPrices = productPrices, // Pass the list of product prices to the view model
                ProductDetails = productDetails,
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Failed()
        {


            return View();
        }
        public async Task<IActionResult> Complete()
        {

            var items = _shoppingCart.GetShoppingCartItems();
            //string userId = "test";
            //string userEmailAddress = "";

            ////await _subscriptionservice.CreateSubscriptionsAsync(userId, items);


            ////await _orderservice.StoreOrderAsync(items, userId, userEmailAddress);
            await _shoppingCart.ClearShoppingCartAsync();

            return View();
        }

        //[HttpGet("/Order/Complete")]

        //public ActionResult Complete([FromQuery] string session_id)
        //{
        //    var sessionService = new SessionService();
        //    Session session = sessionService.Get(session_id);

        //    var customerService = new CustomerService();
        //    Customer customer = customerService.Get(session.CustomerId);

        //    return Content($"<html><body><h1>Thanks for your order, {customer.Name}!</h1></body></html>");
        //}
        //public async Task<IActionResult> Complete()
        //{

        //    //var items = _shoppingCart.GetShoppingCartItems();
        //    //string userId = "test";
        //    //string userEmailAddress = "";

        //    ////await _subscriptionservice.CreateSubscriptionsAsync(userId, items);


        //    ////await _orderservice.StoreOrderAsync(items, userId, userEmailAddress);
        //    //await _shoppingCart.ClearShoppingCartAsync();

        //    return View();
        //}

        //[HttpGet("/Order/CompleteOrder")]
        //public ActionResult OrderSuccess([FromQuery] string session_id)
        //{
        //    var sessionService = new SessionService();
        //    Session session = sessionService.Get(session_id);

        //    var customerService = new CustomerService();
        //    Customer customer = customerService.Get(session.CustomerId);

        //    return Content($"<html><body><h1>Thanks for your order, {customer.Name}!</h1></body></html>");
        //}
        //[Route("/Order/CompleteOrder")]
        //public ActionResult OrderSuccess([FromQuery] string session_id)
        //{
        //    var sessionService = new SessionService();
        //    Session session = sessionService.Get(session_id);

        //    var customerService = new CustomerService();
        //    Customer customer = customerService.Get(session.CustomerId);

        //    // Pass customer name as model to the view
        //    return View(customer.Name);
        //}

        //public async Task<IActionResult> CompleteOrder()
        //{
        //    return View("OrderCompleted");
        //}

        public async Task<IActionResult> RemoveItemFromShoppingCart(string id)
        {
            var item = id;

            if (item != null)
            {
                _shoppingCart.RemoveItemFromCart(item);
            }
            return RedirectToAction(nameof(ShoppingCart));
        }


        public async Task<IActionResult> AddItemToShoppingCart(string id)
        {
            _shoppingCart.AddItemToCart(id);

            return RedirectToAction(nameof(ShoppingCart));
        }

        [HttpPost]

        public async Task<IActionResult> AddItemsToShoppingCart(int storagequantity, int devicequantity, string productid)
        {

            _shoppingCart.AddItemsToCart(storagequantity, devicequantity, productid);

            return Json(new { success = true });

            //return RedirectToAction(nameof(ShoppingCart));
        }



        [Authorize]
        public async Task<ActionResult> checkout()
        {
            /// call DPO Method 
            /// DPOIntegration
            /// 

        

            var stpcustomerid = _userService.GetSTPCustomerId();
            // Fetch products and quantities from the shopping cart
            var items = _shoppingCart.GetShoppingCartItems();
            var domain = "https://localhost:7128/Order";
            var lineItems = new List<SessionLineItemOptions>();

            var productService = new ProductService();

            foreach (var item in items)
            {
                // Retrieve the product associated with the product ID
                var product = productService.Get(item.productid);

                // Ensure that the product is not null and it has a price ID
                if (product != null && product.DefaultPriceId != null)
                {
                    // Add the item to the line items with its quantity and price ID
                    lineItems.Add(new SessionLineItemOptions
                    {
                        Price = product.DefaultPriceId,
                        Quantity = item.quanitity,
                    });
                }

            }

            var options = new SessionCreateOptions
            {
                Customer = stpcustomerid,
                LineItems = lineItems,
                Mode = "subscription",
                SuccessUrl = domain + "/Complete",

                CancelUrl = domain + "/Failed",
            };
            //var service = new SessionService();
            //Session session = service.Create(options);

            //Response.Headers.Add("Location", session.Url);
            //return new StatusCodeResult(303);

            // Use Dependency Injection to inject
            //
            // SessionService
            var service = new SessionService();
            try
            {
                Session session = service.Create(options);
                return Redirect(session.Url); // Redirect user to Stripe checkout
            }
            catch (StripeException ex)
            {
                // Handle Stripe API errors
                // You can log or display an error message to the user
                return View("Error");
            }
            catch (Exception ex)
            {
                // Handle other unexpected errors
                // You can log or display an error message to the user
                return View("Error");
            }
        }


       







    
    }
    }


 

