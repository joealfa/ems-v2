using EmployeeManagementSystem.ApiClient.Generated;
using EmployeeManagementSystem.Gateway.Caching;

namespace EmployeeManagementSystem.Gateway.DataLoaders;

public class SchoolDataLoader(
    EmsApiClient client,
    IRedisCacheService cache,
    ILogger<SchoolDataLoader> logger,
    IBatchScheduler batchScheduler,
    DataLoaderOptions? options = null) : BatchDataLoader<long, SchoolResponseDto?>(batchScheduler, options ?? new DataLoaderOptions())
{
    private readonly TimeSpan _cacheTtl = TimeSpan.FromMinutes(10);

    protected override async Task<IReadOnlyDictionary<long, SchoolResponseDto?>> LoadBatchAsync(
        IReadOnlyList<long> keys,
        CancellationToken ct)
    {
        Dictionary<long, SchoolResponseDto?> results = [];
        List<long> keysToFetch = [];

        // Check cache first
        foreach (long key in keys)
        {
            SchoolResponseDto? cached = await cache.GetAsync<SchoolResponseDto>(CacheKeys.School(key), ct);
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
                SchoolResponseDto? school = await client.SchoolsGET2Async(key, ct);
                results[key] = school;

                if (school is not null)
                {
                    await cache.SetAsync(CacheKeys.School(key), school, _cacheTtl, ct);
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to fetch school {DisplayId}", key);
                results[key] = null;
            }
        }

        return results;
    }
}
