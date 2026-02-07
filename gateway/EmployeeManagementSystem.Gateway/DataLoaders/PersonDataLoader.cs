using EmployeeManagementSystem.ApiClient.Generated;
using EmployeeManagementSystem.Gateway.Caching;
using Microsoft.Extensions.Configuration;

namespace EmployeeManagementSystem.Gateway.DataLoaders;

public class PersonDataLoader(
    EmsApiClient client,
    IRedisCacheService cache,
    IConfiguration configuration,
    ILogger<PersonDataLoader> logger,
    IBatchScheduler batchScheduler,
    DataLoaderOptions? options = null) : BatchDataLoader<long, PersonResponseDto?>(batchScheduler, options ?? new DataLoaderOptions())
{
    private readonly TimeSpan _cacheTtl = TimeSpan.FromMinutes(10);

    protected override async Task<IReadOnlyDictionary<long, PersonResponseDto?>> LoadBatchAsync(
        IReadOnlyList<long> keys,
        CancellationToken ct)
    {
        Dictionary<long, PersonResponseDto?> results = [];
        List<long> keysToFetch = [];

        // Check cache first
        foreach (long key in keys)
        {
            PersonResponseDto? cached = await cache.GetAsync<PersonResponseDto>(CacheKeys.Person(key), ct);
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
        string gatewayBaseUrl = configuration["Gateway:BaseUrl"] ?? "https://localhost:5003";
        if (!gatewayBaseUrl.StartsWith("http://") && !gatewayBaseUrl.StartsWith("https://"))
        {
            gatewayBaseUrl = $"https://{gatewayBaseUrl}";
        }

        foreach (long key in keysToFetch)
        {
            try
            {
                PersonResponseDto? person = await client.PersonsGET2Async(key, ct);
                
                // Transform profileImageUrl to Gateway proxy URL
                if (person is not null && person.HasProfileImage)
                {
                    person.ProfileImageUrl = $"{gatewayBaseUrl}/api/persons/{person.DisplayId}/profile-image";
                }
                
                results[key] = person;

                if (person is not null)
                {
                    await cache.SetAsync(CacheKeys.Person(key), person, _cacheTtl, ct);
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to fetch person {DisplayId}", key);
                results[key] = null;
            }
        }

        return results;
    }
}
