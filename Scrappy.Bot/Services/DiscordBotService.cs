using Discord;
using Discord.WebSocket;
using Scrappy.Bot.Interfaces;

namespace Scrappy.Bot.Services;

public class DiscordBotService
{
    private readonly DiscordSocketClient _client;
    private readonly IEnumerable<IEventHandler> _handlers;

    public HashSet<ulong> DeveloperIds { get; } = [];

    public DiscordBotService(
        DiscordSocketClient client,
        IEnumerable<IEventHandler> handlers
    )
    {
        _client = client;
        _handlers = handlers;
    }

    public async Task StartAsync()
    {
        // Start handlers
        foreach (var handler in _handlers)
        {
            await handler.InitializeAsync();
        }

        // Login and start bot
        await _client.LoginAsync(TokenType.Bot, DotNetEnv.Env.GetString("DISCORD_TOKEN"));
        await _client.StartAsync();

        // Get developer ids from developer portal
        var application = await _client.GetApplicationInfoAsync();

        if (application.Team != null)
        {
            // It's a team, add all user ids of the team to the list
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