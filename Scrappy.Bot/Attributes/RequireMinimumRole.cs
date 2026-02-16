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
        // Bot developer can execute stuff in DMs so we check him first before we cast to IGuildUser
        var botService = services.GetRequiredService<DiscordBotService>();
        if (_requiredPermissionLevel == BotOwner && botService.DeveloperIds.Contains(context.User.Id))
        {
            return  PreconditionResult.FromSuccess();
        }
        
        var configService = services.GetRequiredService<GuildConfigService>();
        var config = await configService.GetOrAddConfigAsync(context.Guild.Id);
        
        // Get user info
        if (context.User is not IGuildUser guildUser)
        {
            return PreconditionResult.FromError("Could not verify guild user permissions!");
        }

        bool hasPermission = HasPermissionLevel(_requiredPermissionLevel, guildUser, config, context.Guild.OwnerId);
        return hasPermission
            ? PreconditionResult.FromSuccess()
            : PreconditionResult.FromError($"You don't have permission to use that! Required: {_requiredPermissionLevel}");
    }
    
    private static bool HasPermissionLevel(PermissionLevel required, IGuildUser user, GuildConfig config, ulong guildOwnerId)
    {
        // Server owner can do anything as long as it's not bot owner requirement
        if (user.Id == guildOwnerId)
            return true;

        // Admin permission
        if ((config.AdminRoleId.HasValue && user.RoleIds.Contains(config.AdminRoleId.Value)) || user.GuildPermissions.Administrator)
            return required <= Administrator;

        // Moderator permission
        if (config.ModeratorRoleId.HasValue && user.RoleIds.Contains(config.ModeratorRoleId.Value))
            return required <= Moderator;

        return false;
    }

}