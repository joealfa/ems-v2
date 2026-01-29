using EmployeeManagementSystem.ApiClient.Generated;
using EmployeeManagementSystem.Gateway.Caching;

namespace EmployeeManagementSystem.Gateway.DataLoaders;

public class PersonDataLoader : BatchDataLoader<long, PersonResponseDto?>
{
    private readonly EmsApiClient _client;
    private readonly IRedisCacheService _cache;
    private readonly ILogger<PersonDataLoader> _logger;
    private readonly TimeSpan _cacheTtl = TimeSpan.FromMinutes(10);

    public PersonDataLoader(
        EmsApiClient client,
        IRedisCacheService cache,
        ILogger<PersonDataLoader> logger,
        IBatchScheduler batchScheduler,
        DataLoaderOptions? options = null)
        : base(batchScheduler, options ?? new DataLoaderOptions())
    {
        _client = client;
        _cache = cache;
        _logger = logger;
    }

    protected override async Task<IReadOnlyDictionary<long, PersonResponseDto?>> LoadBatchAsync(
        IReadOnlyList<long> keys,
        CancellationToken ct)
    {
        Dictionary<long, PersonResponseDto?> results = [];
        List<long> keysToFetch = [];

        // Check cache first
        foreach (long key in keys)
        {
            PersonResponseDto? cached = await _cache.GetAsync<PersonResponseDto>(CacheKeys.Person(key), ct);
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
                PersonResponseDto? person = await _client.PersonsGET2Async(key, ct);
                results[key] = person;

                if (person is not null)
                {
                    await _cache.SetAsync(CacheKeys.Person(key), person, _cacheTtl, ct);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to fetch person {DisplayId}", key);
                results[key] = null;
            }
        }

        return results;
    }
}
