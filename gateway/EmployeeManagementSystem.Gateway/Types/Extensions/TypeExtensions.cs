using EmployeeManagementSystem.ApiClient.Generated;

namespace EmployeeManagementSystem.Gateway.Types;

/// <summary>
/// GraphQL type extensions for NSwag-generated DTOs to properly expose fields
/// with correct GraphQL types.
/// Note: The NSwag-generated DTOs now have proper types (long, int, double),
/// so these extensions mainly provide direct access and compatibility.
/// </summary>

#region Person Types

[ExtendObjectType<PersonResponseDto>]
public class PersonResponseDtoExtensions
{
    /// <summary>
    /// Get the display ID as a long value
    /// </summary>
    public long GetDisplayId([Parent] PersonResponseDto person)
    {
        return person.DisplayId;
    }

    /// <summary>
    /// Get the profile image URL (Gateway proxy URL instead of direct Azure Blob URL for CORS)
    /// Overrides the ProfileImageUrl property from the DTO
    /// </summary>
    [GraphQLName("profileImageUrl")]
    public string? GetProfileImageUrl(
        [Parent] PersonResponseDto person,
        [Service] IConfiguration configuration)
    {
        // Only return URL if person has a profile image
        if (!person.HasProfileImage)
        {
            return null;
        }

        // Return Gateway proxy URL for CORS handling
        string gatewayBaseUrl = configuration["Gateway:BaseUrl"] ?? "https://localhost:5003";
        
        // Ensure the base URL has a protocol
        if (!gatewayBaseUrl.StartsWith("http://") && !gatewayBaseUrl.StartsWith("https://"))
        {
            gatewayBaseUrl = $"https://{gatewayBaseUrl}";
        }
        
        return $"{gatewayBaseUrl}/api/persons/{person.DisplayId}/profile-image";
    }
}

[ExtendObjectType<PersonListDto>]
public class PersonListDtoExtensions
{
    /// <summary>
    /// Get the display ID as a long value
    /// </summary>
    public long GetDisplayId([Parent] PersonListDto person)
    {
        return person.DisplayId;
    }

    /// <summary>
    /// Get the profile image URL (Gateway proxy URL instead of direct Azure Blob URL for CORS)
    /// Overrides the ProfileImageUrl property from the DTO
    /// </summary>
    [GraphQLName("profileImageUrl")]
    public string? GetProfileImageUrl(
        [Parent] PersonListDto person,
        [Service] IConfiguration configuration)
    {
        // Only return URL if person has a profile image
        if (!person.HasProfileImage)
        {
            return null;
        }

        // Return Gateway proxy URL for CORS handling
        string gatewayBaseUrl = configuration["Gateway:BaseUrl"] ?? "https://localhost:5003";
        
        // Ensure the base URL has a protocol
        if (!gatewayBaseUrl.StartsWith("http://") && !gatewayBaseUrl.StartsWith("https://"))
        {
            gatewayBaseUrl = $"https://{gatewayBaseUrl}";
        }
        
        return $"{gatewayBaseUrl}/api/persons/{person.DisplayId}/profile-image";
    }
}

#endregion

#region Pagination Types

[ExtendObjectType<PagedResultOfPersonListDto>]
public class PagedResultOfPersonListDtoExtensions
{
    public int GetTotalCount([Parent] PagedResultOfPersonListDto result)
    {
        return result.TotalCount;
    }

    public int GetPageNumber([Parent] PagedResultOfPersonListDto result)
    {
        return result.PageNumber;
    }

    public int GetPageSize([Parent] PagedResultOfPersonListDto result)
    {
        return result.PageSize;
    }

    public int GetTotalPages([Parent] PagedResultOfPersonListDto result)
    {
        return result.TotalPages;
    }
}

#endregion

#region Document Types

[ExtendObjectType<PagedResultOfDocumentListDto>]
public class PagedResultOfDocumentListDtoExtensions
{
    public int GetTotalCount([Parent] PagedResultOfDocumentListDto result)
    {
        return result.TotalCount;
    }

    public int GetPageNumber([Parent] PagedResultOfDocumentListDto result)
    {
        return result.PageNumber;
    }

    public int GetPageSize([Parent] PagedResultOfDocumentListDto result)
    {
        return result.PageSize;
    }

    public int GetTotalPages([Parent] PagedResultOfDocumentListDto result)
    {
        return result.TotalPages;
    }
}

#endregion

#region Employment Types

[ExtendObjectType<EmploymentResponseDto>]
public class EmploymentResponseDtoExtensions
{
    /// <summary>
    /// Get the display ID as a long value
    /// </summary>
    public long GetDisplayId([Parent] EmploymentResponseDto employment)
    {
        return employment.DisplayId;
    }
}

[ExtendObjectType<EmploymentListDto>]
public class EmploymentListDtoExtensions
{
    /// <summary>
    /// Get the display ID as a long value
    /// </summary>
    public long GetDisplayId([Parent] EmploymentListDto employment)
    {
        return employment.DisplayId;
    }
}

[ExtendObjectType<EmploymentPersonDto>]
public class EmploymentPersonDtoExtensions
{
    /// <summary>
    /// Get the display ID as a long value
    /// </summary>
    public long GetDisplayId([Parent] EmploymentPersonDto person)
    {
        return person.DisplayId;
    }
}

[ExtendObjectType<EmploymentPositionDto>]
public class EmploymentPositionDtoExtensions
{
    /// <summary>
    /// Get the display ID as a long value
    /// </summary>
    public long GetDisplayId([Parent] EmploymentPositionDto position)
    {
        return position.DisplayId;
    }
}

[ExtendObjectType<EmploymentSalaryGradeDto>]
public class EmploymentSalaryGradeDtoExtensions
{
    /// <summary>
    /// Get the display ID as a long value
    /// </summary>
    public long GetDisplayId([Parent] EmploymentSalaryGradeDto salaryGrade)
    {
        return salaryGrade.DisplayId;
    }

    /// <summary>
    /// Get the step as an integer
    /// </summary>
    public int GetStep([Parent] EmploymentSalaryGradeDto salaryGrade)
    {
        return salaryGrade.Step;
    }

    /// <summary>
    /// Get the monthly salary as a decimal
    /// </summary>
    public decimal GetMonthlySalary([Parent] EmploymentSalaryGradeDto salaryGrade)
    {
        return (decimal)salaryGrade.MonthlySalary;
    }
}

[ExtendObjectType<EmploymentItemDto>]
public class EmploymentItemDtoExtensions
{
    /// <summary>
    /// Get the display ID as a long value
    /// </summary>
    public long GetDisplayId([Parent] EmploymentItemDto item)
    {
        return item.DisplayId;
    }
}

[ExtendObjectType<EmploymentSchoolResponseDto>]
public class EmploymentSchoolResponseDtoExtensions
{
    /// <summary>
    /// Get the display ID as a long value
    /// </summary>
    public long GetDisplayId([Parent] EmploymentSchoolResponseDto school)
    {
        return school.DisplayId;
    }

    /// <summary>
    /// Get the school display ID as a long value
    /// </summary>
    public long GetSchoolDisplayId([Parent] EmploymentSchoolResponseDto school)
    {
        return school.SchoolDisplayId;
    }
}

[ExtendObjectType<PagedResultOfEmploymentListDto>]
public class PagedResultOfEmploymentListDtoExtensions
{
    public int GetTotalCount([Parent] PagedResultOfEmploymentListDto result)
    {
        return result.TotalCount;
    }

    public int GetPageNumber([Parent] PagedResultOfEmploymentListDto result)
    {
        return result.PageNumber;
    }

    public int GetPageSize([Parent] PagedResultOfEmploymentListDto result)
    {
        return result.PageSize;
    }

    public int GetTotalPages([Parent] PagedResultOfEmploymentListDto result)
    {
        return result.TotalPages;
    }
}

#endregion

#region School Types

[ExtendObjectType<SchoolResponseDto>]
public class SchoolResponseDtoExtensions
{
    /// <summary>
    /// Get the display ID as a long value
    /// </summary>
    public long GetDisplayId([Parent] SchoolResponseDto school)
    {
        return school.DisplayId;
    }
}

[ExtendObjectType<SchoolListDto>]
public class SchoolListDtoExtensions
{
    /// <summary>
    /// Get the display ID as a long value
    /// </summary>
    public long GetDisplayId([Parent] SchoolListDto school)
    {
        return school.DisplayId;
    }
}

[ExtendObjectType<PagedResultOfSchoolListDto>]
public class PagedResultOfSchoolListDtoExtensions
{
    public int GetTotalCount([Parent] PagedResultOfSchoolListDto result)
    {
        return result.TotalCount;
    }

    public int GetPageNumber([Parent] PagedResultOfSchoolListDto result)
    {
        return result.PageNumber;
    }

    public int GetPageSize([Parent] PagedResultOfSchoolListDto result)
    {
        return result.PageSize;
    }

    public int GetTotalPages([Parent] PagedResultOfSchoolListDto result)
    {
        return result.TotalPages;
    }
}

#endregion

#region Position Types

[ExtendObjectType<PositionResponseDto>]
public class PositionResponseDtoExtensions
{
    /// <summary>
    /// Get the display ID as a long value
    /// </summary>
    public long GetDisplayId([Parent] PositionResponseDto position)
    {
        return position.DisplayId;
    }
}

[ExtendObjectType<PagedResultOfPositionResponseDto>]
public class PagedResultOfPositionResponseDtoExtensions
{
    public int GetTotalCount([Parent] PagedResultOfPositionResponseDto result)
    {
        return result.TotalCount;
    }

    public int GetPageNumber([Parent] PagedResultOfPositionResponseDto result)
    {
        return result.PageNumber;
    }

    public int GetPageSize([Parent] PagedResultOfPositionResponseDto result)
    {
        return result.PageSize;
    }

    public int GetTotalPages([Parent] PagedResultOfPositionResponseDto result)
    {
        return result.TotalPages;
    }
}

#endregion

#region Salary Grade Types

[ExtendObjectType<SalaryGradeResponseDto>]
public class SalaryGradeResponseDtoExtensions
{
    /// <summary>
    /// Get the display ID as a long value
    /// </summary>
    public long GetDisplayId([Parent] SalaryGradeResponseDto salaryGrade)
    {
        return salaryGrade.DisplayId;
    }

    /// <summary>
    /// Get the step as an integer
    /// </summary>
    public int GetStep([Parent] SalaryGradeResponseDto salaryGrade)
    {
        return salaryGrade.Step;
    }

    /// <summary>
    /// Get the monthly salary as a decimal
    /// </summary>
    public decimal GetMonthlySalary([Parent] SalaryGradeResponseDto salaryGrade)
    {
        return (decimal)salaryGrade.MonthlySalary;
    }
}

[ExtendObjectType<PagedResultOfSalaryGradeResponseDto>]
public class PagedResultOfSalaryGradeResponseDtoExtensions
{
    public int GetTotalCount([Parent] PagedResultOfSalaryGradeResponseDto result)
    {
        return result.TotalCount;
    }

    public int GetPageNumber([Parent] PagedResultOfSalaryGradeResponseDto result)
    {
        return result.PageNumber;
    }

    public int GetPageSize([Parent] PagedResultOfSalaryGradeResponseDto result)
    {
        return result.PageSize;
    }

    public int GetTotalPages([Parent] PagedResultOfSalaryGradeResponseDto result)
    {
        return result.TotalPages;
    }
}

#endregion

#region Item Types

[ExtendObjectType<ItemResponseDto>]
public class ItemResponseDtoExtensions
{
    /// <summary>
    /// Get the display ID as a long value
    /// </summary>
    public long GetDisplayId([Parent] ItemResponseDto item)
    {
        return item.DisplayId;
    }
}

[ExtendObjectType<PagedResultOfItemResponseDto>]
public class PagedResultOfItemResponseDtoExtensions
{
    public int GetTotalCount([Parent] PagedResultOfItemResponseDto result)
    {
        return result.TotalCount;
    }

    public int GetPageNumber([Parent] PagedResultOfItemResponseDto result)
    {
        return result.PageNumber;
    }

    public int GetPageSize([Parent] PagedResultOfItemResponseDto result)
    {
        return result.PageSize;
    }

    public int GetTotalPages([Parent] PagedResultOfItemResponseDto result)
    {
        return result.TotalPages;
    }
}

#endregion

#region Address and Contact Types

[ExtendObjectType<AddressResponseDto>]
public class AddressResponseDtoExtensions
{
    /// <summary>
    /// Get the display ID as a long value
    /// </summary>
    public long GetDisplayId([Parent] AddressResponseDto address)
    {
        return address.DisplayId;
    }
}

[ExtendObjectType<ContactResponseDto>]
public class ContactResponseDtoExtensions
{
    /// <summary>
    /// Get the display ID as a long value
    /// </summary>
    public long GetDisplayId([Parent] ContactResponseDto contact)
    {
        return contact.DisplayId;
    }
}

#endregion

#region Document Types

[ExtendObjectType<DocumentListDto>]
public class DocumentListDtoExtensions
{
    /// <summary>
    /// Get the display ID as a long value
    /// </summary>
    public long GetDisplayId([Parent] DocumentListDto document)
    {
        return document.DisplayId;
    }

    /// <summary>
    /// Get the file size in bytes as a long value
    /// </summary>
    public long GetFileSizeBytes([Parent] DocumentListDto document)
    {
        return document.FileSizeBytes;
    }
}

[ExtendObjectType<DocumentResponseDto>]
public class DocumentResponseDtoExtensions
{
    /// <summary>
    /// Get the display ID as a long value
    /// </summary>
    public long GetDisplayId([Parent] DocumentResponseDto document)
    {
        return document.DisplayId;
    }

    /// <summary>
    /// Get the person display ID as a long value
    /// </summary>
    public long GetPersonDisplayId([Parent] DocumentResponseDto document)
    {
        return document.PersonDisplayId;
    }

    /// <summary>
    /// Get the file size in bytes as a long value
    /// </summary>
    public long GetFileSizeBytes([Parent] DocumentResponseDto document)
    {
        return document.FileSizeBytes;
    }
}

#endregion

#region Dashboard Types

[ExtendObjectType<DashboardStatsDto>]
public class DashboardStatsDtoExtensions
{
    /// <summary>
    /// Get the total persons count
    /// </summary>
    public int GetTotalPersons([Parent] DashboardStatsDto stats)
    {
        return stats.TotalPersons;
    }

    /// <summary>
    /// Get the total schools count
    /// </summary>
    public int GetTotalSchools([Parent] DashboardStatsDto stats)
    {
        return stats.TotalSchools;
    }

    /// <summary>
    /// Get the total positions count
    /// </summary>
    public int GetTotalPositions([Parent] DashboardStatsDto stats)
    {
        return stats.TotalPositions;
    }

    /// <summary>
    /// Get the total salary grades count
    /// </summary>
    public int GetTotalSalaryGrades([Parent] DashboardStatsDto stats)
    {
        return stats.TotalSalaryGrades;
    }

    /// <summary>
    /// Get the total items count
    /// </summary>
    public int GetTotalItems([Parent] DashboardStatsDto stats)
    {
        return stats.TotalItems;
    }

    /// <summary>
    /// Get the active employments count
    /// </summary>
    public int GetActiveEmployments([Parent] DashboardStatsDto stats)
    {
        return stats.ActiveEmployments;
    }
}

#endregion
