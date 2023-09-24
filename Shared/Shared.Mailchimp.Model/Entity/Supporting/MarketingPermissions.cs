using System;
using System.Text.Json.Serialization;

namespace Shared.Mailchimp.Model.Entity.Supporting
{
	public class MarketingPermissions
	{
        [JsonPropertyName("marketing_permission_id")]
        public string MarketingPermissionId { get; set; }
        public string Text { get; set; }
        public bool Enabled { get; set; }
    }
}

