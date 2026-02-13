using Discord;
using Discord.Interactions;
using Microsoft.Extensions.DependencyInjection;
using Scrappy.Bot.Services;

namespace Scrappy.Bot.Attributes;

public class RequireModeratorRoleAttribute : PreconditionAttribute
{
    public override async Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context,
        ICommandInfo commandInfo, IServiceProvider services)
    {
        // Prevent crashes in DMs. Yes we can use CommandContextType but attributes should be autonomous
        if (context.Guild == null)
        {
            return PreconditionResult.FromError("This command can only be used within a server (guild).");
        }

        // Cast and initialize guildUser
        if (context.User is not IGuildUser guildUser)
        {
            return PreconditionResult.FromError("Could not verify guild user permissions.");
        }

        var configService = services.GetRequiredService<GuildConfigService>();
        var config = await configService.GetOrAddConfigAsync(context.Guild.Id);

        // Always let admins use this command to prevent command-lockout (cant use /settings to set role command access for example)
        if (guildUser.GuildPermissions.Administrator)
        {
            // Send a message to notify the admin when they don't have a moderator role configured
            if (!config.ModeratorRoleId.HasValue)
            {
                await context.User.SendMessageAsync("**⚠️ Admin Note**: The moderator role is not set for this server. Use `/settings` to configure it"); // TODO: Make this a fancy embed
            }

            return PreconditionResult.FromSuccess();
        }

        if (config.ModeratorRoleId.HasValue && guildUser.RoleIds.Contains(config.ModeratorRoleId.Value))
        {
            return PreconditionResult.FromSuccess();
        }

        return PreconditionResult.FromError("You do not have the required permissions to use this command.");
    }
}