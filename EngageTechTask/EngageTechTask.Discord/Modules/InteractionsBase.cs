using Discord;
using Discord.Interactions;
using Shared.MongoDB;

namespace EngageTechTask.Discord.Modules
{
    public abstract class InteractionsBase : InteractionModuleBase<ShardedInteractionContext>
    {
        public DBClient _dbClient;
        protected Func<string, Embed, Task> Responder;
        protected Func<string, Embed, Task> PublicResponder;

        public InteractionsBase(DBClient dbClient)
        {
            _dbClient = dbClient;
            Responder = (response, embed) =>
            {
                if (embed != null)
                {
                    return RespondAsync(response, new[] { embed }, ephemeral: true);
                }
                return RespondAsync(response, ephemeral: true);
            };

            PublicResponder = (response, embed) =>
            {
                if (embed != null)
                {
                    return RespondAsync(response, new[] { embed }, ephemeral: false);
                }
                return RespondAsync(response, ephemeral: false);
            };
        }
    }
}
