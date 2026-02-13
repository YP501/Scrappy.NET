using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Scrappy.Data.Interfaces;
using Scrappy.Data.Models;

namespace Scrappy.Bot.Services;

public class GuildConfigService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ConcurrentDictionary<ulong, GuildConfig> _cache = new();

    public GuildConfigService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task<GuildConfig> GetOrAddConfigAsync(ulong guildId)
    {
        // Check if we already have a cached config
        if (_cache.TryGetValue(guildId, out var cachedConfig))
        {
            return cachedConfig;
        }
        
        // Not in cache so we fetch from DB
        using var scope = _scopeFactory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IGuildConfigRepository>();
        var config = await repository.GetConfigAsync(guildId);

        if (config == null)
        {
            config = new GuildConfig { GuildId = guildId };
            await repository.AddConfigAsync(config);
        }
        
        // Save config in cache and return it
        _cache[guildId] = config;
        return config;
    }
    
    public async Task UpdateConfigAsync(GuildConfig config)
    {
        // Update in DB
        using var scope = _scopeFactory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IGuildConfigRepository>();
        await repository.SaveConfigAsync(config);
        
        // Update cache
        _cache[config.GuildId] = config;
    }

    public void RemoveFromCache(ulong guildId)
    {
        _cache.TryRemove(guildId, out _);
    }
}