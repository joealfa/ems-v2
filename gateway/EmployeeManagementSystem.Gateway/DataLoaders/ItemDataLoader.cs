using EmployeeManagementSystem.ApiClient.Generated;
using EmployeeManagementSystem.Gateway.Caching;

namespace EmployeeManagementSystem.Gateway.DataLoaders;

public class ItemDataLoader : BatchDataLoader<long, ItemResponseDto?>
{
    private readonly EmsApiClient _client;
    private readonly IRedisCacheService _cache;
    private readonly ILogger<ItemDataLoader> _logger;
    private readonly TimeSpan _cacheTtl = TimeSpan.FromMinutes(10);

    public ItemDataLoader(
        EmsApiClient client,
        IRedisCacheService cache,
        ILogger<ItemDataLoader> logger,
        IBatchScheduler batchScheduler,
        DataLoaderOptions? options = null)
        : base(batchScheduler, options ?? new DataLoaderOptions())
    {
        _client = client;
        _cache = cache;
        _logger = logger;
    }

    protected override async Task<IReadOnlyDictionary<long, ItemResponseDto?>> LoadBatchAsync(
        IReadOnlyList<long> keys,
        CancellationToken ct)
    {
        Dictionary<long, ItemResponseDto?> results = [];
        List<long> keysToFetch = [];

        // Check cache first
        foreach (long key in keys)
        {
            ItemResponseDto? cached = await _cache.GetAsync<ItemResponseDto>(CacheKeys.Item(key), ct);
            if (cached is not null)
            {
                results[key] = cached;
            }
            else
            {
                keysToFetch.Add(key);
            }
        }

        // Fetch missing items from API
        foreach (long key in keysToFetch)
        {
            try
            {
                ItemResponseDto? item = await _client.ItemsGET2Async(key, ct);
                results[key] = item;

                if (item is not null)
                {
                    await _cache.SetAsync(CacheKeys.Item(key), item, _cacheTtl, ct);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to fetch item {DisplayId}", key);
                results[key] = null;
            }
        }

        return results;
    }
}
