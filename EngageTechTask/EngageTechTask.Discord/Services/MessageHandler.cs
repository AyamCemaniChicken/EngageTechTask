using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Shared.MongoDB;

namespace EngageTechTask.Discord.Services;

public class MessageHandler
{
    private static DBClient _dbClient;
    private readonly DiscordShardedClient _discord;
    private readonly IServiceProvider _services;

    public MessageHandler(IServiceProvider services, DBClient dbClient)
    {
        _discord = services.GetRequiredService<DiscordShardedClient>();
        _services = services;
        _dbClient = dbClient;

        _discord.MessageReceived += MessageReceivedAsync;
    }

    public static Task MessageReceivedAsync(SocketMessage rawMessage)
    {
        if (rawMessage is not SocketUserMessage message)
            return Task.CompletedTask;

        if (!(rawMessage.Source is MessageSource.User))
            return Task.CompletedTask;

        if (!(rawMessage.Channel is IGuildChannel))
            return Task.CompletedTask;

        // Add a point to the user's message count
        _ = Task.Run(async () =>
        {
        });

        return Task.CompletedTask;
    }

}