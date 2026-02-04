using Bot.Interfaces;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace Bot.Handlers;

public class InteractionHandler : IEventHandler
{
    private readonly DiscordSocketClient _client;
    private readonly InteractionService _interactions;
    private readonly IServiceProvider _services;

    public InteractionHandler(DiscordSocketClient client, InteractionService interactions, IServiceProvider services)
    {
        _client = client;
        _interactions = interactions;
        _services = services;
    }

    public Task InitializeAsync()
    {
        _client.InteractionCreated += HandleInteraction;
        return Task.CompletedTask;
    }
    
    private async Task HandleInteraction(SocketInteraction interaction)
    {
        try
        {
            var context = new SocketInteractionContext(_client, interaction);
            await _interactions.ExecuteCommandAsync(context, _services);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error handling interaction: {ex}");
            if (interaction.Type == InteractionType.ApplicationCommand)
            {
                // Notify user something went wrong
                if (interaction.HasResponded)
                {
                    await interaction.FollowupAsync($"Execution failed: {ex.Message}", ephemeral: true);
                }
                else
                {
                    await interaction.RespondAsync($"Execution failed: {ex.Message}", ephemeral: true);
                }
            }
        }
    }
}