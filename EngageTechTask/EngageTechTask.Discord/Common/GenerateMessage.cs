using Discord;
using Discord.Commands;

namespace EngageTechTask.Discord.Common;

public class GenerateMessage
{
    public static async Task Error(
        dynamic context,
        string? title = null,
        string? description = null,
        string? color = null,
        bool embeded = true,
        bool ephemeral = false
        )
    {

        var embedBuilder = new EmbedBuilder()
            .WithColor(Config.Colors.Error)
            .WithTitle("An Error has Occured");

        if (string.IsNullOrEmpty(description))
            description = "If this error persists please open a support ticket.";

        if (!string.IsNullOrEmpty(title))
            embedBuilder.WithTitle(title);

        if (!string.IsNullOrEmpty(description))
            embedBuilder.WithDescription(description);

        if (!string.IsNullOrEmpty(color))
            embedBuilder.WithColor(new Color(Convert.ToUInt32(color, 16)));

        if (context is IInteractionContext interactionContext)
        {
            if (embeded)
                if (interactionContext.Interaction.HasResponded)
                    await interactionContext.Interaction.FollowupAsync(embed: embedBuilder.Build(), components: null, ephemeral: ephemeral);
                else
                    await interactionContext.Interaction.RespondAsync(embed: embedBuilder.Build(), components: null, ephemeral: ephemeral);
            else
                if (interactionContext.Interaction.HasResponded)
                await interactionContext.Interaction.FollowupAsync(description, components: null, ephemeral: ephemeral);
            else
                await interactionContext.Interaction.RespondAsync(description, components: null, ephemeral: ephemeral);
        }
        else if (context is ICommandContext commandContext)
        {
            if (embeded)
                await commandContext.Channel.SendMessageAsync(embed: embedBuilder.Build(), components: null);
            else
                await commandContext.Channel.SendMessageAsync(description, components: null);
        }
        else
            throw new Exception("Invalid context type");
    }
}