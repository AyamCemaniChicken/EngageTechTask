using System;
using System.Text.Json.Serialization;

namespace Shared.Mailchimp.Model.Entity.Supporting
{
	public class CampaignDefaults
	{
		[JsonRequired]
        [JsonPropertyName("from_name")]
        public string FromName { get; set; }

        [JsonRequired]
        [JsonPropertyName("from_email")]
		public string FromEmail { get; set; }

        public string Subject { get; set; } = "";

        public string Language { get; set; } = "en";
	}
}

