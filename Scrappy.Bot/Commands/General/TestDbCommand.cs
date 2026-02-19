using Discord;
using Discord.Interactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
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
                IssuerId = Context.User.Id
            };
            await repo.AddInfractionAsync(infraction);

            await FollowupAsync("Success!");
        }
        catch (DbUpdateException ex) when
            (ex.InnerException is MySqlException
             {
                 Number: 1062
             }) // Check for 1/99999999999 or something change for duplicate
        {
            await FollowupAsync("""
                                🤯 CONGRATULATIONS! You just won the statistical lottery.
                                You generated a CaseID that already exists (a one-in-a-trillion chance).
                                Try again. The odds of this happening twice in a row are literally impossible.
                                """);
        }
    }
}