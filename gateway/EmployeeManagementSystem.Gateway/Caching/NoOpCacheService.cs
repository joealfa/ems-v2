namespace EmployeeManagementSystem.Gateway.Caching;

/// <summary>
/// No-operation cache service that bypasses caching entirely.
/// Useful for debugging and development.
/// </summary>
public class NoOpCacheService : IRedisCacheService
{
    private readonly ILogger<NoOpCacheService> _logger;

    public NoOpCacheService(ILogger<NoOpCacheService> logger)
    {
        _logger = logger;
        _logger.LogInformation("Using NoOpCacheService - caching is DISABLED");
    }

    public Task<T?> GetAsync<T>(string key, CancellationToken ct = default) where T : class
    {
        _logger.LogDebug("NoOpCache: Skipping GetAsync for key {Key}", key);
        return Task.FromResult<T?>(null);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? ttl = null, CancellationToken ct = default) where T : class
    {
        _logger.LogDebug("NoOpCache: Skipping SetAsync for key {Key}", key);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key, CancellationToken ct = default)
    {
        _logger.LogDebug("NoOpCache: Skipping RemoveAsync for key {Key}", key);
        return Task.CompletedTask;
    }

    public Task RemoveByPrefixAsync(string prefix, CancellationToken ct = default)
    {
        _logger.LogDebug("NoOpCache: Skipping RemoveByPrefixAsync for prefix {Prefix}", prefix);
        return Task.CompletedTask;
    }

    public async Task<T?> GetOrSetAsync<T>(string key, Func<CancellationToken, Task<T?>> factory, TimeSpan? ttl = null, CancellationToken ct = default) where T : class
    {
        _logger.LogDebug("NoOpCache: Bypassing cache for key {Key}, calling factory directly", key);
        return await factory(ct);
    }
}
