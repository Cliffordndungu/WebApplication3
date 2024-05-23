using System.ComponentModel.DataAnnotations;

namespace WebApplication3.Models
{
    public class Index_Products
    {
       
            [Key]
            public string id { get; set; }

            public string productid { get; set; }
            public decimal price { get; set; }
            public string Name { get; set; }

            public string Description { get; set; }

        public string[] FeatureList { get; set; }


    }
}
