using WebApplication3.Models;

namespace WebApplication3.Data.ViewModels
{
    public class ProductDetailsVM
    {
        public List<Product> Products { get; set; }
        public List<CloudStorageCartStatus> CartStatus { get; set; }
    }
}
