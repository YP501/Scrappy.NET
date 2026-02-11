using Discord;
using Discord.Interactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using Scrappy.Data;
using Scrappy.Data.Enums;
using Scrappy.Data.Interfaces;
using Scrappy.Data.Models;

namespace Scrappy.Bot.Commands.General;

public class TestDbCommand : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IServiceScopeFactory _scopeFactory;
    
    public TestDbCommand(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    [SlashCommand("testdb", "Test the database connection")]
    public async Task ExecuteAsync(IGuildUser targetUser)
    {
        await DeferAsync();

        try
        {
            using var scope = _scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IInfractionRepository>();
            var infraction = new Infraction
            {
                TargetId = targetUser.Id,
                Reason = "Test",
                GuildId = Context.Guild.Id,
                Type = InfractionType.Kick,
                IssuerId = Context.User.Id,
                CaseId = "pluh"
            };
            await repo.AddInfractionAsync(infraction);
            
            await FollowupAsync("Success!");
        }
        catch (DbUpdateException ex) when (ex.InnerException is MySqlException { Number: 1062 }) // Check for 1/99999999999 or something change for duplicate
        {
            await FollowupAsync("🤯 **GEFELICITEERD!** Je hebt zojuist de statistische loterij gewonnen. " +
                                "Je hebt een CaseID gegenereerd die al bestond (kans van 1 op de triljoen). " +
                                "Probeer het nog een keer, de kans dat dit twee keer achter elkaar gebeurt is letterlijk onmogelijk.");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            await FollowupAsync("Something went wrong!");
        }
    }
}