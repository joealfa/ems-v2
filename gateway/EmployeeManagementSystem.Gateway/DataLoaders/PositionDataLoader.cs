using EmployeeManagementSystem.ApiClient.Generated;
using EmployeeManagementSystem.Gateway.Caching;

namespace EmployeeManagementSystem.Gateway.DataLoaders;

public class PositionDataLoader : BatchDataLoader<long, PositionResponseDto?>
{
    private readonly EmsApiClient _client;
    private readonly IRedisCacheService _cache;
    private readonly ILogger<PositionDataLoader> _logger;
    private readonly TimeSpan _cacheTtl = TimeSpan.FromMinutes(10);

    public PositionDataLoader(
        EmsApiClient client,
        IRedisCacheService cache,
        ILogger<PositionDataLoader> logger,
        IBatchScheduler batchScheduler,
        DataLoaderOptions? options = null)
        : base(batchScheduler, options ?? new DataLoaderOptions())
    {
        _client = client;
        _cache = cache;
        _logger = logger;
    }

    protected override async Task<IReadOnlyDictionary<long, PositionResponseDto?>> LoadBatchAsync(
        IReadOnlyList<long> keys,
        CancellationToken ct)
    {
        Dictionary<long, PositionResponseDto?> results = [];
        List<long> keysToFetch = [];

        // Check cache first
        foreach (long key in keys)
        {
            PositionResponseDto? cached = await _cache.GetAsync<PositionResponseDto>(CacheKeys.Position(key), ct);
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
                PositionResponseDto? position = await _client.PositionsGET2Async(key, ct);
                results[key] = position;

                if (position is not null)
                {
                    await _cache.SetAsync(CacheKeys.Position(key), position, _cacheTtl, ct);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to fetch position {DisplayId}", key);
                results[key] = null;
            }
        }

        return results;
    }
}
