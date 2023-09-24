using System;
using System.Text.Json.Serialization;

namespace Shared.Mailchimp.Model.Entity.Supporting
{
	public class ContactSummary
	{
		[JsonRequired]
		public string Company { get; set; }

        [JsonRequired]
        public string Address1 { get; set; }
		public string? Address2 { get; set; } = null;

        [JsonRequired]
        public string City { get; set; }
		public string? State { get; set; } = null;
		public string? Zip { get; set; } = null;

        [JsonRequired]
        public string Country { get; set; }
		public string? Phone { get; set; } = null;
	}
}

