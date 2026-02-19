using Discord;
using Discord.Net;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Scrappy.Bot.Helpers;
using Scrappy.Data.Interfaces;
using Scrappy.Data.Models;

namespace Scrappy.Bot.Services;

public class LevelService
{
    private readonly IMemoryCache _cache;
    private readonly GuildConfigService _guildConfigService;
    private readonly LoggingService _logger;
    private readonly Random _rng = new();
    private readonly IServiceProvider _serviceProvider;

    public LevelService(IServiceProvider serviceProvider, IMemoryCache cache, GuildConfigService guildConfigService,
        LoggingService logger)
    {
        _serviceProvider = serviceProvider;
        _cache = cache;
        _guildConfigService = guildConfigService;
        _logger = logger;
    }

    public async Task<LevelUser?> GetLevelUserAsync(ulong guildId, ulong userId)
    {
        using var scope = _serviceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ILevelUserRepository>();

        return await repository.GetLevelUserAsync(guildId, userId);
    }

    public async Task<List<LevelUser>> GetTopUsersAsync(ulong guildId, int limit)
    {
        using var scope = _serviceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ILevelUserRepository>();

        return await repository.GetTopUsersAsync(guildId, limit);
    }

    public async Task ProcessMessageXpAsync(IMessage message)
    {
        if (message.Author.IsBot || message.Author.IsWebhook ||
            message.Channel is not ITextChannel sentFromChannel) return;

        var userId = message.Author.Id;
        var guildId = sentFromChannel.GuildId;


        // We check if user is on XP cooldown
        var cacheKey = $"xp-cd-{guildId}-{userId}";
        if (_cache.TryGetValue(cacheKey, out _)) return;

        // TODO: Get cooldown from global bot config
        _cache.Set(cacheKey, true, TimeSpan.FromSeconds(30));

        // Database shenanigans
        using var scope = _serviceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ILevelUserRepository>();

        var levelUser = await repository.GetLevelUserAsync(guildId, userId);
        var isNew = false;

        if (levelUser == null)
        {
            levelUser = new LevelUser { GuildId = guildId, UserId = userId, TotalXp = 0 };
            isNew = true;
        }

        var oldLevel = LevelHelper.CalculateLevelForXp(levelUser.TotalXp);
        levelUser.TotalXp += _rng.Next(15, 31);
        var newLevel = LevelHelper.CalculateLevelForXp(levelUser.TotalXp);

        try
        {
            if (isNew)
                await repository.AddLevelUserAsync(levelUser);
            else
                await repository.UpdateLevelUser(levelUser);
        }
        catch (Exception e)
        {
            _cache.Remove(cacheKey);
            await _logger.LogAsync(new LogMessage(LogSeverity.Error, "LevelService", e.Message));
            return;
        }

        // Send message if level-up
        if (newLevel > oldLevel)
        {
            var config = await _guildConfigService.GetOrAddConfigAsync(guildId);
            var targetChannel = sentFromChannel; // Fallback to where message got sent from

            // Get level-up notification channel
            if (config.LevelUpChannelId.HasValue)
            {
                var configuredChannel = await sentFromChannel.Guild.GetTextChannelAsync(config.LevelUpChannelId.Value);
                if (configuredChannel != null) targetChannel = configuredChannel;
            }

            var levelUpMessage = $"🎉 {message.Author.Mention} leveled up to level {newLevel}! 🎉";
            try
            {
                await targetChannel.SendMessageAsync(levelUpMessage);
            }
            catch (HttpException e)
            {
                // Fallback to original channel if configured channel doesn't work
                try
                {
                    await sentFromChannel.SendMessageAsync(levelUpMessage);
                }
                catch
                {
                    /* give up */
                }
            }

            // TODO: Add automatic role adding based on achieved level
        }
    }
}