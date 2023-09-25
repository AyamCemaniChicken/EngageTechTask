using Discord;
using EngageTechTask.Common;
using EngageTechTask.Push;
using MongoDB.Bson;
using Shared.MongoDB;
using System.Text.Json;

namespace EngageTechTask.Fetch
{
    public class FetchService
    {
        private DBClient _dbClient { get; set; }
        private Configuration _config { get; set; }
        private JsonSerializerOptions jsonOpts = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };

        public bool _running { get; set; } = false;
        
        public FetchService(DBClient dbClient)
        {
            _dbClient = dbClient;
        }

        public async Task Start(IMessageChannel msgChannel)
        {
            await Task.Run(() =>
            {
                Run(msgChannel);
            });
        }

        public async Task GetConfig()
        {
            var configRes = await _dbClient.GetAll<Configuration>();
            _config = configRes.OrderBy(cfg => cfg.Created).FirstOrDefault();
        }

        public async Task Run(IMessageChannel msgChannel)
        {
            await msgChannel.SendMessageAsync($"Integration started {DateTime.Now}");

            if (_config == null)
                await GetConfig();

            if (_config.LastRunDate == null)
            {
                foreach (var source in _config.WebSources)
                {
                    await FetchUsers(source.Url, msgChannel, source.AuthRequired ? source.AuthToken : null);
                }

                _config.LastRunDate = DateTime.Now;
                await _dbClient.UpsertOneAsync<Configuration>(_config, new ObjectId(_config.Id));
            }

            if (_config.LastRunDate != null)
            {

                while (_running)
                {
                    await Task.Run(async () =>
                    {
                        int multiplier = 1;
                        _config = await _dbClient.GetByObjectId<Configuration>(new ObjectId(_config.Id));

                        switch (_config.IntervalUnit)
                        {
                            case Enum.IntervalUnit.Day:
                                multiplier = 1;
                                break;
                            case Enum.IntervalUnit.Week:
                                multiplier = 7;
                                break;
                            case Enum.IntervalUnit.Month:
                                multiplier = 30;
                                break;
                            default:
                                multiplier = 1;
                                break;
                        }

                        var timeSpan = (TimeSpan)(_config.LastRunDate - DateTime.Now);

                        var daysSinceLastRun = Math.Abs(timeSpan.Days);
                        var currentTime = TimeOnly.FromDateTime(DateTime.Now);

                        if (daysSinceLastRun >= (_config.Interval * multiplier) && (currentTime >= _config.StartTime && currentTime <= _config.StartTime.AddMinutes(10)) == true)
                        {
                            foreach (var source in _config.WebSources)
                            {
                                await FetchUsers(source.Url, msgChannel, source.AuthRequired ? source.AuthToken : null);
                                await msgChannel.SendMessageAsync($"Synced {DateTime.Now}");
                            }

                            _config.LastRunDate = DateTime.Now;
                            await _dbClient.UpsertOneAsync<Configuration>(_config, new ObjectId(_config.Id));
                        }
                        System.Threading.Thread.Sleep(1000 * 5);
                    });
                }
            }
        }

        public async Task FetchUsers(string url, IMessageChannel msgChannel, string? token = null)
        {
            var webClient = new HttpClient();
            var requestMsg = new HttpRequestMessage(HttpMethod.Get, url);

            if (token != null)
                requestMsg.Headers.Add("Authorization", $"Bearer {token}");

            var responseMsg = await webClient.SendAsync(requestMsg);

            if (!responseMsg.IsSuccessStatusCode)
            {
                await msgChannel.SendMessageAsync($"{responseMsg.StatusCode}: {await responseMsg.Content.ReadAsStringAsync()}");
                return;
            }

            var responseStr = await responseMsg.Content.ReadAsStringAsync();

            await SyncUsers(responseStr, msgChannel);
        }

        public async Task<bool> SyncUsers(string jsonString, IMessageChannel msgChannel)
        {
            if (_config == null)
                await GetConfig();

            var usersObject = JsonSerializer.Deserialize<UsersList>(jsonString, jsonOpts);
            
            if (usersObject.Users.Count() < 1)
                return false;

            foreach (var user in usersObject.Users)
            {
                var existing = await _dbClient.Get<User>("Email", user.Email);
                if (existing == null)
                {
                    user.Processed = false;
                    await _dbClient.InsertOneAsync<User>(user);
                }
                else
                {
                    user.Processed = false;
                    user.UID = existing.UID;
                    await _dbClient.UpsertOneAsync<User>(user, new ObjectId(existing.UID));
                }
            }

            var ps = new PushService(_dbClient, _config);
            await ps.Run(msgChannel);

            return true;
        }
    }
}

