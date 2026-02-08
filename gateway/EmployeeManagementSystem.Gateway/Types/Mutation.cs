using EmployeeManagementSystem.ApiClient.Generated;
using EmployeeManagementSystem.Gateway.Caching;
using EmployeeManagementSystem.Gateway.Mappings;
using EmployeeManagementSystem.Gateway.Types.Inputs;
using System.Net.Http.Headers;

namespace EmployeeManagementSystem.Gateway.Types;

public class Mutation
{
    private static readonly System.Text.Json.JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    #region Person Mutations

    [GraphQLDescription("Create a new person")]
    public async Task<PersonResponseDto?> CreatePersonAsync(
        CreatePersonInput input,
        [Service] EmsApiClient client,
        [Service] IRedisCacheService cache,
        [Service] ILogger<Mutation> logger,
        CancellationToken ct)
    {
        logger.LogInformation("Creating person via GraphQL: {FirstName} {LastName}",
            input.FirstName, input.LastName);

        PersonResponseDto result = await client.PersonsPOSTAsync(input.ToDto(), ct);
        await cache.RemoveByPrefixAsync(CacheKeys.PersonsListPrefix, ct);
        await cache.RemoveAsync(CacheKeys.DashboardStats, ct);

        logger.LogInformation("Person created successfully: DisplayId {DisplayId}, Name: {FullName}",
            result.DisplayId, result.FullName);
        return result;
    }

    [GraphQLDescription("Update an existing person")]
    public async Task<PersonResponseDto?> UpdatePersonAsync(
        long displayId,
        UpdatePersonInput input,
        [Service] EmsApiClient client,
        [Service] IRedisCacheService cache,
        CancellationToken ct)
    {
        PersonResponseDto result = await client.PersonsPUTAsync(displayId, input.ToDto(), ct);
        await cache.RemoveAsync(CacheKeys.Person(displayId), ct);
        await cache.RemoveByPrefixAsync(CacheKeys.PersonsListPrefix, ct);
        return result;
    }

    [GraphQLDescription("Delete a person")]
    public async Task<bool> DeletePersonAsync(
        long displayId,
        [Service] EmsApiClient client,
        [Service] IRedisCacheService cache,
        [Service] ILogger<Mutation> logger,
        CancellationToken ct)
    {
        logger.LogInformation("Deleting person via GraphQL: DisplayId {DisplayId}", displayId);

        await client.PersonsDELETEAsync(displayId, ct);
        await cache.RemoveAsync(CacheKeys.Person(displayId), ct);
        await cache.RemoveByPrefixAsync(CacheKeys.PersonsListPrefix, ct);
        await cache.RemoveAsync(CacheKeys.DashboardStats, ct);

        logger.LogInformation("Person deleted successfully: DisplayId {DisplayId}", displayId);
        return true;
    }

    #endregion

    #region Employment Mutations

    [GraphQLDescription("Create a new employment")]
    public async Task<EmploymentResponseDto?> CreateEmploymentAsync(
        CreateEmploymentInput input,
        [Service] EmsApiClient client,
        [Service] IRedisCacheService cache,
        [Service] ILogger<Mutation> logger,
        CancellationToken ct)
    {
        logger.LogInformation("Creating employment via GraphQL: Person {PersonDisplayId}, Position {PositionDisplayId}",
            input.PersonDisplayId, input.PositionDisplayId);

        EmploymentResponseDto result = await client.EmploymentsPOSTAsync(input.ToDto(), ct);
        await cache.RemoveByPrefixAsync(CacheKeys.EmploymentsListPrefix, ct);
        await cache.RemoveAsync(CacheKeys.DashboardStats, ct);

        logger.LogInformation("Employment created successfully: DisplayId {DisplayId}, DepEdId {DepEdId}",
            result.DisplayId, result.DepEdId);
        return result;
    }

    [GraphQLDescription("Update an existing employment")]
    public async Task<EmploymentResponseDto?> UpdateEmploymentAsync(
        long displayId,
        UpdateEmploymentInput input,
        [Service] EmsApiClient client,
        [Service] IRedisCacheService cache,
        CancellationToken ct)
    {
        EmploymentResponseDto result = await client.EmploymentsPUTAsync(displayId, input.ToDto(), ct);
        await cache.RemoveAsync(CacheKeys.Employment(displayId), ct);
        await cache.RemoveByPrefixAsync(CacheKeys.EmploymentsListPrefix, ct);
        return result;
    }

    [GraphQLDescription("Delete an employment")]
    public async Task<bool> DeleteEmploymentAsync(
        long displayId,
        [Service] EmsApiClient client,
        [Service] IRedisCacheService cache,
        [Service] ILogger<Mutation> logger,
        CancellationToken ct)
    {
        logger.LogInformation("Deleting employment via GraphQL: DisplayId {DisplayId}", displayId);

        await client.EmploymentsDELETEAsync(displayId, ct);
        await cache.RemoveAsync(CacheKeys.Employment(displayId), ct);
        await cache.RemoveByPrefixAsync(CacheKeys.EmploymentsListPrefix, ct);
        await cache.RemoveAsync(CacheKeys.DashboardStats, ct);

        logger.LogInformation("Employment deleted successfully: DisplayId {DisplayId}", displayId);
        return true;
    }

    [GraphQLDescription("Add a school assignment to an employment")]
    public async Task<EmploymentSchoolResponseDto?> AddSchoolToEmploymentAsync(
        long employmentDisplayId,
        CreateEmploymentSchoolInput input,
        [Service] EmsApiClient client,
        [Service] IRedisCacheService cache,
        CancellationToken ct)
    {
        EmploymentSchoolResponseDto result = await client.SchoolsPOSTAsync(employmentDisplayId, input.ToDto(), ct);
        await cache.RemoveAsync(CacheKeys.Employment(employmentDisplayId), ct);
        await cache.RemoveByPrefixAsync(CacheKeys.EmploymentsListPrefix, ct);
        return result;
    }

    [GraphQLDescription("Remove a school assignment from an employment")]
    public async Task<bool> RemoveSchoolFromEmploymentAsync(
        long employmentDisplayId,
        long schoolAssignmentDisplayId,
        [Service] EmsApiClient client,
        [Service] IRedisCacheService cache,
        CancellationToken ct)
    {
        await client.SchoolsDELETEAsync(employmentDisplayId, schoolAssignmentDisplayId, ct);
        await cache.RemoveAsync(CacheKeys.Employment(employmentDisplayId), ct);
        await cache.RemoveByPrefixAsync(CacheKeys.EmploymentsListPrefix, ct);
        return true;
    }

    #endregion

    #region School Mutations

    [GraphQLDescription("Create a new school")]
    public async Task<SchoolResponseDto?> CreateSchoolAsync(
        CreateSchoolInput input,
        [Service] EmsApiClient client,
        [Service] IRedisCacheService cache,
        CancellationToken ct)
    {
        SchoolResponseDto result = await client.SchoolsPOSTAsync(input.ToDto(), ct);
        await cache.RemoveByPrefixAsync(CacheKeys.SchoolsListPrefix, ct);
        await cache.RemoveAsync(CacheKeys.DashboardStats, ct);
        return result;
    }

    [GraphQLDescription("Update an existing school")]
    public async Task<SchoolResponseDto?> UpdateSchoolAsync(
        long displayId,
        UpdateSchoolInput input,
        [Service] EmsApiClient client,
        [Service] IRedisCacheService cache,
        CancellationToken ct)
    {
        SchoolResponseDto result = await client.SchoolsPUTAsync(displayId, input.ToDto(), ct);
        await cache.RemoveAsync(CacheKeys.School(displayId), ct);
        await cache.RemoveByPrefixAsync(CacheKeys.SchoolsListPrefix, ct);
        return result;
    }

    [GraphQLDescription("Delete a school")]
    public async Task<bool> DeleteSchoolAsync(
        long displayId,
        [Service] EmsApiClient client,
        [Service] IRedisCacheService cache,
        CancellationToken ct)
    {
        await client.SchoolsDELETEAsync(displayId, ct);
        await cache.RemoveAsync(CacheKeys.School(displayId), ct);
        await cache.RemoveByPrefixAsync(CacheKeys.SchoolsListPrefix, ct);
        await cache.RemoveAsync(CacheKeys.DashboardStats, ct);
        return true;
    }

    #endregion

    #region Position Mutations

    [GraphQLDescription("Create a new position")]
    public async Task<PositionResponseDto?> CreatePositionAsync(
        CreatePositionInput input,
        [Service] EmsApiClient client,
        [Service] IRedisCacheService cache,
        CancellationToken ct)
    {
        PositionResponseDto result = await client.PositionsPOSTAsync(input.ToDto(), ct);
        await cache.RemoveByPrefixAsync(CacheKeys.PositionsListPrefix, ct);
        return result;
    }

    [GraphQLDescription("Update an existing position")]
    public async Task<PositionResponseDto?> UpdatePositionAsync(
        long displayId,
        UpdatePositionInput input,
        [Service] EmsApiClient client,
        [Service] IRedisCacheService cache,
        CancellationToken ct)
    {
        PositionResponseDto result = await client.PositionsPUTAsync(displayId, input.ToDto(), ct);
        await cache.RemoveAsync(CacheKeys.Position(displayId), ct);
        await cache.RemoveByPrefixAsync(CacheKeys.PositionsListPrefix, ct);
        return result;
    }

    [GraphQLDescription("Delete a position")]
    public async Task<bool> DeletePositionAsync(
        long displayId,
        [Service] EmsApiClient client,
        [Service] IRedisCacheService cache,
        CancellationToken ct)
    {
        await client.PositionsDELETEAsync(displayId, ct);
        await cache.RemoveAsync(CacheKeys.Position(displayId), ct);
        await cache.RemoveByPrefixAsync(CacheKeys.PositionsListPrefix, ct);
        return true;
    }

    #endregion

    #region SalaryGrade Mutations

    [GraphQLDescription("Create a new salary grade")]
    public async Task<SalaryGradeResponseDto?> CreateSalaryGradeAsync(
        CreateSalaryGradeInput input,
        [Service] EmsApiClient client,
        [Service] IRedisCacheService cache,
        CancellationToken ct)
    {
        SalaryGradeResponseDto result = await client.SalaryGradesPOSTAsync(input.ToDto(), ct);
        await cache.RemoveByPrefixAsync(CacheKeys.SalaryGradesListPrefix, ct);
        return result;
    }

    [GraphQLDescription("Update an existing salary grade")]
    public async Task<SalaryGradeResponseDto?> UpdateSalaryGradeAsync(
        long displayId,
        UpdateSalaryGradeInput input,
        [Service] EmsApiClient client,
        [Service] IRedisCacheService cache,
        CancellationToken ct)
    {
        SalaryGradeResponseDto result = await client.SalaryGradesPUTAsync(displayId, input.ToDto(), ct);
        await cache.RemoveAsync(CacheKeys.SalaryGrade(displayId), ct);
        await cache.RemoveByPrefixAsync(CacheKeys.SalaryGradesListPrefix, ct);
        return result;
    }

    [GraphQLDescription("Delete a salary grade")]
    public async Task<bool> DeleteSalaryGradeAsync(
        long displayId,
        [Service] EmsApiClient client,
        [Service] IRedisCacheService cache,
        CancellationToken ct)
    {
        await client.SalaryGradesDELETEAsync(displayId, ct);
        await cache.RemoveAsync(CacheKeys.SalaryGrade(displayId), ct);
        await cache.RemoveByPrefixAsync(CacheKeys.SalaryGradesListPrefix, ct);
        return true;
    }

    #endregion

    #region Item Mutations

    [GraphQLDescription("Create a new item")]
    public async Task<ItemResponseDto?> CreateItemAsync(
        CreateItemInput input,
        [Service] EmsApiClient client,
        [Service] IRedisCacheService cache,
        CancellationToken ct)
    {
        ItemResponseDto result = await client.ItemsPOSTAsync(input.ToDto(), ct);
        await cache.RemoveByPrefixAsync(CacheKeys.ItemsListPrefix, ct);
        return result;
    }

    [GraphQLDescription("Update an existing item")]
    public async Task<ItemResponseDto?> UpdateItemAsync(
        long displayId,
        UpdateItemInput input,
        [Service] EmsApiClient client,
        [Service] IRedisCacheService cache,
        CancellationToken ct)
    {
        ItemResponseDto result = await client.ItemsPUTAsync(displayId, input.ToDto(), ct);
        await cache.RemoveAsync(CacheKeys.Item(displayId), ct);
        await cache.RemoveByPrefixAsync(CacheKeys.ItemsListPrefix, ct);
        return result;
    }

    [GraphQLDescription("Delete an item")]
    public async Task<bool> DeleteItemAsync(
        long displayId,
        [Service] EmsApiClient client,
        [Service] IRedisCacheService cache,
        CancellationToken ct)
    {
        await client.ItemsDELETEAsync(displayId, ct);
        await cache.RemoveAsync(CacheKeys.Item(displayId), ct);
        await cache.RemoveByPrefixAsync(CacheKeys.ItemsListPrefix, ct);
        return true;
    }

    #endregion

    #region Document Mutations

    [GraphQLDescription("Upload a document for a person")]
    public async Task<DocumentResponseDto?> UploadDocumentAsync(
        long personDisplayId,
        IFile file,
        string? description,
        [Service] IHttpClientFactory httpClientFactory,
        [Service] IConfiguration configuration,
        [Service] IHttpContextAccessor httpContextAccessor,
        [Service] IRedisCacheService cache,
        [Service] ILogger<Mutation> logger,
        CancellationToken ct)
    {
        logger.LogInformation("Uploading document via GraphQL for person {PersonDisplayId}: {FileName} ({Length} bytes)",
            personDisplayId, file.Name, file.Length);

        using HttpClient client = CreateApiClient(httpClientFactory, configuration, httpContextAccessor);
        using MultipartFormDataContent content = [];

        using Stream fileStream = file.OpenReadStream();
        using StreamContent streamContent = new(fileStream);
        streamContent.Headers.ContentType = MediaTypeHeaderValue.Parse(
            string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream" : file.ContentType);
        content.Add(streamContent, "file", file.Name);

        if (!string.IsNullOrEmpty(description))
        {
            content.Add(new StringContent(description), "description");
        }

        string apiBaseUrl = configuration["ApiClient:BaseUrl"] ?? "https://localhost:7166";
        HttpResponseMessage response = await client.PostAsync(
            $"{apiBaseUrl}/api/v1/Persons/{personDisplayId}/Documents",
            content,
            ct);

        string responseBody = await response.Content.ReadAsStringAsync(ct);
        if (!response.IsSuccessStatusCode)
        {
            logger.LogWarning("Document upload failed for person {PersonDisplayId}: {StatusCode} {ReasonPhrase}",
                personDisplayId, response.StatusCode, response.ReasonPhrase);
            throw new GraphQLException(
                $"Backend upload failed ({(int)response.StatusCode} {response.ReasonPhrase}). {responseBody}");
        }

        // Some backends return 201 with an empty body or a non-JSON body even on success.
        // Avoid throwing (which Apollo logs as a GraphQL error) when the upload actually succeeded.
        DocumentResponseDto? result = null;
        if (!string.IsNullOrWhiteSpace(responseBody))
        {
            try
            {
                result = System.Text.Json.JsonSerializer.Deserialize<DocumentResponseDto>(responseBody, JsonOptions);
            }
            catch (System.Text.Json.JsonException)
            {
                result = null;
            }
        }

        if (result is null && response.Headers.Location is Uri location)
        {
            string locationUrl = location.IsAbsoluteUri
                ? location.ToString()
                : $"{apiBaseUrl.TrimEnd('/')}{location}";

            using HttpResponseMessage followUp = await client.GetAsync(locationUrl, ct);
            if (followUp.IsSuccessStatusCode)
            {
                string followUpBody = await followUp.Content.ReadAsStringAsync(ct);
                if (!string.IsNullOrWhiteSpace(followUpBody))
                {
                    try
                    {
                        result = System.Text.Json.JsonSerializer.Deserialize<DocumentResponseDto>(followUpBody, JsonOptions);
                    }
                    catch (System.Text.Json.JsonException)
                    {
                        result = null;
                    }
                }
            }
        }

        logger.LogInformation("Document uploaded successfully for person {PersonDisplayId}: {FileName}",
            personDisplayId, file.Name);

        await cache.RemoveAsync(CacheKeys.Person(personDisplayId), ct);
        return result;
    }

    [GraphQLDescription("Delete a person's profile image")]
    public async Task<bool> DeleteProfileImageAsync(
        long personDisplayId,
        [Service] EmsApiClient client,
        [Service] IRedisCacheService cache,
        [Service] ILogger<Mutation> logger,
        CancellationToken ct)
    {
        logger.LogInformation("Deleting profile image via GraphQL for person {PersonDisplayId}", personDisplayId);

        await client.ProfileImageDELETEAsync(personDisplayId, ct);

        await cache.RemoveAsync(CacheKeys.Person(personDisplayId), ct);
        await cache.RemoveByPrefixAsync(CacheKeys.PersonsListPrefix, ct);

        logger.LogInformation("Profile image deleted successfully for person {PersonDisplayId}", personDisplayId);
        return true;
    }

    [GraphQLDescription("Update document metadata")]
    public async Task<DocumentResponseDto?> UpdateDocumentAsync(
        long personDisplayId,
        long documentDisplayId,
        UpdateDocumentInput input,
        [Service] EmsApiClient client,
        [Service] IRedisCacheService cache,
        CancellationToken ct)
    {
        DocumentResponseDto result = await client.DocumentsPUTAsync(
            personDisplayId,
            documentDisplayId,
            new UpdateDocumentDto { Description = input.Description },
            ct);

        await cache.RemoveAsync(CacheKeys.Person(personDisplayId), ct);
        return result;
    }

    [GraphQLDescription("Delete a document")]
    public async Task<bool> DeleteDocumentAsync(
        long personDisplayId,
        long documentDisplayId,
        [Service] EmsApiClient client,
        [Service] IRedisCacheService cache,
        [Service] ILogger<Mutation> logger,
        CancellationToken ct)
    {
        logger.LogInformation("Deleting document via GraphQL: Person {PersonDisplayId}, Document {DocumentDisplayId}",
            personDisplayId, documentDisplayId);

        await client.DocumentsDELETEAsync(personDisplayId, documentDisplayId, ct);
        await cache.RemoveAsync(CacheKeys.Person(personDisplayId), ct);

        logger.LogInformation("Document deleted successfully: Person {PersonDisplayId}, Document {DocumentDisplayId}",
            personDisplayId, documentDisplayId);
        return true;
    }

    [GraphQLDescription("Upload a profile image for a person")]
    public async Task<string?> UploadProfileImageAsync(
        long personDisplayId,
        IFile file,
        [Service] IHttpClientFactory httpClientFactory,
        [Service] IConfiguration configuration,
        [Service] IHttpContextAccessor httpContextAccessor,
        [Service] IRedisCacheService cache,
        [Service] ILogger<Mutation> logger,
        CancellationToken ct)
    {
        logger.LogInformation("Uploading profile image via GraphQL for person {PersonDisplayId}: {FileName} ({Length} bytes)",
            personDisplayId, file.Name, file.Length);

        using HttpClient client = CreateApiClient(httpClientFactory, configuration, httpContextAccessor);
        using MultipartFormDataContent content = [];

        using Stream fileStream = file.OpenReadStream();
        using StreamContent streamContent = new(fileStream);
        streamContent.Headers.ContentType = MediaTypeHeaderValue.Parse(
            string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream" : file.ContentType);
        content.Add(streamContent, "file", file.Name);

        string apiBaseUrl = configuration["ApiClient:BaseUrl"] ?? "https://localhost:7166";
        HttpResponseMessage response = await client.PostAsync(
            $"{apiBaseUrl}/api/v1/Persons/{personDisplayId}/Documents/profile-image",
            content,
            ct);

        string result = await response.Content.ReadAsStringAsync(ct);
        if (!response.IsSuccessStatusCode)
        {
            logger.LogWarning("Profile image upload failed for person {PersonDisplayId}: {StatusCode} {ReasonPhrase}",
                personDisplayId, response.StatusCode, response.ReasonPhrase);
            throw new GraphQLException(
                $"Backend profile image upload failed ({(int)response.StatusCode} {response.ReasonPhrase}). {result}");
        }

        logger.LogInformation("Profile image uploaded successfully for person {PersonDisplayId}", personDisplayId);

        // Invalidate both individual person cache and persons list cache
        await cache.RemoveAsync(CacheKeys.Person(personDisplayId), ct);
        await cache.RemoveByPrefixAsync(CacheKeys.PersonsListPrefix, ct);

        return result.Trim('"');
    }

    private static HttpClient CreateApiClient(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor)
    {
        HttpClient client = httpClientFactory.CreateClient("EmsApiClient");

        // Forward the Authorization header from the incoming request
        string? authHeader = httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString();
        if (!string.IsNullOrEmpty(authHeader))
        {
            _ = client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authHeader);
        }

        return client;
    }

    #endregion

    #region Auth Mutations

    [GraphQLDescription("Login with Google ID token")]
    public async Task<AuthResponseDto?> GoogleLoginAsync(
        string idToken,
        [Service] EmsApiClient client,
        [Service] ILogger<Mutation> logger,
        CancellationToken ct)
    {
        logger.LogInformation("Google login attempt via GraphQL");

        AuthResponseDto? result = await client.GoogleAsync(
            new GoogleAuthRequestDto { IdToken = idToken },
            ct);

        if (result != null)
        {
            logger.LogInformation("Google login successful for user {UserId}", result.User.Id);
        }

        return result;
    }

    [GraphQLDescription("Login with Google access token")]
    public async Task<AuthResponseDto?> GoogleTokenLoginAsync(
        string accessToken,
        [Service] EmsApiClient client,
        [Service] ILogger<Mutation> logger,
        CancellationToken ct)
    {
        logger.LogInformation("Google token login attempt via GraphQL");

        AuthResponseDto? result = await client.TokenAsync(
            new GoogleAccessTokenRequestDto { AccessToken = accessToken },
            ct);

        if (result != null)
        {
            logger.LogInformation("Google token login successful for user {UserId}", result.User.Id);
        }

        return result;
    }

    [GraphQLDescription("Refresh authentication token")]
    public async Task<AuthResponseDto?> RefreshTokenAsync(
        string? refreshToken,
        [Service] EmsApiClient client,
        [Service] ILogger<Mutation> logger,
        CancellationToken ct)
    {
        logger.LogDebug("Token refresh attempt via GraphQL");

        try
        {
            AuthResponseDto? result = await client.RefreshAsync(
                new RefreshTokenRequestDto { RefreshToken = refreshToken },
                ct);

            if (result != null)
            {
                logger.LogInformation("Token refreshed successfully for user {UserId}", result.User.Id);
            }

            return result;
        }
        catch (ApiException ex) when (ex.StatusCode == 401)
        {
            logger.LogWarning("Token refresh failed: Invalid or expired refresh token");
            // Treat invalid/expired/missing refresh token as a non-error for GraphQL.
            // The frontend will interpret null and clear local auth state.
            return null;
        }
    }

    [GraphQLDescription("Logout and revoke tokens")]
    public async Task<bool> LogoutAsync(
        string? refreshToken,
        [Service] EmsApiClient client,
        [Service] ILogger<Mutation> logger,
        CancellationToken ct)
    {
        logger.LogInformation("Logout attempt via GraphQL");

        try
        {
            await client.RevokeAsync(
                new RevokeTokenRequestDto { RefreshToken = refreshToken },
                ct);
            logger.LogInformation("Logout successful - tokens revoked");
            return true;
        }
        catch (ApiException ex) when (ex.StatusCode is 400 or 401)
        {
            logger.LogDebug("Logout completed - token was already invalid/expired");
            // If the token is already invalid/expired, consider logout successful.
            return true;
        }
    }

    #endregion
}
