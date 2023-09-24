using System;
using System.Text.Json.Serialization;

namespace Shared.Mailchimp.Model.Entity.Request.Supporting
{
	public class MergeFieldOptions
	{
        [JsonPropertyName("default_country")]
        public int? DefaultCountry { get; set; } = null;

        [JsonPropertyName("phone_format")]
        public string? PhoneFormat { get; set; } = null;

        [JsonPropertyName("date_format")]
        public string? DateFormat { get; set; } = null;
        public List<string>? Choices { get; set; } = null;
        public int? Size { get; set; } = null;
    }
}

