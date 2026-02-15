using Discord;
using Discord.Interactions;
using Microsoft.Extensions.DependencyInjection;
using Scrappy.Bot.Services;
using Scrappy.Data.Enums;
using Scrappy.Data.Models;
using static Scrappy.Data.Enums.PermissionLevel;

namespace Scrappy.Bot.Attributes;

public class RequireMinimumPermissionAttribute : PreconditionAttribute
{
    private readonly PermissionLevel _requiredPermissionLevel;

    public RequireMinimumPermissionAttribute(PermissionLevel permissionLevel)
    {
        _requiredPermissionLevel = permissionLevel;
    }

    public override async Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context,
        ICommandInfo commandInfo, IServiceProvider services)
    {
        // Premature check if command is for bot owner only, however requiredPermissionLevel MUST Be BotOwner.
        // Else the developer could just execute any command on any server his bot is in which is a big no no
        if (_requiredPermissionLevel == BotOwner)
        {
            var application = await context.Client.GetApplicationInfoAsync();

            if (context.User.Id == application.Owner.Id)
            {
                return PreconditionResult.FromSuccess();
            }

            return PreconditionResult.FromError("Only the bot developer can use that!");
        }

        var configService = services.GetRequiredService<GuildConfigService>();
        var config = await configService.GetOrAddConfigAsync(context.Guild.Id);

        // Server owner check, should be able to execute anything marked as moderator or admin as wel
        if (context.User.Id == context.Guild.OwnerId)
        {
            // Do send them a reminder if roles haven't been set up yet
            await CheckMissingRolesAndNotifyAsync(context, config);
            return PreconditionResult.FromSuccess();
        }

        if (_requiredPermissionLevel == ServerOwner)
        {
            return PreconditionResult.FromError("Only the server owner can use that!");
        }

        if (context.User is not IGuildUser guildUser)
        {
            return PreconditionResult.FromError("Could not verify guild user permissions!");
        }

        // Admins can always do what moderators can, including people who have the actual discord admin permission
        if ((config.AdminRoleId.HasValue && guildUser.RoleIds.Contains(config.AdminRoleId.Value)) ||
            guildUser.GuildPermissions.Administrator)
        {
            // Do send them a reminder if roles haven't been set up yet
            await CheckMissingRolesAndNotifyAsync(context, config);
            return PreconditionResult.FromSuccess();
        }

        if (_requiredPermissionLevel == Administrator)
        {
            return PreconditionResult.FromError("Only server administrators can use that!");
        }

        // Check if we require and have moderator permission
        if (config.ModeratorRoleId.HasValue && guildUser.RoleIds.Contains(config.ModeratorRoleId.Value))
        {
            await CheckMissingRolesAndNotifyAsync(context, config);
            return PreconditionResult.FromSuccess();
        }

        return PreconditionResult.FromError("You do not have permission to use that command!");
    }

    private async Task CheckMissingRolesAndNotifyAsync(IInteractionContext context, GuildConfig config)
    {
        // Tuple array just in case we decide to add more roles in the future for permissions?
        var missingRoles = new[]
        {
            (config.ModeratorRoleId, "Moderator"),
            (config.AdminRoleId, "Administrator")
        }.Where(x => !x.Item1.HasValue).Select(x => $"`{x.Item2}`").ToList();

        if (!missingRoles.Any()) return;
        string rolesList = string.Join(" and ", missingRoles);

        bool isPlural = missingRoles.Count > 1;
        string verb = isPlural ? "roles are" : "role is";
        string pronoun = isPlural ? "them" : "it";

        await context.User.SendMessageAsync(
            $"""
             **⚠️ Admin Note**
             The {rolesList} {verb} are not set for this server. Use `/settings` to configure {pronoun}.
             You were able to run this command because you are either the server owner or have administrator permissions.
             """
        );
    }
}