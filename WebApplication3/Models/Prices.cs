using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication3.Models
{
    public class Prices
    {
        public string id { get; set; }
        public decimal unitamount { get; set; }

        public string productid { get; set; }
    
        
    }
}


