using EngageTechTask.Discord.Services;
using DiscordInteractions = Discord.Interactions;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Discord;
using Shared.MongoDB;
using EngageTechTask.Fetch;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System.Text.Json;

namespace EngageTechTask.Discord
{
    public class Startup
    {
        private DiscordShardedClient _client;
        private InteractionService _interactions;
        private CommandService _commands;
        private IServiceProvider _services;

        private System.Collections.Generic.IEnumerable<DiscordInteractions.ModuleInfo> _interactionModules;

        public async Task Initialize()
        {
            ObjectSerializer objectSerializer = new ObjectSerializer(type => ObjectSerializer.DefaultAllowedTypes(type) || type.FullName.StartsWith("System") || type.FullName.StartsWith("EngageTechTask"));
            BsonSerializer.RegisterSerializer(objectSerializer);

            BsonClassMap.RegisterClassMap<JsonElement>();

            var clientConfig = new DiscordSocketConfig()
            {
                GatewayIntents = GatewayIntents.All
            };

            await using var services = ConfigureServices(clientConfig);
            _services = services.GetRequiredService<IServiceProvider>();
            _client = services.GetRequiredService<DiscordShardedClient>();

            _client.Log += LogAsync;
            services.GetRequiredService<CommandService>().Log += LogAsync;

            var interactions = services.GetRequiredService<InteractionService>();
            _interactions = interactions;

            await InstallCommandsAsync();

            await _client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("ENGAGEBOTTOKEN"));

            await _client.SetGameAsync("Through your window.", null, ActivityType.Watching);
            await _client.StartAsync();

            // Here we initialize the logic required to register our commands.
            await services.GetRequiredService<CommandHandlingService>().InitializeAsync();
            services.GetRequiredService<InteractionHandlingService>().Initialize();
            services.GetRequiredService<MessageHandler>();

            await Task.Delay(-1);
        }

        public async Task InstallCommandsAsync()
        {
            _client.ShardReady += RegisterCommands;

            var assembly = Assembly.GetEntryAssembly();
            _interactionModules = await _interactions.AddModulesAsync(assembly, _services);
            
            // add modules from ComponentLib
            //_interactionModules = _interactionModules.Concat(await _interactions.AddModulesAsync(typeof(ComponentLib.ComponentLib).Assembly, new ComponentLib.ComponentLib()._services));
        }

        private async Task RegisterCommands(DiscordSocketClient client)
        {
            if (client.ShardId != 0) return;

            await _interactions.AddModulesGloballyAsync(true, _interactionModules.ToArray());
            
            foreach (var module in _interactionModules)
            {
                Console.WriteLine($"Registered {module.Name}");
            }
        }

        private static Task LogAsync(LogMessage log)
        {
            if (log.Exception is GatewayReconnectException)
            {
                if (log.Exception.InnerException is not null)
                    Console.WriteLine(log.Exception.InnerException.Message);
                return Task.CompletedTask;
            }

            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }

        private static ServiceProvider ConfigureServices(DiscordSocketConfig clientConfig)
        {
            return new ServiceCollection()
                .AddSingleton(new DiscordShardedClient(clientConfig))
                .AddSingleton<CommandService>()
                .AddSingleton<InteractionService>()
                .AddSingleton<InteractionHandlingService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<MessageHandler>()
                .AddSingleton(fetch => new FetchService(new DBClient("EngageBot", Environment.GetEnvironmentVariable("ENGAGEBOTCONNECTIONSTRING", EnvironmentVariableTarget.Process))))
                .AddSingleton(client => new DBClient("EngageBot", Environment.GetEnvironmentVariable("ENGAGEBOTCONNECTIONSTRING", EnvironmentVariableTarget.Process)))
                .BuildServiceProvider();
        }

        static bool IsDebug()
        {
#if DEBUG
            return true;
#else
            return false;
#endif
        }
    }
}