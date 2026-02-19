using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Scrappy.Bot.Handlers;
using Scrappy.Bot.Interfaces;
using Scrappy.Bot.Services;
using Scrappy.Data.Interfaces;
using Scrappy.Data.Repositories;

namespace Scrappy.Bot.Extensions;

public static class ServiceExtension
{
    public static IServiceCollection AddBotServices(this IServiceCollection services)
    {
        services.AddSingleton(x => new InteractionService(
            x.GetRequiredService<DiscordSocketClient>(),
            x.GetRequiredService<InteractionServiceConfig>()
        ));
        services.AddSingleton<LoggingService>();
        services.AddSingleton<DiscordBotService>();
        services.AddSingleton<GuildConfigService>();
        services.AddSingleton<LevelService>();
        services.AddHttpClient();
        services.AddMemoryCache();

        return services;
    }

    public static IServiceCollection AddBotHandlers(this IServiceCollection services)
    {
        services.AddSingleton<IEventHandler, ReadyHandler>();
        services.AddSingleton<IEventHandler, MessageUpdatesHandler>();
        services.AddSingleton<IEventHandler, MessageReceivedHandler>();
        services.AddSingleton<IEventHandler, InteractionHandler>();

        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IInfractionRepository, InfractionRepository>();
        services.AddScoped<IGuildConfigRepository, GuildConfigRepository>();
        services.AddScoped<ILevelUserRepository, LevelUserRepository>();

        return services;
    }
}