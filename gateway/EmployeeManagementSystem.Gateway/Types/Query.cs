using EmployeeManagementSystem.ApiClient.Generated;
using EmployeeManagementSystem.Gateway.Caching;
using EmployeeManagementSystem.Gateway.DataLoaders;

namespace EmployeeManagementSystem.Gateway.Types;

public class Query
{
    #region Person Queries

    [GraphQLDescription("Get a single person by display ID")]
    public async Task<PersonResponseDto?> GetPersonAsync(
        long displayId,
        PersonDataLoader dataLoader,
        CancellationToken ct)
    {
        return await dataLoader.LoadAsync(displayId, ct);
    }

    [GraphQLDescription("Get paginated list of persons with optional filtering")]
    public async Task<PagedResultOfPersonListDto?> GetPersonsAsync(
        int? pageNumber,
        int? pageSize,
        string? searchTerm,
        string? fullNameFilter,
        string? displayIdFilter,
        string? gender,
        string? civilStatus,
        string? sortBy,
        bool? sortDescending,
        [Service] EmsApiClient client,
        [Service] IRedisCacheService cache,
        [Service] IConfiguration configuration,
        CancellationToken ct)
    {
        string cacheKey = CacheKeys.PersonsList(
            pageNumber,
            pageSize,
            searchTerm,
            fullNameFilter,
            displayIdFilter,
            gender,
            civilStatus,
            sortBy,
            sortDescending);

        // Try to get from cache first
        PagedResultOfPersonListDto? result = await cache.GetAsync<PagedResultOfPersonListDto>(cacheKey, ct);

        if (result is null)
        {
            // Fetch from API
            result = await client.PersonsGETAsync(
                gender != null ? Enum.Parse<Gender>(gender, ignoreCase: true) : null,
                civilStatus != null ? Enum.Parse<CivilStatus>(civilStatus, ignoreCase: true) : null,
                displayIdFilter,
                fullNameFilter,
                pageNumber,
                pageSize,
                searchTerm,
                sortBy,
                sortDescending,
                ct);

            // Transform profileImageUrl to Gateway proxy URL BEFORE caching
            if (result?.Items != null)
            {
                string gatewayBaseUrl = configuration["Gateway:BaseUrl"] ?? "https://localhost:5003";
                if (!gatewayBaseUrl.StartsWith("http://") && !gatewayBaseUrl.StartsWith("https://"))
                {
                    gatewayBaseUrl = $"https://{gatewayBaseUrl}";
                }

                foreach (PersonListDto person in result.Items)
                {
                    if (person.HasProfileImage)
                    {
                        person.ProfileImageUrl = $"{gatewayBaseUrl}/api/persons/{person.DisplayId}/profile-image";
                    }
                }
            }

            // Cache the transformed data
            if (result is not null)
            {
                await cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(1), ct);
            }
        }

        return result;
    }

    #endregion

    #region Employment Queries

    [GraphQLDescription("Get a single employment by display ID")]
    public async Task<EmploymentResponseDto?> GetEmploymentAsync(
        long displayId,
        EmploymentDataLoader dataLoader,
        CancellationToken ct)
    {
        return await dataLoader.LoadAsync(displayId, ct);
    }

    [GraphQLDescription("Get paginated list of employments with optional filtering")]
    public async Task<PagedResultOfEmploymentListDto?> GetEmploymentsAsync(
        int? pageNumber,
        int? pageSize,
        string? searchTerm,
        string? displayIdFilter,
        string? employeeNameFilter,
        string? positionFilter,
        string? depEdIdFilter,
        string? employmentStatus,
        bool? isActive,
        string? sortBy,
        bool? sortDescending,
        [Service] EmsApiClient client,
        [Service] IRedisCacheService cache,
        CancellationToken ct)
    {
        string cacheKey = CacheKeys.EmploymentsList(
            pageNumber,
            pageSize,
            searchTerm,
            displayIdFilter,
            employeeNameFilter,
            positionFilter,
            depEdIdFilter,
            employmentStatus,
            isActive,
            sortBy,
            sortDescending);

        return await cache.GetOrSetAsync(
            cacheKey,
            async token => await client.EmploymentsGETAsync(
                employmentStatus != null ? Enum.Parse<EmploymentStatus>(employmentStatus, ignoreCase: true) : null,
                isActive,
                displayIdFilter,
                employeeNameFilter,
                depEdIdFilter,
                positionFilter,
                pageNumber,
                pageSize,
                searchTerm,
                sortBy,
                sortDescending,
                token),
            TimeSpan.FromMinutes(2),
            ct);
    }

    #endregion

    #region School Queries

    [GraphQLDescription("Get a single school by display ID")]
    public async Task<SchoolResponseDto?> GetSchoolAsync(
        long displayId,
        SchoolDataLoader dataLoader,
        CancellationToken ct)
    {
        return await dataLoader.LoadAsync(displayId, ct);
    }

    [GraphQLDescription("Get paginated list of schools with optional filtering")]
    public async Task<PagedResultOfSchoolListDto?> GetSchoolsAsync(
        int? pageNumber,
        int? pageSize,
        string? searchTerm,
        string? sortBy,
        bool? sortDescending,
        [Service] EmsApiClient client,
        [Service] IRedisCacheService cache,
        CancellationToken ct)
    {
        string cacheKey = CacheKeys.SchoolsList(
            pageNumber,
            pageSize,
            searchTerm,
            sortBy,
            sortDescending);

        return await cache.GetOrSetAsync(
            cacheKey,
            async token => await client.SchoolsGETAsync(
                pageNumber,
                pageSize,
                searchTerm,
                sortBy,
                sortDescending,
                token),
            TimeSpan.FromMinutes(2),
            ct);
    }

    #endregion

    #region Position Queries

    [GraphQLDescription("Get a single position by display ID")]
    public async Task<PositionResponseDto?> GetPositionAsync(
        long displayId,
        PositionDataLoader dataLoader,
        CancellationToken ct)
    {
        return await dataLoader.LoadAsync(displayId, ct);
    }

    [GraphQLDescription("Get paginated list of positions with optional filtering")]
    public async Task<PagedResultOfPositionResponseDto?> GetPositionsAsync(
        int? pageNumber,
        int? pageSize,
        string? searchTerm,
        string? sortBy,
        bool? sortDescending,
        [Service] EmsApiClient client,
        [Service] IRedisCacheService cache,
        CancellationToken ct)
    {
        string cacheKey = CacheKeys.PositionsList(
            pageNumber,
            pageSize,
            searchTerm,
            sortBy,
            sortDescending);

        return await cache.GetOrSetAsync(
            cacheKey,
            async token => await client.PositionsGETAsync(
                pageNumber,
                pageSize,
                searchTerm,
                sortBy,
                sortDescending,
                token),
            TimeSpan.FromMinutes(2),
            ct);
    }

    #endregion

    #region SalaryGrade Queries

    [GraphQLDescription("Get a single salary grade by display ID")]
    public async Task<SalaryGradeResponseDto?> GetSalaryGradeAsync(
        long displayId,
        SalaryGradeDataLoader dataLoader,
        CancellationToken ct)
    {
        return await dataLoader.LoadAsync(displayId, ct);
    }

    [GraphQLDescription("Get paginated list of salary grades with optional filtering")]
    public async Task<PagedResultOfSalaryGradeResponseDto?> GetSalaryGradesAsync(
        int? pageNumber,
        int? pageSize,
        string? searchTerm,
        string? sortBy,
        bool? sortDescending,
        [Service] EmsApiClient client,
        [Service] IRedisCacheService cache,
        CancellationToken ct)
    {
        string cacheKey = CacheKeys.SalaryGradesList(
            pageNumber,
            pageSize,
            searchTerm,
            sortBy,
            sortDescending);

        return await cache.GetOrSetAsync(
            cacheKey,
            async token => await client.SalaryGradesGETAsync(
                pageNumber,
                pageSize,
                searchTerm,
                sortBy,
                sortDescending,
                token),
            TimeSpan.FromMinutes(2),
            ct);
    }

    #endregion

    #region Item Queries

    [GraphQLDescription("Get a single item by display ID")]
    public async Task<ItemResponseDto?> GetItemAsync(
        long displayId,
        ItemDataLoader dataLoader,
        CancellationToken ct)
    {
        return await dataLoader.LoadAsync(displayId, ct);
    }

    [GraphQLDescription("Get paginated list of items with optional filtering")]
    public async Task<PagedResultOfItemResponseDto?> GetItemsAsync(
        int? pageNumber,
        int? pageSize,
        string? searchTerm,
        string? sortBy,
        bool? sortDescending,
        [Service] EmsApiClient client,
        [Service] IRedisCacheService cache,
        CancellationToken ct)
    {
        string cacheKey = CacheKeys.ItemsList(
            pageNumber,
            pageSize,
            searchTerm,
            sortBy,
            sortDescending);

        return await cache.GetOrSetAsync(
            cacheKey,
            async token => await client.ItemsGETAsync(
                pageNumber,
                pageSize,
                searchTerm,
                sortBy,
                sortDescending,
                token),
            TimeSpan.FromMinutes(2),
            ct);
    }

    #endregion

    #region Document Queries

    [GraphQLDescription("Get paginated list of documents for a person")]
    public async Task<PagedResultOfDocumentListDto?> GetPersonDocumentsAsync(
        long personDisplayId,
        int? pageNumber,
        int? pageSize,
        string? searchTerm,
        string? sortBy,
        bool? sortDescending,
        [Service] EmsApiClient client,
        CancellationToken ct)
    {
        return await client.DocumentsGETAsync(
            personDisplayId,
            pageNumber,
            pageSize,
            searchTerm,
            sortBy,
            sortDescending,
            ct);
    }

    [GraphQLDescription("Get a single document by display ID")]
    public async Task<DocumentResponseDto?> GetDocumentAsync(
        long personDisplayId,
        long documentDisplayId,
        [Service] EmsApiClient client,
        CancellationToken ct)
    {
        return await client.DocumentsGET2Async(personDisplayId, documentDisplayId, ct);
    }

    [GraphQLDescription("Get profile image URL for a person")]
    public string GetProfileImageUrl(
        long personDisplayId,
        [Service] IConfiguration configuration)
    {
        // Return Gateway proxy URL instead of direct backend URL
        // This ensures proper CORS handling and consistent API access
        string gatewayBaseUrl = configuration["Gateway:BaseUrl"] ?? "https://localhost:5003";

        // Ensure the base URL has a protocol
        if (!gatewayBaseUrl.StartsWith("http://") && !gatewayBaseUrl.StartsWith("https://"))
        {
            gatewayBaseUrl = $"https://localhost{gatewayBaseUrl}";
        }

        return $"{gatewayBaseUrl}/api/persons/{personDisplayId}/profile-image";
    }

    #endregion

    #region Dashboard & User Queries

    [GraphQLDescription("Get dashboard statistics")]
    public async Task<DashboardStatsDto?> GetDashboardStatsAsync(
        [Service] EmsApiClient client,
        [Service] IRedisCacheService cache,
        CancellationToken ct)
    {
        return await cache.GetOrSetAsync(
            CacheKeys.DashboardStats,
            async token => await client.DashboardAsync(token),
            TimeSpan.FromMinutes(1),
            ct);
    }

    [GraphQLDescription("Get current authenticated user")]
    public async Task<UserDto?> GetCurrentUserAsync(
        [Service] EmsApiClient client,
        CancellationToken ct)
    {
        return await client.MeAsync(ct);
    }

    #endregion
}
