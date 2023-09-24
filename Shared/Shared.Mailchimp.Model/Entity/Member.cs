using System;
using Shared.Mailchimp.Model.Enum;
using Shared.Mailchimp.Model.Entity.Supporting;
using System.Text.Json.Serialization;
using Shared.Common.Attributes;

namespace Shared.Mailchimp.Model.Entity
{
	[Endpoint("/members")]
	public class Member<T> where T: BaseMergeFields
	{
		public string Id { get; set; }

		[JsonPropertyName("email_address")]
		public string EmailAddress { get; set; }

        [JsonPropertyName("unique_email_id")]
		public string UniqueEmailId { get; set; }

        [JsonPropertyName("contact_id")]
		public string ContactId { get; set; }

        [JsonPropertyName("full_name")]
		public string FullName { get; set; }

        [JsonPropertyName("web_id")]
		public int WebId { get; set; }

        [JsonPropertyName("email_type")]
		public string EmailType { get; set; }
		public MemberStatus Status { get; set; }

        [JsonPropertyName("unsubscribe_reason")]
		public string UnsubscribeReason { get; set; }

        [JsonPropertyName("consents_to_one_to_one_messaging")]
		public bool ConsentsToOneOnOneMessaging { get; set; }

        [JsonPropertyName("merge_fields")]
		public T MergeFields { get; set; }
		public object Interests { get; set; }
		public MemberStats Stats { get; set; }

        [JsonPropertyName("email_client")]
        public string EmailClient { get; set; }

        [JsonPropertyName("marketing_permissions")]
		public MarketingPermissions MarketingPermissions { get; set; }

		[JsonPropertyName("_links")]
		public List<Link> Links { get; set; } 
    }
}

