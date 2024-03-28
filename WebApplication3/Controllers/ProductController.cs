using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Stripe;
using WebApplication3.Data.Cart;
using WebApplication3.Data.ViewModels;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    public class ProductController : Controller
    {
        private readonly ShoppingCart _shoppingCart;

        public ProductController(ShoppingCart shoppingCart)
        {
            _shoppingCart = shoppingCart;
        }
        public async Task<IActionResult> Index()
        {
            List<Products> products = new List<Products>();
            var options = new ProductListOptions
            {
                Active = true,
            };
            var service = new ProductService();
            StripeList<Stripe.Product> productList = service.List(options);

            foreach (var stripeProduct in productList)
            {
                Products product = new Products
                {
                    id = stripeProduct.Id,
                    Name = stripeProduct.Name,
                    Description = stripeProduct.Description,
                    Price = stripeProduct.Metadata.ContainsKey("price") ? Convert.ToDecimal(stripeProduct.Metadata["price"]) : 0,
                    //ImageUrl = stripeProduct.Images.FirstOrDefault()
                    engine = stripeProduct.Metadata.ContainsKey("Engine") ? stripeProduct.Metadata["Engine"] : null,
                    compatibility = stripeProduct.Metadata.ContainsKey("Comp") ? stripeProduct.Metadata["Comp"] : null,
                    subscription = stripeProduct.Metadata.ContainsKey("Subscription") ? stripeProduct.Metadata["Subscription"] : null,
                    productcode = stripeProduct.Metadata.ContainsKey("colorcode") ? stripeProduct.Metadata["colorcode"] : null,
                    datacenterlocation = stripeProduct.Metadata.ContainsKey("Datacenterlocation") ? stripeProduct.Metadata["Datacenterlocation"] : null,
                };

                products.Add(product);
            }
            // Remove the product with the specified ID
            products.RemoveAll(p => p.id == "prod_PgyiINyEyBPyea");

            return View(products);
        }

        public async Task<IActionResult> Details(string id)
        {
            var items = _shoppingCart.GetShoppingCartItems();
            _shoppingCart.ShoppingCartItems = items;
      

            bool productExists = items.Any(item => item.productid == id);
            ViewBag.CartStatus = productExists ? new CloudStorageCartStatus { cloudstoragecart = false, cloudstoragesuscribed = false } : new CloudStorageCartStatus { cloudstoragecart = false, cloudstoragesuscribed = false };

            var productService = new ProductService();
                var stripeProduct = productService.Get(id);
           

            if (stripeProduct == null)
                {
                    // Handle the case where the product with the given ID is not found.
                    return NotFound();
                }

                var service = new PriceService();
           
            var prices =service.Get(stripeProduct.DefaultPriceId);
            var productName = stripeProduct.Name;

            Models.Product productViewModel = new Models.Product
            {
                productid = id,
                price = (decimal)prices.UnitAmount / 100,
                Name = productName, // Fetching the product name from StripePrice
                id = prices.Id,
                compatibility = stripeProduct.Metadata.ContainsKey("Compatibility") ? stripeProduct.Metadata["Compatibility"] : null,
                Description = stripeProduct.Description
            };
          

            return View(productViewModel);
            }


        }
    }


//if (productExists)
//{
//    CartStatus.Add(new CloudStorageCartStatus

//    {
//        cloudstoragecart = true,
//        cloudstoragesuscribed = true

//    });



//}
//else
//{
//    CartStatus.Add(new CloudStorageCartStatus

//    {
//        cloudstoragecart = false,
//        cloudstoragesuscribed = false

//    });
//}
//var priceoptions = new PriceListOptions
//{
//    Product = id,
//};

//var prices = await service.ListAsync(priceoptions);

//foreach (var price in prices.Data)
//{
//var productViewModel = new Models.Product
//{
//    productid = id,
//    price = (decimal)price.UnitAmount / 100,
//    Name = productName, // Fetching the product name from StripePrice
//    id = price.Id,
//    compatibility = stripeProduct.Metadata.ContainsKey("Compatibility") ? stripeProduct.Metadata["Compatibility"] : null,
//    Description = stripeProduct.Description 
//};
//    productViewModels.Add(productViewModel);
//}

//add productviewmodels to productdetailsvm 

//code to check cloudstorage
//
//var viewModel = new ProductDetailsVM
//{
//    Products = productViewModels,
//    CartStatus = CartStatus

//};