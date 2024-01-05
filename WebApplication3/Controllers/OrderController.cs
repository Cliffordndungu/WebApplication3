using Microsoft.AspNetCore.Mvc;
using Stripe;
using System.Security.Claims;
using WebApplication3.Data.Cart;
using WebApplication3.Data.Services;
using WebApplication3.Data.ViewModels;


namespace WebApplication3.Controllers
{
    public class OrderController : Controller
    {
        private readonly ShoppingCart _shoppingCart;
        private readonly OrdersService _orderservice;
        private readonly SubscriptionsService _subscriptionservice;
        public OrderController(ShoppingCart shoppingCart, OrdersService orderService, SubscriptionsService subscriptionsService)
        {
            _shoppingCart = shoppingCart;
            _orderservice = orderService;
            _subscriptionservice = subscriptionsService;
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
                    ShoppingCart = _shoppingCart,
                    ShoppingCartTotal = await _shoppingCart.GetShoppingCartTotalAsync(),
                    ProductPrices = productPrices  // Pass the list of product prices to the view model
                };

                return View(viewModel);
            }


        //public async Task<IActionResult> CompleteOrder()
        //{




        //    //var items = _shoppingCart.GetShoppingCartItems();
        //    //string userId = "test";
        //    //string userEmailAddress = "";

        //    //await _subscriptionservice.CreateSubscriptionsAsync(userId, items);
        //    ////await _orderservice.StoreOrderAsync(items, userId, userEmailAddress);
        //    ////await _shoppingCart.ClearShoppingCartAsync();

        //    //return View("OrderCompleted");
        //}

        public async Task<IActionResult> CompleteOrder()
        {
            return View("OrderCompleted");
        }

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
    }
}
