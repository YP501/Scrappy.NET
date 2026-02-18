using Discord;
using Discord.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Scrappy.Bot.Helpers;
using Scrappy.Data;
using Scrappy.Data.Models;

namespace Scrappy.Bot.Services;

public class LevelService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IMemoryCache _cache;
    private readonly GuildConfigService _guildConfigService;
    private readonly LoggingService _logger;
    private readonly Random _rng = new();

    public LevelService(IServiceProvider serviceProvider, IMemoryCache cache, GuildConfigService guildConfigService, LoggingService logger)
    {
        _serviceProvider = serviceProvider;
        _cache = cache;
        _guildConfigService =  guildConfigService;
        _logger = logger;
    }

    public async Task<LevelUser?> GetLevelUserAsync(ulong guildId, ulong userId)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BotDbContext>();

        return await dbContext.LevelUsers.FirstOrDefaultAsync(u => u.UserId == userId && u.GuildId == guildId);
    }

    public async Task<List<LevelUser>> GetTopUsersAsync(ulong guildId, int limit)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BotDbContext>();
        
        return await dbContext.LevelUsers
            .Where(u => u.GuildId == guildId)
            .OrderByDescending(u => u.TotalXp)
            .Take(limit)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task ProcessMessageXpAsync(IMessage message)
    {
        if (message.Author.IsBot || message.Author.IsWebhook ||
            message.Channel is not ITextChannel sentFromChannel) return;

        ulong userId = message.Author.Id;
        ulong guildId = sentFromChannel.GuildId;


        // We check if user is on XP cooldown
        string cacheKey = $"xp-cd-{guildId}-{userId}";
        if (_cache.TryGetValue(cacheKey, out _)) return;

        // TODO: Get cooldown from global bot config
        _cache.Set(cacheKey, true, TimeSpan.FromSeconds(30));

        // Database shenanigans
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BotDbContext>();

        var levelUser = await dbContext.LevelUsers.FirstOrDefaultAsync(u => u.GuildId == guildId && u.UserId == userId);
        if (levelUser == null)
        {
            levelUser = new()
            {
                GuildId = guildId,
                UserId = userId,
                TotalXp = 0,
                CurrentLevel = 0
            };
            dbContext.LevelUsers.Add(levelUser);
        }

        // random number between 15 and 30. TODO: Replace lower and upper with global bot config
        int xpToAdd = _rng.Next(15, 31);
        levelUser.TotalXp += xpToAdd;

        int oldLevel = levelUser.CurrentLevel;
        int newLevel = LevelHelper.CalculateLevelForXp(levelUser.TotalXp);

        bool isLevelUp = newLevel > oldLevel;
        if (isLevelUp)
        {
            levelUser.CurrentLevel = newLevel;
        }
        
        // Try to save XP and if it fails, remove cooldown for user
        try
        {
            await dbContext.SaveChangesAsync();
        }
        catch(Exception e)
        {
            _cache.Remove(cacheKey);
            await _logger.LogAsync(new LogMessage(LogSeverity.Error, "LevelService", e.Message));
            return;
        }
        
        // Send message if level-up
        if (isLevelUp)
        {
            var config = await _guildConfigService.GetOrAddConfigAsync(guildId);
            ITextChannel targetChannel = sentFromChannel; // Fallback to where message got sent from
            
            // Get level-up notification channel
            if (config.LevelUpChannelId.HasValue)
            {
                var configuredChannel = await sentFromChannel.Guild.GetTextChannelAsync(config.LevelUpChannelId.Value);
                if (configuredChannel != null)
                {
                    targetChannel = configuredChannel;
                }
            }

            string levelUpMessage = $"🎉 {message.Author.Mention} leveled up to level {newLevel}! 🎉";
            try
            {
                await targetChannel.SendMessageAsync(levelUpMessage);
            }
            catch (HttpException e)
            {
                // Fallback to original channel if configured channel doesn't work
                try { await sentFromChannel.SendMessageAsync(levelUpMessage); } catch {  /* give up */  }
            }
        }
    }
}