using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scrappy.Bot.Handlers;
using Scrappy.Bot.Interfaces;
using Scrappy.Bot.Services;
using Scrappy.Data.Extensions;
using Scrappy.Data.Interfaces;
using Scrappy.Data.Repositories;

namespace Scrappy.Bot.Extensions;

public static class ServiceExtension
{
    public static IServiceCollection AddBotInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        DiscordSocketConfig clientConfig)
    {
        // Global Configuration
        services.AddSingleton<IConfiguration>(configuration);

        // Client & Interaction Config
        services.AddSingleton<DiscordSocketClient>(_ => new DiscordSocketClient(clientConfig));
        services.AddSingleton<InteractionService>(s => new InteractionService(s.GetRequiredService<DiscordSocketClient>()));

        // Core Bot Services
        services.AddSingleton<LoggingService>();
        services.AddSingleton<DiscordBotService>();
        services.AddSingleton<GuildConfigService>();
        services.AddSingleton<LevelService>();

        // .NET Utilities
        services.AddHttpClient();
        services.AddMemoryCache();

        // Discord Event Handlers
        services.AddSingleton<IEventHandler, GuildHandler>();
        services.AddSingleton<IEventHandler, InteractionHandler>();
        services.AddSingleton<IEventHandler, MessageHandler>();
        services.AddSingleton<IEventHandler, ReadyHandler>();

        // Database Layer
        services.AddDataServices(configuration);

        return services;
    }
}