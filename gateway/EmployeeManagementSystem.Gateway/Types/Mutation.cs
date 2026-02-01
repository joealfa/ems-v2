using EmployeeManagementSystem.ApiClient.Generated;
using EmployeeManagementSystem.Gateway.Caching;
using EmployeeManagementSystem.Gateway.Mappings;
using EmployeeManagementSystem.Gateway.Types.Inputs;

namespace EmployeeManagementSystem.Gateway.Types;

public class Mutation
{
    #region Person Mutations

    [GraphQLDescription("Create a new person")]
    public async Task<PersonResponseDto?> CreatePersonAsync(
        CreatePersonInput input,
        [Service] EmsApiClient client,
        [Service] IRedisCacheService cache,
        CancellationToken ct)
    {
        PersonResponseDto result = await client.PersonsPOSTAsync(input.ToDto(), ct);
        await cache.RemoveByPrefixAsync(CacheKeys.PersonsListPrefix, ct);
        await cache.RemoveAsync(CacheKeys.DashboardStats, ct);
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
        CancellationToken ct)
    {
        await client.PersonsDELETEAsync(displayId, ct);
        await cache.RemoveAsync(CacheKeys.Person(displayId), ct);
        await cache.RemoveByPrefixAsync(CacheKeys.PersonsListPrefix, ct);
        await cache.RemoveAsync(CacheKeys.DashboardStats, ct);
        return true;
    }

    #endregion

    #region Employment Mutations

    [GraphQLDescription("Create a new employment")]
    public async Task<EmploymentResponseDto?> CreateEmploymentAsync(
        CreateEmploymentInput input,
        [Service] EmsApiClient client,
        [Service] IRedisCacheService cache,
        CancellationToken ct)
    {
        EmploymentResponseDto result = await client.EmploymentsPOSTAsync(input.ToDto(), ct);
        await cache.RemoveByPrefixAsync(CacheKeys.EmploymentsListPrefix, ct);
        await cache.RemoveAsync(CacheKeys.DashboardStats, ct);
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
        CancellationToken ct)
    {
        await client.EmploymentsDELETEAsync(displayId, ct);
        await cache.RemoveAsync(CacheKeys.Employment(displayId), ct);
        await cache.RemoveByPrefixAsync(CacheKeys.EmploymentsListPrefix, ct);
        await cache.RemoveAsync(CacheKeys.DashboardStats, ct);
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

    [GraphQLDescription("Update document metadata")]
    public async Task<DocumentResponseDto?> UpdateDocumentAsync(
        long personDisplayId,
        long documentDisplayId,
        string? description,
        [Service] EmsApiClient client,
        CancellationToken ct)
    {
        UpdateDocumentDto dto = new() { Description = description };
        return await client.DocumentsPUTAsync(personDisplayId, documentDisplayId, dto, ct);
    }

    [GraphQLDescription("Delete a document")]
    public async Task<bool> DeleteDocumentAsync(
        long personDisplayId,
        long documentDisplayId,
        [Service] EmsApiClient client,
        CancellationToken ct)
    {
        await client.DocumentsDELETEAsync(personDisplayId, documentDisplayId, ct);
        return true;
    }

    [GraphQLDescription("Delete a person's profile image")]
    public async Task<bool> DeleteProfileImageAsync(
        long personDisplayId,
        [Service] EmsApiClient client,
        [Service] IRedisCacheService cache,
        CancellationToken ct)
    {
        await client.ProfileImageDELETEAsync(personDisplayId, ct);

        // Invalidate person cache since profile image URL may have changed
        await cache.RemoveAsync(CacheKeys.Person(personDisplayId), ct);
        await cache.RemoveByPrefixAsync(CacheKeys.PersonsListPrefix, ct);

        return true;
    }

    #endregion

    #region Auth Mutations

    [GraphQLDescription("Login with Google ID token")]
    public async Task<AuthResponseDto?> GoogleLoginAsync(
        string idToken,
        [Service] EmsApiClient client,
        CancellationToken ct)
    {
        return await client.GoogleAsync(
            new GoogleAuthRequestDto { IdToken = idToken },
            ct);
    }

    [GraphQLDescription("Login with Google access token")]
    public async Task<AuthResponseDto?> GoogleTokenLoginAsync(
        string accessToken,
        [Service] EmsApiClient client,
        CancellationToken ct)
    {
        return await client.TokenAsync(
            new GoogleAccessTokenRequestDto { AccessToken = accessToken },
            ct);
    }

    [GraphQLDescription("Refresh authentication token")]
    public async Task<AuthResponseDto?> RefreshTokenAsync(
        string? refreshToken,
        [Service] EmsApiClient client,
        CancellationToken ct)
    {
        return await client.RefreshAsync(
            new RefreshTokenRequestDto { RefreshToken = refreshToken },
            ct);
    }

    [GraphQLDescription("Logout and revoke tokens")]
    public async Task<bool> LogoutAsync(
        string? refreshToken,
        [Service] EmsApiClient client,
        CancellationToken ct)
    {
        await client.RevokeAsync(
            new RevokeTokenRequestDto { RefreshToken = refreshToken },
            ct);
        return true;
    }

    #endregion
}
