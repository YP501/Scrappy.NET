using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DotNetEnv;
using DotNetEnv.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scrappy.Bot.Extensions;
using Scrappy.Bot.Services;
using Scrappy.Data;
using Scrappy.Data.Extensions;

namespace Scrappy.Bot;

public static class Program
{
    public static async Task Main()
    {
        var clientConfig = new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.Guilds |
                             GatewayIntents.GuildBans |
                             GatewayIntents.GuildMembers |
                             GatewayIntents.GuildMessages |
                             GatewayIntents.MessageContent,
            MessageCacheSize = 500,
            AuditLogCacheSize = 0
        };

        var configuration = new ConfigurationBuilder()
            .AddDotNetEnv(".env", LoadOptions.TraversePath())
            .Build();

        var services = new ServiceCollection()
            .AddBotInfrastructure(configuration, clientConfig)
            .BuildServiceProvider();

        // Start bot
        var bot = services.GetRequiredService<DiscordBotService>();
        await bot.StartAsync();

        // Keep running
        await Task.Delay(-1);
    }
}