using EngageTechTask.Discord.Common;
using Discord;
using Shared.MongoDB;
using Discord.Interactions;
using MongoDB.Bson;
using EngageTechTask.Common;
using EngageTechTask.Fetch.Enum;
using EngageTechTask.Fetch;

namespace EngageTechTask.Discord.Modules.Interaction;

public class GeneralModule
{
    public class GeneralGroup : InteractionsBase
    {
        FetchService _fs;

        public GeneralGroup(DBClient dbClient, FetchService fs) : base(dbClient)
        {
            _fs = fs;
        }

        [SlashCommand("ping", "Display bot latency")]
        public async Task LatencyAsync()
        {
            await DeferAsync();

            int latency = Context.Client.Latency;

            var embed = new EmbedBuilder()
                .WithTitle("Latency")
                .WithDescription($"⌛ {latency} ms")
                .WithColor(Config.Colors.Primary)
                .Build();

            await FollowupAsync(embed: embed);
        }

        [SlashCommand("run", "Run integration")]
        public async Task Run()
        {
            await DeferAsync();

            var existingConfigs = await _dbClient.GetAll<Configuration>();
            if (existingConfigs.Count() < 1)
            {
                var err = new EmbedBuilder()
                    .WithTitle("Profile unconfigured")
                    .WithDescription("Please run /configure to start.")
                    .WithColor(Config.Colors.Error)
                    .Build();

                await FollowupAsync(embed: err);

                return;
            }

            var latestConf = existingConfigs.OrderBy(cfg => cfg.Created).FirstOrDefault();

            var embed = new EmbedBuilder()
                .WithTitle("Running")
                .WithColor(Config.Colors.Primary)
                .Build();

            var logChannel = Context.Guild.Channels.First(chnl => chnl.Name == "logs") as IMessageChannel;

            _fs._running = true;
            _fs.Start(logChannel);

            await FollowupAsync(embed: embed);
        }

        [SlashCommand("stop", "Stop integration")]
        public async Task Stop()
        {
            _fs._running = false;
            var logChannel = Context.Guild.Channels.First(chnl => chnl.Name == "logs") as IMessageChannel;
            await logChannel.SendMessageAsync($"Integration stopped {DateTime.Now}");
            await RespondAsync("Stopped");
        }

        [SlashCommand("configure", "Configure integration")]
        public async Task Configure()
        {
            var mb = new ModalBuilder()
                .WithCustomId("config_integration")
                .WithTitle("Integration Settings")
                .AddTextInput("Endpoint URL", "endpoint", required: true, style: TextInputStyle.Short)
                .AddTextInput("Endpoint token", "endpoint_token", required: false)
                .AddTextInput("First web source URL", "web_source", required: true, style: TextInputStyle.Short)
                .AddTextInput("Web source token", "web_source_token", required: false)
                .AddTextInput("Run time", "run_time", required: false, placeholder: "hh:MM:ss");

            await Context.Interaction.RespondWithModalAsync(mb.Build());

            Context.Client.ModalSubmitted += async modal =>
            {
                var components = modal.Data.Components.ToList();
                string endpointUrl = components.First(x => x.CustomId == "endpoint").Value;
                string endpointToken = components.First(x => x.CustomId == "endpoint_token").Value;
                bool requiresAuth = string.IsNullOrWhiteSpace(endpointToken) ? false : true;
                string webSourceUrl = components.First(x => x.CustomId == "web_source").Value;
                string webSourceToken = components.First(x => x.CustomId == "web_source_token").Value;
                bool webSourceRequiresAuth = string.IsNullOrWhiteSpace(webSourceToken) ? false : true;

                string runTimeStr = components.First(x => x.CustomId == "run_time").Value;
                TimeOnly runTime;
                if (!string.IsNullOrWhiteSpace(runTimeStr))
                {
                    TimeOnly.TryParse(runTimeStr, out runTime);
                }
                else
                {
                    runTime = new TimeOnly(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                }

                var config = new Configuration()
                {
                    Endpoint = new WebSource()
                    {
                        Url = endpointUrl,
                        AuthRequired = requiresAuth,
                        AuthToken = endpointToken
                    },
                    WebSources = new List<WebSource>()
                    {
                        {
                            new WebSource()
                            {
                                Url = webSourceUrl,
                                AuthRequired = webSourceRequiresAuth,
                                AuthToken = webSourceToken
                            }
                        }
                    },
                    StartTime = runTime
                };

                await _dbClient.InsertOneAsync<Configuration>(config);

                await modal.RespondAsync("Success!");
            };
        }

        [SlashCommand("addsource", "Add an additonal web source")]
        public async Task AddSource()
        {
            var configs = await _dbClient.GetAll<Configuration>();
            if (configs.Count() < 1)
            {
                await RespondAsync("No existing configuration, please run /configure");
                return;
            }

            var latestConfig = configs.OrderBy(x => x.Created).FirstOrDefault();

            var mb = new ModalBuilder()
                .WithCustomId("add_source")
                .WithTitle("Add web source")
                .AddTextInput("Source URL", "source_url", required: true, style: TextInputStyle.Short)
                .AddTextInput("Source token", "source_token", required: false);

            await Context.Interaction.RespondWithModalAsync(mb.Build());

            Context.Client.ModalSubmitted += async modal =>
            {
                var components = modal.Data.Components.ToList();
                string webSourceUrl = components.First(x => x.CustomId == "source_url").Value;
                string webSourceToken = components.First(x => x.CustomId == "source_token").Value;
                bool webSourceRequiresAuth = string.IsNullOrWhiteSpace(webSourceToken) ? false : true;

                if (latestConfig.WebSources.Where(src => src.Url == webSourceUrl.Trim()).Count() > 0)
                {
                    await modal.RespondAsync("Source already added!");
                    return;
                }

                latestConfig.WebSources.Add(new WebSource()
                {
                    Url = webSourceUrl,
                    AuthToken = webSourceToken,
                    AuthRequired = webSourceRequiresAuth
                });

                await _dbClient.UpsertOneAsync(latestConfig, new ObjectId(latestConfig.Id));

                await modal.RespondAsync("Succesfully added source");
            };

        }

        [SlashCommand("changeinterval", "Change the interval between runs")]
        public async Task ChangeInterval()
        {
            var configs = await _dbClient.GetAll<Configuration>();
            if (configs.Count() < 1)
            {
                await RespondAsync("No existing configuration, please run /configure");
                return;
            }

            var latestConfig = configs.OrderBy(x => x.Created).FirstOrDefault();

            var mb = new ModalBuilder()
                .WithCustomId("change_interval")
                .WithTitle("Change run interval")
                .AddTextInput("Interval unit", "interval_unit", required: true, style: TextInputStyle.Short, maxLength: 5, placeholder: "Day, Week or Month")
                .AddTextInput("Interval period", "interval", required: true)
                .AddTextInput("Run time", "run_time", required: true, placeholder: "hh:MM:ss");

            await Context.Interaction.RespondWithModalAsync(mb.Build());

            Context.Client.ModalSubmitted += async modal =>
            {
                var components = modal.Data.Components.ToList();
                string intervalUnitStr = components.First(x => x.CustomId == "interval_unit").Value;
                string intervalStr = components.First(x => x.CustomId == "interval").Value;
                string runTimeStr = components.First(x => x.CustomId == "run_time").Value;

                IntervalUnit intervalUnit;
                int interval;
                TimeOnly runTime;

                var intervalUnitParsed = System.Enum.TryParse(intervalUnitStr, out intervalUnit);
                var intervalParse = int.TryParse(intervalStr, out interval);
                TimeOnly.TryParse(runTimeStr, out runTime);

                latestConfig.IntervalUnit = intervalUnit;
                latestConfig.Interval = interval;
                latestConfig.StartTime = runTime;

                await _dbClient.UpsertOneAsync(latestConfig, new ObjectId(latestConfig.Id));

                await modal.RespondAsync("Succesfully changed interval");
            };
        }
    }
}