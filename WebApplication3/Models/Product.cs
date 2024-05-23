using System.ComponentModel.DataAnnotations;

namespace WebApplication3.Models
{
    public class Product
    {
        [Key]
        public string id { get; set; }

        public string productid { get; set; }
        public decimal price { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; }

        public string featureset { get; set; }

        public string featurelist { get; set; }

        public string compatibility { get; set; }
    }
}
