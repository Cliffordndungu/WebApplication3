using Microsoft.Identity.Client;

namespace WebApplication3.Models
{
    public class StripeEventRequest
    {
        public string id { get; set; }
        public string type { get; set; }
        public StripeEventData data { get; set; }
    }

    public class StripeEventData
    {
        public StripeEventObject @object { get; set; }
    }

    public class StripeEventObject
    {
        public string customer { get; set; }
        public string id { get; set; }
    }

   




}