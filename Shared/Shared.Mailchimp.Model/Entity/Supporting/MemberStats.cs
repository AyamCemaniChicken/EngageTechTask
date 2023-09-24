using System;
using System.Text.Json.Serialization;

namespace Shared.Mailchimp.Model.Entity.Supporting
{
	public class MemberStats
	{
        [JsonPropertyName("avg_open_rate")]
        public int AvgOpenRate { get; set; }

        [JsonPropertyName("avg_click_rate")]
        public int AvgClickRate { get; set; }

        [JsonPropertyName("ecommerce_data")]
        public EcommerceData EcommerceData { get; set; }
    }
}

