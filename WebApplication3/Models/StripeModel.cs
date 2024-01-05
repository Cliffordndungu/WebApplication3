using Stripe;

namespace WebApplication3.Models
{
    public class StripeModel
    {
        public class StripeSettings
        {
            public string StripePublishableKey { get; set; }
            public string StripeSecretKey { get; set; }
        }

        public class Backend
        {
            public string PriceId { get; set; }
            public int Amount { get; set; }
            public int Quantity { get; set; }
            public String Currency { get; set; }
            public String Description { get; set; }
            public String StripeAccountCountry { get; set; }
        }

        public class BaseClass
        {
            public static Customer CreateCustomer(string name, string email, string paymentMethodId)
            {
                var customerOptions = new CustomerCreateOptions
                {
                    Name = name,
                    Email = email,
                };
                var customerService = new CustomerService();
                var customer = customerService.Create(customerOptions);

                UpdateCustomer(customer.Id, paymentMethodId);

                return customer;
            }
            public static void UpdateCustomer(string customerId, string paymentMethodId)
            {
                var paymentMethodAttachOptions = new PaymentMethodAttachOptions
                {
                    Customer = customerId,
                };
                var paymentMethodservice = new PaymentMethodService();
                paymentMethodservice.Attach(paymentMethodId, paymentMethodAttachOptions);

                var customerOptions = new CustomerUpdateOptions
                {
                    InvoiceSettings = new CustomerInvoiceSettingsOptions
                    {
                        DefaultPaymentMethod = paymentMethodId,
                    },
                };
                var customerService = new CustomerService();
                customerService.Update(customerId, customerOptions);
            }

            public static Subscription CreateSubscription(string priceId, string customerId, long quantity)
            {
                var subscriptionOptions = new SubscriptionCreateOptions
                {
                    Customer = customerId,
                    Items = new List<SubscriptionItemOptions>
                    {
                        new SubscriptionItemOptions
                        {
                            Price = priceId,
                            Quantity = quantity,
                        },
                    },
                };
                subscriptionOptions.AddExpand("latest_invoice.payment_intent");

                var subscriptionService = new SubscriptionService();
                Subscription subscription = subscriptionService.Create(subscriptionOptions);
                return subscription;
            }
            public static Subscription RetrieveSubscription(string subscriptionId)
            {
                var subscriptionService = new SubscriptionService();
                return subscriptionService.Get(subscriptionId);
            }


            //for your future usage
            public static Stripe.Product CreateProduct(string id, string name, string description)
            {
                var productService = new ProductService();
                var productCreateOptions = new ProductCreateOptions
                {
                    Id = id,
                    Name = name,
                    Description = description,
                };
                var product = productService.Create(productCreateOptions);

                return product;
            }
            public static Stripe.Product RetrieveProduct(string productId)
            {
                var productService = new ProductService();
                return productService.Get(productId);
            }
            public static Price CreatePrice(string productId, string currency, long unitAmount, string interval)
            {
                var product = RetrieveProduct(productId);

                var priceService = new PriceService();
                var priceCreateOptions = new PriceCreateOptions
                {
                    Currency = currency,
                    UnitAmount = unitAmount,
                    Product = product.Id,
                    Recurring = new PriceRecurringOptions
                    {
                        Interval = interval, /*month or year*/
                    },
                };
                var price = priceService.Create(priceCreateOptions);

                return price;
            }
            public static Price RetrievePrice(string priceId)
            {
                var priceService = new PriceService();
                return priceService.Get(priceId);
            }
            public static Invoice GetInvoice(string invoiceId)
            {
                var invoiceOptions = new InvoiceGetOptions();
                invoiceOptions.AddExpand("payment_intent");
                var invoiceService = new InvoiceService();
                Invoice invoice = invoiceService.Get(invoiceId, invoiceOptions);
                return invoice;
            }
            //for your future usage
        }
    }
}
