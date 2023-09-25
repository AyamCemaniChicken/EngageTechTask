using System;
using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace EngageTechTask.Common
{
	public class User
	{
        [BsonId]
        [BsonElement("_id")]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string UID { get; set; }
        [JsonPropertyName("id")]
		public long IntId { get; set; }
		public string UserName { get; set; }
		[JsonPropertyName("first_name")]
		public string FirstName { get; set; }
        [JsonPropertyName("last_name")]
        public string LastName { get; set; }
		public string Email { get; set; }
        [JsonPropertyName("job_title")]
        public string JobTitle { get; set; }
		public string Department { get; set; }
		public string Manager { get; set; }
        [JsonPropertyName("location_ref")]
		public string LocationRef { get; set; }
		public string Image { get; set; }
		public string Bio { get; set; }
        [JsonPropertyName("date_of_birth")]
		public string DateOfBirth { get; set; }
        [JsonPropertyName("start_date")]
		public string StartDate { get; set; }
		public string Pin { get; set; }
        [JsonPropertyName("created_at")]
		public string CreatedAt { get; set; }
		public int? Status { get; set; }
        [JsonPropertyName("updated_at")]
		public string UpdatedAt { get; set; }
		public List<Attribute>? Attributes { get; set; }
        public List<Group>? Groups { get; set; }
        [JsonPropertyName("visible_to_groups")]
        public List<Group>? VisibleToGroups { get; set; }
        [JsonPropertyName("first_name_editable")]
		public int FirstNameEditable { get; set; }
        [JsonPropertyName("last_name_editable")]
        public int LastNameEditable { get; set; }
        [JsonPropertyName("manager_visible")]
        public int ManagerVisible { get; set; }
        [JsonPropertyName("date_of_birth_visible")]
        public int DateOfBirthVisible { get; set; }
        [JsonPropertyName("date_of_birth_editable")]
        public int DateOfBirthEditable { get; set; }
        [JsonPropertyName("start_date_visible")]
        public int StartDateVisible { get; set; }
        [JsonPropertyName("start_date_editable")]
        public int StartDateEditable { get; set; }
        [JsonPropertyName("department_visible")]
        public int DepartmentVisible { get; set; }
        [JsonPropertyName("department_editable")]
        public int DepartmentEditable { get; set; }
        [JsonPropertyName("job_title_visible")]
        public int JobTitleVisible { get; set; }
        [JsonPropertyName("job_title_editable")]
        public int JobTitleEditable { get; set; }
        [JsonPropertyName("email_visible")]
        public int EmailVisible { get; set; }
        [JsonPropertyName("email_editable")]
        public int EmailEditable { get; set; }
        [JsonPropertyName("bio_visible")]
        public int BioVisible { get; set; }
        [JsonPropertyName("bio_editable")]
        public int BioEditable { get; set; }
        public bool Processed { get; set; }
    }

    public class Attribute
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
        [JsonPropertyName("is_visible")]
        public int IsVisible { get; set; }
        [JsonPropertyName("is_editable")]
        public int IsEditable { get; set; }
        [JsonPropertyName("user_managed")]
        public int UserManaged { get; set; }
        public string Icon { get; set; }
        [JsonPropertyName("display_order")]
        public int DisplayOrder { get; set; }
        public string Template { get; set; }
        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; }
        [JsonPropertyName("updated_at")]
        public string UpdatedAt { get; set; }
    }

    public class Group
    {
        public int Id { get; set; }
        public string Reference { get; set; }
        public string Title { get; set; }
    }
}

