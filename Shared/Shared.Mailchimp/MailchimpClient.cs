using System;
using System.Text.Json;
using Shared.Common.Attributes;
using Shared.Mailchimp.Model;
using Shared.Mailchimp.Model.Entity;
using Shared.Mailchimp.Model.Entity.Request;
using Shared.Mailchimp.Model.Entity.Response;

namespace Shared.Mailchimp
{
    public class MailchimpClient
    {
        private string apiKey;
        private static HttpClient client;
        private static string TargetAudience { get; set; }

        private static readonly JsonSerializerOptions JsonOpts = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public MailchimpClient(string ApiKey)
        {
            apiKey = ApiKey;

            string dataCenter = apiKey.Split("-")[1];
            client = new HttpClient()
            {
                BaseAddress = new Uri($"https://{dataCenter}.api.mailchimp.com"),
            };

            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
        }

        public async Task CreateMergeFields(List<NewMergeFieldRequest> MergeFields, string ListId)
        {
            TargetAudience = ListId;
            var EndpointAttribute = (EndpointAttribute?)Attribute.GetCustomAttribute(typeof(NewMergeFieldRequest), typeof(EndpointAttribute));

            if (EndpointAttribute == null)
                return;

            var List = await GetList(ListId);

            if (List == null)
                return;

            var Uri = EndpointAttribute.Url;

            foreach (var MergeField in MergeFields)
            {
                var jsonString = JsonSerializer.Serialize(MergeField);
                var RequestContent = new StringContent(jsonString, new System.Net.Http.Headers.MediaTypeHeaderValue("application/json"));

                var Res = await client.PostAsync($"/{List.Id}{Uri}", RequestContent);

                if (Res.StatusCode == System.Net.HttpStatusCode.BadRequest || Res.StatusCode == System.Net.HttpStatusCode.Conflict)
                    Console.ForegroundColor = ConsoleColor.Red;
                else if (Res.IsSuccessStatusCode || Res.StatusCode == System.Net.HttpStatusCode.NoContent)
                    Console.ForegroundColor = ConsoleColor.Green;
                else
                    Console.ForegroundColor = ConsoleColor.Yellow;

                Console.WriteLine($"Status: {Res.StatusCode}\n\rMessage:\n\r{await Res.Content.ReadAsStringAsync()}");
            }
        }

        public async Task<Model.Entity.List> GetListById(string ListId)
        {
            var EndpointAttribute = (EndpointAttribute?)Attribute.GetCustomAttribute(typeof(Model.Entity.List), typeof(EndpointAttribute));

            if (EndpointAttribute == null)
                return null;

            var Uri = $"{EndpointAttribute.Url}/{ListId}";

            var Res = await client.GetAsync(Uri);

            Res.EnsureSuccessStatusCode();

            var List = JsonSerializer.Deserialize<Model.Entity.List>(await Res.Content.ReadAsStringAsync(), JsonOpts);

            return List;
        }

        public async Task<Model.Entity.List> GetList(string ListName)
        {
            var EndpointAttribute = (EndpointAttribute?)Attribute.GetCustomAttribute(typeof(Model.Entity.List), typeof(EndpointAttribute));

            if (EndpointAttribute == null)
                return null;

            var Uri = $"{EndpointAttribute.Url}";

            var Res = await client.GetAsync(Uri);

            Res.EnsureSuccessStatusCode();

            var resBody = await Res.Content.ReadAsStringAsync();

            var Lists = JsonSerializer.Deserialize<ListsResponse>(resBody, JsonOpts);

            var List = Lists.Lists.Where(list => list.Name == ListName);

            if (List.Count() < 1)
                return null;

            return List.FirstOrDefault();
        }

        public async Task<Model.Entity.List> CreateList(NewListRequest NewList)
        {
            var existingList = await GetList(NewList.Name);

            if (existingList != null)
                return existingList;

            var EndpointAttribute = (EndpointAttribute?)Attribute.GetCustomAttribute(typeof(NewListRequest), typeof(EndpointAttribute));

            if (EndpointAttribute == null)
                return null;

            var Uri = EndpointAttribute.Url;

            var JsonString = JsonSerializer.Serialize(NewList, JsonOpts);
            var RequestContent = new StringContent(JsonString, new System.Net.Http.Headers.MediaTypeHeaderValue("application/json"));

            var Res = await client.PostAsync(Uri, RequestContent);
            var resBody = await Res.Content.ReadAsStringAsync();

            Res.EnsureSuccessStatusCode();
           
            var List = JsonSerializer.Deserialize<Model.Entity.List>(await Res.Content.ReadAsStringAsync());

            return List;
        }

        public async Task AddMembers<T>(List<NewMemberRequest<T>> NewMembers) where T: BaseMergeFields
        {
            foreach (var NewMember in NewMembers)
            {
                await AddMember(NewMember);
            }
        }
        
        public async Task AddMember<T>(NewMemberRequest<T> NewMember) where T : BaseMergeFields
        {
            var EndpointAttribute = (EndpointAttribute)Attribute.GetCustomAttribute(typeof(Member<T>), typeof(EndpointAttribute));

            if (EndpointAttribute == null)
                return;

            var Uri = $"/3.0/lists/{TargetAudience}{EndpointAttribute.Url}";

            var JsonString = JsonSerializer.Serialize(NewMember);
            var RequestContent = new StringContent(JsonString, new System.Net.Http.Headers.MediaTypeHeaderValue("application/json"));

            var Res = await client.PostAsync(Uri, RequestContent);

            if (Res.StatusCode == System.Net.HttpStatusCode.BadRequest || Res.StatusCode == System.Net.HttpStatusCode.Conflict)
                Console.ForegroundColor = ConsoleColor.Red;
            else if (Res.StatusCode == System.Net.HttpStatusCode.OK || Res.StatusCode == System.Net.HttpStatusCode.NoContent)
                Console.ForegroundColor = ConsoleColor.Green;
            else
                Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine($"Status: {Res.StatusCode}\n\rMessage: {await Res.Content.ReadAsStringAsync()}");
        }
    }
}
