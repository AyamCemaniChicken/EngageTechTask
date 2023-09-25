using Discord;
using DiscordInteractions = Discord.Interactions;
using Discord.WebSocket;

namespace EngageTechTask.Discord.Preconditions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    class RequirePermissionsAttribute : DiscordInteractions.PreconditionAttribute
    {
        private readonly GuildPermission[] _permissions;

        public RequirePermissionsAttribute(params GuildPermission[] permissions)
        {
            _permissions = permissions;
        }

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

            var missingPermissions = _permissions
                .Where(permission => !user.GuildPermissions.Has(permission))
                .ToList();

            if (missingPermissions.Any())
            {
                var missingPermissionNames = string.Join(", ", missingPermissions.Select(permission => permission.ToString()));
                return DiscordInteractions.PreconditionResult.FromError($"You must have the following permissions to run this command: {missingPermissionNames}");
            }

            return DiscordInteractions.PreconditionResult.FromSuccess();
        }
    }
}
