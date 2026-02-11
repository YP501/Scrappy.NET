using Discord;
using Discord.Interactions;
using Scrappy.Data;
using Scrappy.Data.Enums;
using Scrappy.Data.Models;

namespace Scrappy.Bot.Commands.General;

public class TestDbCommand : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ScrappyDbContext _db;
    
    public TestDbCommand(ScrappyDbContext db)
    {
        _db = db;
    }

    [SlashCommand("testdb", "Test the database connection")]
    public async Task ExecuteAsync(IGuildUser targetUser)
    {
        await DeferAsync();
        
        try
        {
            var infraction = new Infraction
            {
                TargetId = targetUser.Id,
                Reason = "Test",
                GuildId = Context.Guild.Id,
                Type = InfractionType.Kick,
                IssuerId = Context.User.Id,
            };
            
            _db.Infractions.Add(infraction);
            await _db.SaveChangesAsync();

            await FollowupAsync("Success!");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            await FollowupAsync("Something went wrong!");
        }
    }
}