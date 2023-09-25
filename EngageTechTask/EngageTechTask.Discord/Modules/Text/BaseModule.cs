using Discord.Commands;
using Discord;
using Shared.MongoDB;

namespace EngageTechTask.Discord.Modules.Text;

public class BaseModule : ModuleBase<ShardedCommandContext>
{
    private DBClient _dbClient;
    public BaseModule(DBClient dbClient)
    {
        _dbClient = dbClient;
    }

    [Command("ping")]
    [Summary("Pong!")] 
    public Task PongAsync()
        => ReplyAsync("Pong!");
}