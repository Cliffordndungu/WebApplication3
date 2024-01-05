using Microsoft.EntityFrameworkCore;
using WebApplication3.Models;

namespace WebApplication3.Data.Services
{
    public class OrdersService : IOrdersService
    {
        private readonly DataContext _context;
        public OrdersService(DataContext context)
        {
            _context = context;
        }

        public async Task<List<Order>> GetOrdersByUserIdAsync(string userId)
        {
            var orders = await _context.Orders.Include(n => n.OrderItems).Where(n => n.UserId == userId).ToListAsync();
            return orders;
        }

        public async Task StoreOrderAsync(List<ShoppingCartItem> items, string userId, string userEmailAddress)
        {
            var order = new Order()
            {
                UserId = userId,
                Email = userEmailAddress
            };
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            foreach (var item in items)
            {
                var orderItem = new OrderItem()
                {
                    Amount = item.quanitity,
                    OrderId = order.Id,
                    ProductId = item.productid

                };
                await _context.OrderItems.AddAsync(orderItem);

            };


            await _context.SaveChangesAsync();
        }
    }
}
