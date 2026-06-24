using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Scrappy.Bot.Interfaces;
using Scrappy.Bot.Services;

namespace Scrappy.Bot.Handlers;

public class ReadyHandler : IEventHandler
{
    private readonly DiscordSocketClient _client;
    private readonly InteractionService _interactions;
    private readonly LoggingService _logger;
    private readonly IServiceProvider _services;

    public ReadyHandler(
        DiscordSocketClient client,
        InteractionService interactions,
        IServiceProvider services,
        LoggingService logger)
    {
        _client = client;
        _interactions = interactions;
        _services = services;
        _logger = logger;
    }

    public Task InitializeAsync()
    {
        _client.Ready += OnReadyAsync;
        return Task.CompletedTask;
    }

    private async Task OnReadyAsync()
    {
        await _interactions.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        await _logger.LogAsync(new LogMessage(
            LogSeverity.Info,
            "Ready",
            $"Found {_interactions.Modules.Count} modules:"
        ));

        foreach (var module in _interactions.Modules)
            await _logger.LogAsync(new LogMessage(
                LogSeverity.Info,
                "Ready",
                module.Name
            ));

        await _interactions.RegisterCommandsToGuildAsync(928369763552464997); // Replace guild id through options later
        await _logger.LogAsync(new LogMessage(
            LogSeverity.Info,
            "Ready",
            $"Commands uploaded and Bot is connected as {_client.CurrentUser.Username}"
        ));
    }
}