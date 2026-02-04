using System.Reflection;
using Bot.Interfaces;
using Bot.Services;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace Bot.Handlers;

public class ReadyHandler : IEventHandler
{
    private readonly DiscordSocketClient _client;
    private readonly InteractionService _interactions;
    private readonly IServiceProvider _services;
    private readonly LoggingService _logger;

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
        await _logger.LogAsync(new(
            LogSeverity.Info,
            "Ready",
            $"Found {_interactions.Modules.Count} modules:"
        ));

        foreach (var module in _interactions.Modules)
        {
            await _logger.LogAsync(new(
                LogSeverity.Info,
                "Ready",
                module.Name
            ));
        }

        await _interactions.RegisterCommandsToGuildAsync(928369763552464997); // Replace guild id through options later
        await _logger.LogAsync(new(
            LogSeverity.Info,
            "Ready",
            $"Commands uploaded and Bot is connected as {_client.CurrentUser.Username}"
        ));
    }
}