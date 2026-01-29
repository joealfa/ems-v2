using EmployeeManagementSystem.ApiClient.Generated;
using EmployeeManagementSystem.Gateway.Caching;

namespace EmployeeManagementSystem.Gateway.DataLoaders;

public class EmploymentDataLoader : BatchDataLoader<long, EmploymentResponseDto?>
{
    private readonly EmsApiClient _client;
    private readonly IRedisCacheService _cache;
    private readonly ILogger<EmploymentDataLoader> _logger;
    private readonly TimeSpan _cacheTtl = TimeSpan.FromMinutes(10);

    public EmploymentDataLoader(
        EmsApiClient client,
        IRedisCacheService cache,
        ILogger<EmploymentDataLoader> logger,
        IBatchScheduler batchScheduler,
        DataLoaderOptions? options = null)
        : base(batchScheduler, options ?? new DataLoaderOptions())
    {
        _client = client;
        _cache = cache;
        _logger = logger;
    }

    protected override async Task<IReadOnlyDictionary<long, EmploymentResponseDto?>> LoadBatchAsync(
        IReadOnlyList<long> keys,
        CancellationToken ct)
    {
        Dictionary<long, EmploymentResponseDto?> results = [];
        List<long> keysToFetch = [];

        // Check cache first
        foreach (long key in keys)
        {
            EmploymentResponseDto? cached = await _cache.GetAsync<EmploymentResponseDto>(CacheKeys.Employment(key), ct);
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
                EmploymentResponseDto? employment = await _client.EmploymentsGET2Async(key, ct);
                results[key] = employment;

                if (employment is not null)
                {
                    await _cache.SetAsync(CacheKeys.Employment(key), employment, _cacheTtl, ct);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to fetch employment {DisplayId}", key);
                results[key] = null;
            }
        }

        return results;
    }
}
