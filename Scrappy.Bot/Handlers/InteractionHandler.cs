using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Scrappy.Bot.Interfaces;
using Scrappy.Bot.Services;

namespace Scrappy.Bot.Handlers;

public class InteractionHandler : IEventHandler
{
    private readonly DiscordSocketClient _client;
    private readonly InteractionService _interactions;
    private readonly IServiceProvider _services;
    private readonly IMemoryCache _cache;
    private readonly LoggingService _logger;

    public InteractionHandler(
        DiscordSocketClient client,
        InteractionService interactions,
        IServiceProvider services,
        IMemoryCache cache,
        LoggingService logger
        )
    {
        _client = client;
        _interactions = interactions;
        _services = services;
        _cache = cache;
        _logger = logger;
    }

    public Task InitializeAsync()
    {
        _client.InteractionCreated += HandleInteractionCreated;
        _interactions.SlashCommandExecuted += HandleInteractionExecuted; // Error handling on interactions
        return Task.CompletedTask;
    }

    private async Task HandleInteractionCreated(SocketInteraction interaction)
    {
        if (interaction is SocketSlashCommand && IsOnCooldown(interaction.User.Id))
        {
            
            // TODO: turn this in an embed and maybe add fetching cooldown from global config set by bot owner?
            await interaction.RespondAsync("Slow down! You can only run a command every 3 seconds", ephemeral: true);
            return;
        }
        
        var context = new SocketInteractionContext(_client, interaction);
        await _interactions.ExecuteCommandAsync(context, _services);
    }

    private async Task HandleInteractionExecuted(ICommandInfo info, IInteractionContext context, IResult result)
    {
        if (result.IsSuccess || context.Interaction is SocketAutocompleteInteraction) return;

        string userMessage = $"Error: {result.ErrorReason}";

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
        catch
        {
            // This only happens if the interaction token expired or Discord is down
            await _logger.LogAsync(new LogMessage(LogSeverity.Error, "InteractionHandler", "Couldn't notify user"));
        }
    }

    private bool IsOnCooldown(ulong userId) // TODO: add bypass cooldown role check
    {
        string key = $"cd-{userId}";
        if (_cache.TryGetValue(key, out _)) return true;

        // TODO: Get cooldown time from global config
        _cache.Set(key, true, TimeSpan.FromSeconds(3)); 
        return false;
    }
}