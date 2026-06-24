using Discord;
using Discord.WebSocket;
using DotNetEnv;
using Microsoft.Extensions.Configuration;
using Scrappy.Bot.Interfaces;

namespace Scrappy.Bot.Services;

public class DiscordBotService
{
    private readonly DiscordSocketClient _client;
    private readonly IEnumerable<IEventHandler> _handlers;
    private readonly IConfiguration _configuration;

    public DiscordBotService(
        DiscordSocketClient client,
        IEnumerable<IEventHandler> handlers,
        IConfiguration configuration
    )
    {
        _client = client;
        _handlers = handlers;
        _configuration = configuration;
    }

    public HashSet<ulong> DeveloperIds { get; } = [];

    public async Task StartAsync()
    {
        // Start handlers
        foreach (var handler in _handlers) await handler.InitializeAsync();

        // Login and start bot
        var botToken = _configuration.GetValue<string>("Bot:Token");
        if (string.IsNullOrEmpty(botToken)) throw new ArgumentException("Token not provided in environment variables.");;

        await _client.LoginAsync(TokenType.Bot, botToken);
        await _client.StartAsync();

        // TODO: Clean this up. Separate helper method maybe?
        // Get developer ids from developer portal
        var application = await _client.GetApplicationInfoAsync();

        // Check if application is part of a team
        if (application.Team != null)
        {
           // Add all team members to list
            foreach (var member in application.Team.TeamMembers)
            {
                DeveloperIds.Add(member.User.Id);
            }
        }
        else
        {
            // No team so individual developer
            DeveloperIds.Add(application.Owner.Id);
        }
    }
}