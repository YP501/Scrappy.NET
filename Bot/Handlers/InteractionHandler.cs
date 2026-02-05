using Bot.Interfaces;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Caching.Memory;

namespace Bot.Handlers;

public class InteractionHandler : IEventHandler
{
    private readonly DiscordSocketClient _client;
    private readonly InteractionService _interactions;
    private readonly IServiceProvider _services;
    private readonly IMemoryCache _cache;

    public InteractionHandler(DiscordSocketClient client, InteractionService interactions, IServiceProvider services,
        IMemoryCache cache)
    {
        _client = client;
        _interactions = interactions;
        _services = services;
        _cache = cache;
    }

    public Task InitializeAsync()
    {
        _client.InteractionCreated += HandleInteractionCreated;
        _interactions.SlashCommandExecuted += HandleInteractionExecuted; // Error handling on interactions
        return Task.CompletedTask;
    }

    private async Task HandleInteractionCreated(SocketInteraction interaction)
    {
        var context = new SocketInteractionContext(_client, interaction);

        if (interaction is SocketSlashCommand && IsOnCooldown(context.User.Id))
        {
            
            // TODO: turn this in an embed and get cooldown time from global settings
            await interaction.RespondAsync("Slow down! You can only run a command every 3 seconds");
            return;
        }

        await _interactions.ExecuteCommandAsync(context, _services);
    }

    private async Task HandleInteractionExecuted(ICommandInfo info, IInteractionContext context, IResult result)
    {
        if (result.IsSuccess || context.Interaction is SocketAutocompleteInteraction) return;

        string userMessage = $"Something went wrong: {result.ErrorReason}";

        try
        {
            if (context.Interaction.HasResponded)
            {
                await context.Interaction.FollowupAsync(userMessage, ephemeral: true); // TODO: Replace with embed
            }
            else
            {
                await context.Interaction.RespondAsync(userMessage, ephemeral: true); // TODO: Replace with embed
            }
        }
        catch (Exception ex)
        {
            // This only happens if the interaction token expired or Discord is down
            Console.WriteLine($"Could not notify user: {ex.Message}");
        }
    }

    private bool IsOnCooldown(ulong userId) // TODO: add bypass cooldown role check
    {
        string key = $"cd-{userId}";
        if (_cache.TryGetValue(key, out _)) return true;

        // TODO: Get cooldown time from global settings
        _cache.Set(key, true, TimeSpan.FromSeconds(3)); 
        return false;
    }
}