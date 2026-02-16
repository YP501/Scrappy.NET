using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Scrappy.Bot.Helpers;
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
            var cooldownEmbed = EmbedHelper.CreateCooldownEmbed(3);
            await interaction.RespondAsync(embed: cooldownEmbed, ephemeral: true);
            return;
        }
        
        var context = new SocketInteractionContext(_client, interaction);
        await _interactions.ExecuteCommandAsync(context, _services);
    }

    private async Task HandleInteractionExecuted(ICommandInfo info, IInteractionContext context, IResult result)
    {
        if (result.IsSuccess || context.Interaction is SocketAutocompleteInteraction) return;

        try
        {
            if (context.Interaction.HasResponded)
            {
                var errorEmbed = EmbedHelper.CreateErrorEmbed(result.ErrorReason);
                await context.Interaction.FollowupAsync(embed: errorEmbed, ephemeral: true);
            }
            else
            {
                var errorEmbed = EmbedHelper.CreateErrorEmbed(result.ErrorReason);
                await context.Interaction.RespondAsync(embed: errorEmbed, ephemeral: true);
            }
        }
        catch
        {
            // This only happens if the interaction token expired or Discord is down
            await _logger.LogAsync(new LogMessage(LogSeverity.Error, "InteractionHandler", "Couldn't notify user"));
        }
    }

    private bool IsOnCooldown(ulong userId)
    {
        // Bypass cooldown if you're a bot developer
        var botService = _services.GetRequiredService<DiscordBotService>();
        if (botService.DeveloperIds.Contains(userId)) return false;
        
        
        string key = $"cd-{userId}";
        if (_cache.TryGetValue(key, out _)) return true;
        
        _cache.Set(key, true, TimeSpan.FromSeconds(3)); 
        return false;
    }
}