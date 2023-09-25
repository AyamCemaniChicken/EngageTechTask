using System.Text;
using System.Text.Json;
using Discord;
using EngageTechTask.Common;
using Shared.MongoDB;

namespace EngageTechTask.Push;

public class PushService
{
    private Configuration _config { get; set; }
    private DBClient _dbClient { get; set; }

    public PushService(DBClient dbClient, Configuration config)
    {
        _dbClient = dbClient;
        _config = config;
    }

    public async Task Run(IMessageChannel msgChannel)
    {
        var users = await _dbClient.GetAll<User>("Processed", false);

        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

        var requestMsg = new HttpRequestMessage(HttpMethod.Post, _config.Endpoint.Url);
        if (_config.Endpoint.AuthRequired)
            requestMsg.Headers.Add("Authorization", $"Bearer {_config.Endpoint.AuthToken}");

        var reqString = JsonSerializer.Serialize(users, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
        requestMsg.Content = new StringContent(reqString, Encoding.UTF8, "application/json");

        var response = await httpClient.SendAsync(requestMsg);
        if (!response.IsSuccessStatusCode)
            await msgChannel.SendMessageAsync($"{response.StatusCode}: {await response.Content.ReadAsStringAsync()} \r\n{DateTime.Now}");
        else
            await msgChannel.SendMessageAsync($"Data pushed \r\n{DateTime.Now}");
    }
}

