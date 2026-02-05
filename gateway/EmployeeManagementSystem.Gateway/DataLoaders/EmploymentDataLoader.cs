using EmployeeManagementSystem.ApiClient.Generated;
using EmployeeManagementSystem.Gateway.Caching;

namespace EmployeeManagementSystem.Gateway.DataLoaders;

public class EmploymentDataLoader(
    EmsApiClient client,
    IRedisCacheService cache,
    ILogger<EmploymentDataLoader> logger,
    IBatchScheduler batchScheduler,
    DataLoaderOptions? options = null) : BatchDataLoader<long, EmploymentResponseDto?>(batchScheduler, options ?? new DataLoaderOptions())
{
    private readonly TimeSpan _cacheTtl = TimeSpan.FromMinutes(10);

    protected override async Task<IReadOnlyDictionary<long, EmploymentResponseDto?>> LoadBatchAsync(
        IReadOnlyList<long> keys,
        CancellationToken ct)
    {
        Dictionary<long, EmploymentResponseDto?> results = [];
        List<long> keysToFetch = [];

        // Check cache first
        foreach (long key in keys)
        {
            EmploymentResponseDto? cached = await cache.GetAsync<EmploymentResponseDto>(CacheKeys.Employment(key), ct);
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
                EmploymentResponseDto? employment = await client.EmploymentsGET2Async(key, ct);
                results[key] = employment;

                if (employment is not null)
                {
                    await cache.SetAsync(CacheKeys.Employment(key), employment, _cacheTtl, ct);
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to fetch employment {DisplayId}", key);
                results[key] = null;
            }
        }

        return results;
    }
}
