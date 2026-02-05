using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace EmployeeManagementSystem.Gateway.Caching;

public static class CacheKeys
{
    public const string PersonPrefix = "person:";
    public const string PersonsListPrefix = "persons:list:";
    public const string EmploymentPrefix = "employment:";
    public const string EmploymentsListPrefix = "employments:list:";
    public const string SchoolPrefix = "school:";
    public const string SchoolsListPrefix = "schools:list:";
    public const string PositionPrefix = "position:";
    public const string PositionsListPrefix = "positions:list:";
    public const string SalaryGradePrefix = "salarygrade:";
    public const string SalaryGradesListPrefix = "salarygrades:list:";
    public const string ItemPrefix = "item:";
    public const string ItemsListPrefix = "items:list:";
    public const string DashboardStats = "dashboard:stats";
    public const string CurrentUser = "user:current:";

    public static string Person(long displayId)
    {
        return $"{PersonPrefix}{displayId}";
    }

    public static string Employment(long displayId)
    {
        return $"{EmploymentPrefix}{displayId}";
    }

    public static string School(long displayId)
    {
        return $"{SchoolPrefix}{displayId}";
    }

    public static string Position(long displayId)
    {
        return $"{PositionPrefix}{displayId}";
    }

    public static string SalaryGrade(long displayId)
    {
        return $"{SalaryGradePrefix}{displayId}";
    }

    public static string Item(long displayId)
    {
        return $"{ItemPrefix}{displayId}";
    }

    public static string PersonsList(
        int? pageNumber,
        int? pageSize,
        string? searchTerm,
        string? fullNameFilter = null,
        string? displayIdFilter = null,
        string? gender = null,
        string? civilStatus = null,
        string? sortBy = null,
        bool? sortDescending = null)
    {
        return GenerateHashedKey(PersonsListPrefix, new
        {
            pageNumber,
            pageSize,
            searchTerm,
            fullNameFilter,
            displayIdFilter,
            gender,
            civilStatus,
            sortBy,
            sortDescending
        });
    }

    public static string EmploymentsList(
        int? pageNumber,
        int? pageSize,
        string? searchTerm,
        string? displayIdFilter = null,
        string? employeeNameFilter = null,
        string? positionFilter = null,
        string? depEdIdFilter = null,
        string? employmentStatus = null,
        bool? isActive = null,
        string? sortBy = null,
        bool? sortDescending = null)
    {
        return GenerateHashedKey(EmploymentsListPrefix, new
        {
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
            sortDescending
        });
    }

    public static string SchoolsList(
        int? pageNumber,
        int? pageSize,
        string? searchTerm,
        string? sortBy = null,
        bool? sortDescending = null)
    {
        return GenerateHashedKey(SchoolsListPrefix, new
        {
            pageNumber,
            pageSize,
            searchTerm,
            sortBy,
            sortDescending
        });
    }

    public static string PositionsList(
        int? pageNumber,
        int? pageSize,
        string? searchTerm,
        string? sortBy = null,
        bool? sortDescending = null)
    {
        return GenerateHashedKey(PositionsListPrefix, new
        {
            pageNumber,
            pageSize,
            searchTerm,
            sortBy,
            sortDescending
        });
    }

    public static string SalaryGradesList(
        int? pageNumber,
        int? pageSize,
        string? searchTerm,
        string? sortBy = null,
        bool? sortDescending = null)
    {
        return GenerateHashedKey(SalaryGradesListPrefix, new
        {
            pageNumber,
            pageSize,
            searchTerm,
            sortBy,
            sortDescending
        });
    }

    public static string ItemsList(
        int? pageNumber,
        int? pageSize,
        string? searchTerm,
        string? sortBy = null,
        bool? sortDescending = null)
    {
        return GenerateHashedKey(ItemsListPrefix, new
        {
            pageNumber,
            pageSize,
            searchTerm,
            sortBy,
            sortDescending
        });
    }

    /// <summary>
    /// Generates a deterministic hash-based cache key from filter parameters.
    /// This ensures that different filter combinations produce different cache keys.
    /// </summary>
    private static string GenerateHashedKey(string prefix, object parameters)
    {
        // Serialize parameters to JSON for consistent hashing
        string json = JsonSerializer.Serialize(parameters, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never
        });

        // Generate SHA256 hash
        byte[] hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(json));
        string hash = Convert.ToHexString(hashBytes).ToLowerInvariant();

        // Return prefix + hash (first 16 characters for brevity)
        return $"{prefix}{hash[..16]}";
    }
}
