using System;
using System.Text.Json.Serialization;
using Shared.Common.Attributes;
using Shared.Mailchimp.Model.Entity.Request.Supporting;
using Shared.Mailchimp.Model.Enum;

namespace Shared.Mailchimp.Model.Entity.Request
{
	[Endpoint("/3.0/merge-fields")]
	public class NewMergeFieldRequest
	{
		[JsonRequired]
		public string Name { get; set; }

        [JsonRequired]
        public DataType Type { get; set; }
        public string? Tag { get; set; } = null;
		public bool? Required { get; set; } = null;

		[JsonPropertyName("default_value")]
		public string? DefaultValue { get; set; } = null;
		public bool? Public { get; set; } = null;

		[JsonPropertyName("display_order")]
		public int? DisplayOrder { get; set; } = null;
		public MergeFieldOptions? Options { get; set; } = null;

		[JsonPropertyName("help_text")]
		public string? HelpText { get; set; } = null;
	}
}

