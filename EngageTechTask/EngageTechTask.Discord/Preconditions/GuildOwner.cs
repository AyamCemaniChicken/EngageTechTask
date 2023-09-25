using Discord;
using DiscordInteractions = Discord.Interactions;
using Discord.WebSocket;

namespace BotEngageTechTask.Discord.Preconditions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    class GuildOwner : DiscordInteractions.PreconditionAttribute

    {

        public async override Task<DiscordInteractions.PreconditionResult> CheckRequirementsAsync(IInteractionContext context, DiscordInteractions.ICommandInfo commandInfo, IServiceProvider services)
        {
            if (context.Guild == null)
            {
                return DiscordInteractions.PreconditionResult.FromError("This command can only be executed from within server channels.");
            }

            var user = context.User as SocketGuildUser;

            if (user == null)
            {
                return DiscordInteractions.PreconditionResult.FromError("This command can only be executed in a server.");
            }

            var IsOwner = user.Guild.Owner.Id == user.Id;

            if (!IsOwner)
            {
                return DiscordInteractions.PreconditionResult.FromError($"This command can only be executed by the guild owner ({user.Guild.Owner.Username}).");
            }

            return DiscordInteractions.PreconditionResult.FromSuccess();
        }
    }
}
