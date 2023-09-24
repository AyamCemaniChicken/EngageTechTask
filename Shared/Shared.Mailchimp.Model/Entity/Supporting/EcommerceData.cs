using System;
using System.Text.Json.Serialization;

namespace Shared.Mailchimp.Model.Entity.Supporting
{
	public class EcommerceData
	{
        [JsonPropertyName("total_revenue")]
        public int TotalRevenue { get; set; }

        [JsonPropertyName("number_of_orders")]
        public int NumberOfOrders { get; set; }

        [JsonPropertyName("currency_code")]
        public string CurrencyCode { get; set; }
    }
}

