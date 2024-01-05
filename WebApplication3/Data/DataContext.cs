using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplication3.Models;

namespace WebApplication3.Data
{
    public class DataContext : IdentityDbContext<ApplicationUser>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    base.OnConfiguring(optionsBuilder);
        //    optionsBuilder.UseSqlServer("Data Source=.\\SQLEXPRESS;Initial Catalog=App;User Id=decisive;Password=fatcat_88$$;TrustServerCertificate=True;");
        //}

     
        public DbSet<User> Users => Set <User>();

        //Orders related tables
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }

        // Pricing and Products from Stripe

        //public DbSet<Prices> Prices { get; set; }
        //public DbSet<Product> Products { get; set; }

    }
}
