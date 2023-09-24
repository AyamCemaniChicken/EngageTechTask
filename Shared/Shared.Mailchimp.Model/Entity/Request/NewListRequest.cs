using System;
using Shared.Mailchimp.Model.Entity.Supporting;
using System.Text.Json.Serialization;
using Shared.Common.Attributes;

namespace Shared.Mailchimp.Model.Entity.Request
{
    [Endpoint("/3.0/lists")]
	public class NewListRequest
	{
        [JsonRequired]
        public string Name { get; set; }

        [JsonRequired]
        public ContactSummary Contact { get; set; }

        [JsonRequired]
        [JsonPropertyName("permission_reminder")]
        public string PermissionReminder { get; set; }

        [JsonRequired]
        [JsonPropertyName("campaign_defaults")]
        public CampaignDefaults CampaignDefaults { get; set; }

        [JsonRequired]
        [JsonPropertyName("email_type_option")]
        public bool EmailTypeOption { get; set; } = true;

        [JsonPropertyName("notify_on_subscribe")]
        public string? NotifyOnSubscribe { get; set; } = null;

        [JsonPropertyName("notify_on_unsubscribe")]
        public string? NotifyOnUnsubscribe { get; set; } = null;

        [JsonPropertyName("double_optin")]
        public bool? DoubleOptin { get; set; } = null;

        [JsonPropertyName("marketing_permissions")]
        public bool? MarketingPermissions { get; set; } = null;

        [JsonPropertyName("use_archive_bar")]
        public bool? UseArchiveBar { get; set; } = null;
    }
}

