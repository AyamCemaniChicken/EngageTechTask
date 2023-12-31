﻿using System.Reflection;
using System.Windows.Input;
using Discord;
using DiscordInteractions =Discord.Interactions;
using Discord.WebSocket;
using EngageTechTask.Discord.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EngageTechTask.Discord.Services;

public class InteractionHandlingService
{
    private readonly DiscordInteractions.InteractionService _interactions;
    private readonly DiscordShardedClient _client;
    private readonly IServiceProvider _services;

    public InteractionHandlingService(IServiceProvider services)
    {
        _interactions = services.GetRequiredService<DiscordInteractions.InteractionService>();
        _client = services.GetRequiredService<DiscordShardedClient>();
        _services = services;
    }

    public void Initialize()
    {
        // process the InteractionCreated payloads to execute Interactions commands
        _client.InteractionCreated += HandleInteraction;

        // process the command execution results 
        _interactions.SlashCommandExecuted += SlashCommandExecuted;
        _interactions.ContextCommandExecuted += ContextCommandExecuted;
        _interactions.ComponentCommandExecuted += ComponentCommandExecuted;
    }

    private Task ComponentCommandExecuted(DiscordInteractions.ComponentCommandInfo arg1, IInteractionContext arg2, DiscordInteractions.IResult arg3)
    {
        if (!arg3.IsSuccess)
        {
            switch (arg3.Error)
            {
                case DiscordInteractions.InteractionCommandError.UnmetPrecondition:
                    // implement
                    break;
                case DiscordInteractions.InteractionCommandError.UnknownCommand:
                    // implement
                    break;
                case DiscordInteractions.InteractionCommandError.BadArgs:
                    // implement
                    break;
                case DiscordInteractions.InteractionCommandError.Exception:
                    // implement
                    break;
                case DiscordInteractions.InteractionCommandError.Unsuccessful:
                    // implement
                    break;
                default:
                    break;
            }
        }

        return Task.CompletedTask;
    }

    private Task ContextCommandExecuted(DiscordInteractions.ContextCommandInfo arg1, IInteractionContext arg2, DiscordInteractions.IResult arg3)
    {
        if (!arg3.IsSuccess)
        {
            switch (arg3.Error)
            {
                case DiscordInteractions.InteractionCommandError.UnmetPrecondition:
                    // implement
                    break;
                case DiscordInteractions.InteractionCommandError.UnknownCommand:
                    // implement
                    break;
                case DiscordInteractions.InteractionCommandError.BadArgs:
                    // implement
                    break;
                case DiscordInteractions.InteractionCommandError.Exception:
                    // implement
                    break;
                case DiscordInteractions.InteractionCommandError.Unsuccessful:
                    // implement
                    break;
                default:
                    break;
            }
        }

        return Task.CompletedTask;
    }

    private async Task SlashCommandExecuted(DiscordInteractions.SlashCommandInfo arg1, IInteractionContext arg2, DiscordInteractions.IResult arg3)
    {
        if (!arg3.IsSuccess)
        {
            switch (arg3.Error)
            {
                case DiscordInteractions.InteractionCommandError.UnmetPrecondition:
                    if (arg2.Interaction.HasResponded)
                        await arg2.Interaction.FollowupAsync($"Unmet Precondition: {arg3.ErrorReason}");
                    else
                        await arg2.Interaction.RespondAsync($"Unmet Precondition: {arg3.ErrorReason}", ephemeral: true);
                    break;
                case DiscordInteractions.InteractionCommandError.UnknownCommand:
                    if (arg2.Interaction.HasResponded)
                        await arg2.Interaction.FollowupAsync("Unknown command", ephemeral: true);
                    else
                        await arg2.Interaction.RespondAsync("Unknown command", ephemeral: true);
                    break;
                case DiscordInteractions.InteractionCommandError.BadArgs:
                    if (arg2.Interaction.HasResponded)
                        await arg2.Interaction.FollowupAsync("Invalid number or arguments");
                    else
                        await arg2.Interaction.RespondAsync("Invalid number or arguments", ephemeral: true);
                    break;
                case DiscordInteractions.InteractionCommandError.Exception:
                    Console.WriteLine("Command Error:");
                    Console.WriteLine(arg3.ErrorReason);
                    await GenerateMessage.Error(arg2, title: $"Command exception: {arg3.ErrorReason}.", description: "If this message persists, please let us know in the support server!", ephemeral: true);
                    //await arg2.Interaction.RespondAsync($"Command exception: {arg3.ErrorReason}. If this message persists, please let us know in the support server ({Config.SupportServer}) !", ephemeral: true);
                    break;
                case DiscordInteractions.InteractionCommandError.Unsuccessful:
                    if (arg2.Interaction.HasResponded)
                        await arg2.Interaction.FollowupAsync("Command could not be executed");
                    else
                        await arg2.Interaction.RespondAsync("Command could not be executed", ephemeral: true);
                    break;
                default:
                    break;
            }
        }
        return;
    }

    private async Task HandleInteraction(SocketInteraction arg)
    {
        try
        {
            // create an execution context that matches the generic type parameter of your InteractionModuleBase<T> modules
            var ctx = new DiscordInteractions.ShardedInteractionContext(_client, arg);
            await _interactions.ExecuteCommandAsync(ctx, _services);
            Console.WriteLine("processed interaction!");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Caught exception:");
            Console.WriteLine(ex);
            // if a Slash Command execution fails it is most likely that the original interaction acknowledgement will persist. It is a good idea to delete the original
            // response, or at least let the user know that something went wrong during the command execution.
            if (arg.Type == InteractionType.ApplicationCommand)
            {
                await arg.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
            }
        }
    }
}