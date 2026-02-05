using Bot.Handlers;
using Bot.Interfaces;
using Bot.Services;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace Bot.Extensions;

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
}