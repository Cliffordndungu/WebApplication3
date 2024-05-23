
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication3.Data.Cart;

namespace WebApplication3.Data.ViewModels
{
    public class ShoppingCartVM
    {
        public ShoppingCart ShoppingCart { get; set; }
        public double ShoppingCartTotal { get; set; }

        // Add a property to hold product prices
        public List<ProductPrice> ProductPrices { get; set; }

        public List<ProductDetails> ProductDetails { get; set; }

    }
}
