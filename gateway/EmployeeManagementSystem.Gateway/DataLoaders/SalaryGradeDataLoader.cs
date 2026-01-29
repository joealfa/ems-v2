using EmployeeManagementSystem.ApiClient.Generated;
using EmployeeManagementSystem.Gateway.Caching;

namespace EmployeeManagementSystem.Gateway.DataLoaders;

public class SalaryGradeDataLoader : BatchDataLoader<long, SalaryGradeResponseDto?>
{
    private readonly EmsApiClient _client;
    private readonly IRedisCacheService _cache;
    private readonly ILogger<SalaryGradeDataLoader> _logger;
    private readonly TimeSpan _cacheTtl = TimeSpan.FromMinutes(10);

    public SalaryGradeDataLoader(
        EmsApiClient client,
        IRedisCacheService cache,
        ILogger<SalaryGradeDataLoader> logger,
        IBatchScheduler batchScheduler,
        DataLoaderOptions? options = null)
        : base(batchScheduler, options ?? new DataLoaderOptions())
    {
        _client = client;
        _cache = cache;
        _logger = logger;
    }

    protected override async Task<IReadOnlyDictionary<long, SalaryGradeResponseDto?>> LoadBatchAsync(
        IReadOnlyList<long> keys,
        CancellationToken ct)
    {
        Dictionary<long, SalaryGradeResponseDto?> results = [];
        List<long> keysToFetch = [];

        // Check cache first
        foreach (long key in keys)
        {
            SalaryGradeResponseDto? cached = await _cache.GetAsync<SalaryGradeResponseDto>(CacheKeys.SalaryGrade(key), ct);
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
                SalaryGradeResponseDto? salaryGrade = await _client.SalaryGradesGET2Async(key, ct);
                results[key] = salaryGrade;

                if (salaryGrade is not null)
                {
                    await _cache.SetAsync(CacheKeys.SalaryGrade(key), salaryGrade, _cacheTtl, ct);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to fetch salary grade {DisplayId}", key);
                results[key] = null;
            }
        }

        return results;
    }
}
