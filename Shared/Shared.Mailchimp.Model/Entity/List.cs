using System;
using System.Text.Json.Serialization;
using Shared.Common.Attributes;
using Shared.Mailchimp.Model.Entity.Supporting;

namespace Shared.Mailchimp.Model.Entity
{
	[Endpoint("/3.0/lists")]
	public class List
	{
		public string Id { get; set; }

		[JsonPropertyName("web_id")]
		public int WebId { get; set; }

		public string Name { get; set; }

		public ContactSummary Contact { get; set; }

		[JsonPropertyName("permission_reminder")]
		public string PermissionReminder { get; set; }

        [JsonPropertyName("use_archive_bar")]
        public bool UseArchiveBar { get; set; }

		[JsonPropertyName("campaign_defaults")]
		public CampaignDefaults CampaignDefaults { get; set; }

		[JsonPropertyName("notify_on_subscribe")]
		public string NotifyOnSubscribe { get; set; }

		[JsonPropertyName("notify_on_unsubscribe")]
		public string NotifyOnUnsubscribe { get; set; }

		[JsonPropertyName("list_rating")]
		public int ListRating { get; set; }

		[JsonPropertyName("email_type_option")]
		public bool EmailTypeOption { get; set; }

        [JsonPropertyName("subscribe_url_short")]
        public string SubscribeUrlShort { get; set; }

        [JsonPropertyName("beamer_address")]
        public string BeamerAddress { get; set; }

		public string Visibility { get; set; }

        [JsonPropertyName("double_optin")]
        public bool DoubleOptin { get; set; }

        [JsonPropertyName("marketing_permissions")]
        public bool MarketingPermissions { get; set; }

        [JsonPropertyName("has_welcome")]
        public bool HasWelcome { get; set; }

		public List<string> Modules { get; set; }

		public AudienceStats Stats { get; set; }

		[JsonPropertyName("_links")]
		public List<Link> Links { get; set; }

        [JsonPropertyName("date_created")]
        public string DateCreated { get; set; }
    }
}

