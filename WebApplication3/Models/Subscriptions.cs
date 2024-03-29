﻿using System.Diagnostics.Eventing.Reader;

namespace WebApplication3.Models
{
    public class Subscriptions
    {
        public string id { get; set; }
        public string product { get; set; }
        public string priceid { get; set; }
        public string interval { get; set; }
        public long? quantity { get; set; }

        public long? unitamount { get; set; }

        public string? subitemid { get; set; }
        public string? status { get; set; }
        public string? name { get; set; }

        public DateTime? startdate { get; set; }

        public string? assigned { get; set; }
        public DateTime? renewaldate { get; set; }
        public DateTime? trialend { get; set; }

    }
}
