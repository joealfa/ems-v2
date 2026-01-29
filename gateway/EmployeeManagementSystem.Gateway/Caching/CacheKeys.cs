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

    public static string PersonsList(int? pageNumber, int? pageSize, string? searchTerm)
    {
        return $"{PersonsListPrefix}{pageNumber}:{pageSize}:{searchTerm ?? "null"}";
    }

    public static string EmploymentsList(int? pageNumber, int? pageSize, string? searchTerm, bool? isActive)
    {
        return $"{EmploymentsListPrefix}{pageNumber}:{pageSize}:{searchTerm ?? "null"}:{isActive?.ToString() ?? "null"}";
    }

    public static string SchoolsList(int? pageNumber, int? pageSize, string? searchTerm)
    {
        return $"{SchoolsListPrefix}{pageNumber}:{pageSize}:{searchTerm ?? "null"}";
    }

    public static string PositionsList(int? pageNumber, int? pageSize, string? searchTerm)
    {
        return $"{PositionsListPrefix}{pageNumber}:{pageSize}:{searchTerm ?? "null"}";
    }

    public static string SalaryGradesList(int? pageNumber, int? pageSize, string? searchTerm)
    {
        return $"{SalaryGradesListPrefix}{pageNumber}:{pageSize}:{searchTerm ?? "null"}";
    }

    public static string ItemsList(int? pageNumber, int? pageSize, string? searchTerm)
    {
        return $"{ItemsListPrefix}{pageNumber}:{pageSize}:{searchTerm ?? "null"}";
    }
}
