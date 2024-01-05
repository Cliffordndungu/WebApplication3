using Microsoft.AspNetCore.Mvc;
using Stripe;
using WebApplication3.Models;

namespace WebApplication3.Data.Services
{
    public interface ISubscriptionsService
    {
        Task CreateSubscriptionsAsync(string customerId, List<ShoppingCartItem> items);

        Task<PaymentIntent> CreatePaymentintent();

        //Customer CreateCustomer(string name, string email, string paymentMethodId);

        //void UpdateCustomer(string customerId, string paymentMethodId);




    }
}
