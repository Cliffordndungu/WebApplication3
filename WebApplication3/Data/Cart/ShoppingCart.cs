
using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client.Extensions.Msal;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Transactions;
using WebApplication3.Migrations;
using WebApplication3.Models;

namespace WebApplication3.Data.Cart
{
    public class ShoppingCart
    {
        public DataContext _context { get; set; }

        public string ShoppingCartId { get; set; }
        public List<ShoppingCartItem> ShoppingCartItems { get; set; }

        public ShoppingCart(DataContext context)
        {
            _context = context;
        }

        public List<ShoppingCartItem> GetShoppingCartItems()
        {
            return ShoppingCartItems ?? (ShoppingCartItems = _context.ShoppingCartItems.Where(n => n.ShoppingCartId == ShoppingCartId).ToList());
        }

        public async Task<string> GetPriceid(string productid)
        {
            try
            {
                // Fetch the Stripe Product by ID
                var productService = new ProductService();
                var stripeProduct = productService.Get(productid);

                if (stripeProduct == null)
                {
                    // Handle the case where the product with the given ID is not found.
                    throw new Exception("Product not found");
                }

                // Fetch the prices associated with the product
                var service = new PriceService();
                var priceoptions = new PriceListOptions
                {
                    Product = productid,
                };
                var prices = await service.ListAsync(priceoptions);

                if (prices.Data.Count == 0)
                {
                    // Handle the case where there are no prices for the product.
                    throw new Exception("No prices found for the product");
                }

                // For simplicity, you can assume the first price in the list is the current price.
                var currentPrice = prices.Data[0].Id;

                // You can return the price amount as a decimal.
                return currentPrice; // Convert from cents to dollars
            }
            catch (Exception ex)
            {
                // Handle any errors or exceptions here, e.g., log them or return a default price.
                // You can also customize the error handling based on your application's needs.
                Console.WriteLine($"Error fetching product price: {ex.Message}");
                var exceptionstring = "fakedid";
                return exceptionstring; // Return a default price or handle the error as needed
            }

        }

        public async Task<decimal> GetProductPrice(string productid)
        {
           try
                {
                // Fetch the Stripe Product by ID
                var productService = new ProductService();
                var stripeProduct = productService.Get(productid);

                if (stripeProduct == null)
                    {
                        // Handle the case where the product with the given ID is not found.
                        throw new Exception("Product not found");
                    }

                // Fetch the prices associated with the product
                var service = new PriceService();
                var priceoptions = new PriceListOptions
                {
                    Product = productid,
                };
                var prices = await service.ListAsync(priceoptions);

                if (prices.Data.Count == 0)
                    {
                        // Handle the case where there are no prices for the product.
                        throw new Exception("No prices found for the product");
                    }

                    // For simplicity, you can assume the first price in the list is the current price.
                 var currentPrice = prices.Data[0];

                    // You can return the price amount as a decimal.
                 return (decimal)currentPrice.UnitAmount / 100; // Convert from cents to dollars
                }
                catch (Exception ex)
                {
                    // Handle any errors or exceptions here, e.g., log them or return a default price.
                    // You can also customize the error handling based on your application's needs.
                    Console.WriteLine($"Error fetching product price: {ex.Message}");
                    return 0.00m; // Return a default price or handle the error as needed
                }
            
        }

    public async Task<double> GetShoppingCartTotalAsync()
        {
            // 1. Fetch prices from Stripe based on product IDs in the shopping cart
            var shoppingCartItems = _context.ShoppingCartItems.Where(n => n.ShoppingCartId == ShoppingCartId).ToList();
            var productIds = shoppingCartItems.Select(item => item.productid).ToList();

            var productService = new ProductService();
            var priceService = new PriceService();

            var productPrices = new Dictionary<string, long>();

            foreach (var productId in productIds)
            {
                var stripeProduct = productService.Get(productId);

                if (stripeProduct != null)
                {
                    //check if the productid is for cloud storage
                    if (productId == "prod_PgyiINyEyBPyea")
                    {
                        // Find the corresponding shopping cart item
                        var cartItem = shoppingCartItems.FirstOrDefault(item => item.productid == productId);
                        if (cartItem != null)
                        {
                            // Extract quantity from the shopping cart item
                            var storage = cartItem.quanitity;

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



                            var service = new InvoiceService();
                            var UpcomingPrice = service.Upcoming(options);
                            long totalupcomingprice = UpcomingPrice.AmountDue / storage;

                            productPrices[productId] = totalupcomingprice;
                        }
                    }
                    else
                    {

                        //if for cloud storage 
                        //productprices[productid] = get the pricing for the item 
                        //else do below. 

                        var priceListOptions = new PriceListOptions
                        {
                            Product = productId,
                        };

                        var prices = await priceService.ListAsync(priceListOptions);

                        if (prices.Data.Count > 0)
                        {
                            // Assuming you want to use the first price in the list
                            var price = prices.Data.First();
                            productPrices[productId] = (long)price.UnitAmount;
                        }
                    }
                   
                }
            }

            // 2. Calculate subtotals for each item in the shopping cart
            double subtotal = 0.0;

            foreach (var item in shoppingCartItems)
            {
                if (productPrices.TryGetValue(item.productid, out long price))
                {
                    subtotal += (price / 100.0) * item.quanitity; // Stripe prices are in cents
                }
            }

            return subtotal;
        }

        //public async Task<string> GetProductDetails()
        //{
        //    var shoppingCartItems = _context.ShoppingCartItems.Where(n => n.ShoppingCartId == ShoppingCartId).ToList();
        //    var productIds = shoppingCartItems.Select(item => item.productid).ToList();

           
        //    var productDetails = new List<(string Name, string Description)>();
        //    var service = new ProductService();
        //    // Fetch product details from Stripe
        //    foreach (var productId in productIds)
        //    {
               
                
        //        var product = service.Get(productId);
        //        if (product != null)
        //        {
        //            productDetails.Add((product.Name, product.Description));
        //        }
        //    }

        //    // Now you have product details, you can return it
        //    return productDetails;
        //}
    

        public static ShoppingCart GetShoppingCart(IServiceProvider services)
        {
            ISession session = services.GetRequiredService<IHttpContextAccessor>()?.HttpContext.Session;
            var context = services.GetService<DataContext>();

            string cartId = session.GetString("CartId") ?? Guid.NewGuid().ToString();
            session.SetString("CartId", cartId);

            return new ShoppingCart(context) { ShoppingCartId = cartId };
        }
        public void RemoveItemFromCart(string productid)
        {
            var shoppingCartItem = _context.ShoppingCartItems.FirstOrDefault(n => n.productid == productid && n.ShoppingCartId == ShoppingCartId);

            if (shoppingCartItem != null)
            {
                if (shoppingCartItem.quanitity > 1)
                {
                    shoppingCartItem.quanitity--;
                }
                else
                {
                    _context.ShoppingCartItems.Remove(shoppingCartItem);
                }
            }
            _context.SaveChanges();
        }
        public async void AddItemToCart(string productid)
        {
            var shoppingCartItem = _context.ShoppingCartItems.FirstOrDefault(n => n.productid == productid && n.ShoppingCartId == ShoppingCartId);
            // get price id from product id then pass it pass it as product id into shopping cart items. 
           


            if(shoppingCartItem == null)
            {
                shoppingCartItem = new ShoppingCartItem()
                {
                    Id = Guid.NewGuid().ToString(),
                    ShoppingCartId = ShoppingCartId,
                    productid = productid,
                    quanitity = 1

                    //quanitity = quantity
                };

                _context.ShoppingCartItems.Add(shoppingCartItem);
            } else
            {
                shoppingCartItem.quanitity++;
            }
            _context.SaveChanges();
        }

        public async void AddItemsToCart(int storagequantity, int devicequantity,  string productid)
        {
            var shoppingCartItem = _context.ShoppingCartItems.FirstOrDefault(n => n.productid == productid && n.ShoppingCartId == ShoppingCartId);
            // get price id from product id then pass it pass it as product id into shopping cart items. 
            string storageproductid = "prod_PgyiINyEyBPyea";

            if (shoppingCartItem == null)
            {
               var shoppingCartItem1 = new ShoppingCartItem()
                {
                    Id = Guid.NewGuid().ToString(),
                    ShoppingCartId = ShoppingCartId,
                    productid = productid,
                    quanitity = devicequantity

                    //quanitity = quantity
                };

                _context.ShoppingCartItems.Add(shoppingCartItem1);

                if (storagequantity > 0)
                {
               var  shoppingCartItem2 = new ShoppingCartItem()
                {
                    Id = Guid.NewGuid().ToString(),
                    ShoppingCartId = ShoppingCartId,
                    productid = storageproductid,
                    quanitity = storagequantity

                    //quanitity = quantity
                };


                _context.ShoppingCartItems.Add(shoppingCartItem2);
                }
            }
            else
            {
                shoppingCartItem.quanitity++;
            }
            _context.SaveChanges();
        }

        public async Task ClearShoppingCartAsync()
        {
            var items = await _context.ShoppingCartItems.Where(n => n.ShoppingCartId == ShoppingCartId).ToListAsync();
            _context.ShoppingCartItems.RemoveRange(items);
            await _context.SaveChangesAsync();
        }
    }
}
