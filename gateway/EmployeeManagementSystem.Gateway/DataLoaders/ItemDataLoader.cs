using EmployeeManagementSystem.ApiClient.Generated;
using EmployeeManagementSystem.Gateway.Caching;

namespace EmployeeManagementSystem.Gateway.DataLoaders;

public class ItemDataLoader(
    EmsApiClient client,
    IRedisCacheService cache,
    ILogger<ItemDataLoader> logger,
    IBatchScheduler batchScheduler,
    DataLoaderOptions? options = null) : BatchDataLoader<long, ItemResponseDto?>(batchScheduler, options ?? new DataLoaderOptions())
{
    private readonly TimeSpan _cacheTtl = TimeSpan.FromMinutes(10);

    protected override async Task<IReadOnlyDictionary<long, ItemResponseDto?>> LoadBatchAsync(
        IReadOnlyList<long> keys,
        CancellationToken ct)
    {
        Dictionary<long, ItemResponseDto?> results = [];
        List<long> keysToFetch = [];

        // Check cache first
        foreach (long key in keys)
        {
            ItemResponseDto? cached = await cache.GetAsync<ItemResponseDto>(CacheKeys.Item(key), ct);
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
                ItemResponseDto? item = await client.ItemsGET2Async(key, ct);
                results[key] = item;

                if (item is not null)
                {
                    await cache.SetAsync(CacheKeys.Item(key), item, _cacheTtl, ct);
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to fetch item {DisplayId}", key);
                results[key] = null;
            }
        }

        return results;
    }
}
