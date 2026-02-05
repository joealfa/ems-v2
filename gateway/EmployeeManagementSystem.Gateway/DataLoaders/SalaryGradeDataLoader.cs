using EmployeeManagementSystem.ApiClient.Generated;
using EmployeeManagementSystem.Gateway.Caching;

namespace EmployeeManagementSystem.Gateway.DataLoaders;

public class SalaryGradeDataLoader(
    EmsApiClient client,
    IRedisCacheService cache,
    ILogger<SalaryGradeDataLoader> logger,
    IBatchScheduler batchScheduler,
    DataLoaderOptions? options = null) : BatchDataLoader<long, SalaryGradeResponseDto?>(batchScheduler, options ?? new DataLoaderOptions())
{
    private readonly TimeSpan _cacheTtl = TimeSpan.FromMinutes(10);

    protected override async Task<IReadOnlyDictionary<long, SalaryGradeResponseDto?>> LoadBatchAsync(
        IReadOnlyList<long> keys,
        CancellationToken ct)
    {
        Dictionary<long, SalaryGradeResponseDto?> results = [];
        List<long> keysToFetch = [];

        // Check cache first
        foreach (long key in keys)
        {
            SalaryGradeResponseDto? cached = await cache.GetAsync<SalaryGradeResponseDto>(CacheKeys.SalaryGrade(key), ct);
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
                SalaryGradeResponseDto? salaryGrade = await client.SalaryGradesGET2Async(key, ct);
                results[key] = salaryGrade;

                if (salaryGrade is not null)
                {
                    await cache.SetAsync(CacheKeys.SalaryGrade(key), salaryGrade, _cacheTtl, ct);
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to fetch salary grade {DisplayId}", key);
                results[key] = null;
            }
        }

        return results;
    }
}
