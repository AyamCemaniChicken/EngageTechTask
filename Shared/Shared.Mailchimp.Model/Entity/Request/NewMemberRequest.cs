using System;
using System.Text.Json.Serialization;
using Shared.Mailchimp.Model.Entity.Supporting;
using Shared.Mailchimp.Model.Enum;

namespace Shared.Mailchimp.Model.Entity.Request
{
	public class NewMemberRequest<T> where T: BaseMergeFields
	{
        [JsonRequired]
        [JsonPropertyName("email_address")]
        public string EmailAddress { get; set; }

        [JsonRequired]
        public MemberStatus Status { get; set; }

        [JsonPropertyName("merge_fields")]
        public T? MergeFields { get; set; } = null;

        [JsonPropertyName("marketing_permissions")]
        public MarketingPermissions? MarketingPermissions { get; set; } = null;

        [JsonPropertyName("timestamp_opt")]
        public string? TimestampOpt { get; set; } = null;
    }
}

