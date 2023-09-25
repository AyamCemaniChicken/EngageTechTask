using System;
using EngageTechTask.Fetch.Enum;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EngageTechTask.Common
{
	public class Configuration
	{
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public List<WebSource> WebSources { get; set; }
        public DateTime Created = DateTime.UtcNow.ToLocalTime();
        public TimeOnly StartTime { get; set; } = new TimeOnly(DateTime.UtcNow.ToLocalTime().Hour, DateTime.UtcNow.ToLocalTime().Minute, DateTime.UtcNow.ToLocalTime().Second);
        [BsonRepresentation(BsonType.String)]
        public IntervalUnit IntervalUnit { get; set; } = IntervalUnit.Day;
        public int Interval { get; set; } = 1;
        public WebSource Endpoint { get; set; }
        public DateTime? LastRunDate { get; set; }
    }

    public class WebSource
    {
        public string Url { get; set; }
        public string AuthToken { get; set; }
        public bool AuthRequired { get; set; }
    }
}

