using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Stripe;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    public class ProductController : Controller
    {

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

            return View(products);
        }

        public async Task<IActionResult> Details(string id)
        {
            
                var productService = new ProductService();
                var stripeProduct = productService.Get(id);

                // Assuming ProductService.Get returns a StripeProduct

                if (stripeProduct == null)
                {
                    // Handle the case where the product with the given ID is not found.
                    return NotFound();
                }

                var service = new PriceService();
                var priceoptions = new PriceListOptions
                {
                    Product = id,
                };
                var prices = await service.ListAsync(priceoptions);
                var productName = stripeProduct.Name;
                var productViewModels = new List<Models.Product>();

                foreach (var price in prices.Data)
                {
                var productViewModel = new Models.Product
                {
                    productid = id,
                    price = (decimal)price.UnitAmount / 100,
                    Name = productName, // Fetching the product name from StripePrice
                    id = price.Id,
                    compatibility = stripeProduct.Metadata.ContainsKey("Compatibility") ? stripeProduct.Metadata["Compatibility"] : null,
                    Description = stripeProduct.Description 
                };
                    productViewModels.Add(productViewModel);
                }

                return View(productViewModels);
            }


        }
    }


