using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication3.Models
{
    public class ShoppingCartItem
    {
        [Key]
        public string Id { get; set; }
        public string productid { get; set; }
        public int quanitity { get; set; }
        public string ShoppingCartId { get; set; }
    }
}
