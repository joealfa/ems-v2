# Data Transfer Objects (DTOs)

This document describes all DTOs used in the EMS backend API.

---

## Authentication DTOs

### GoogleAuthRequestDto

Request DTO for Google OAuth2 authentication using ID token.

```csharp
public class GoogleAuthRequestDto
{
    public string IdToken { get; set; } = string.Empty;
}
```

### GoogleAccessTokenRequestDto

Request DTO for Google OAuth2 authentication using access token (Swagger flow).

```csharp
public class GoogleAccessTokenRequestDto
{
    public string AccessToken { get; set; } = string.Empty;
}
```

### RefreshTokenRequestDto

Request DTO for refreshing an access token.

```csharp
public class RefreshTokenRequestDto
{
    public string RefreshToken { get; set; } = string.Empty;
}
```

### AuthResponseDto

Response DTO containing authentication tokens and user information.

```csharp
public class AuthResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresOn { get; set; }
    public UserDto User { get; set; } = null!;
}
```

### UserDto

DTO representing user information.

```csharp
public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? ProfilePictureUrl { get; set; }
    public string Role { get; set; } = string.Empty;
}
```

---

## Common DTOs

### BaseResponseDto

Base class for all response DTOs with audit information.

```csharp
public abstract class BaseResponseDto
{
    public long DisplayId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public string? ModifiedBy { get; set; }
}
```

### PagedResult<T>

Generic wrapper for paginated responses.

```csharp
public class PagedResult<T>
{
    public List<T> Items { get; set; }
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
}
```

### PaginationParams

Common pagination parameters for list queries.

```csharp
public class PaginationParams
{
    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;
    
    [Range(1, 100)]
    public int PageSize { get; set; } = 10;
    
    public string? Search { get; set; }
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; } = false;
}
```

---

## Person DTOs

### CreatePersonDto

```csharp
public class CreatePersonDto
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string LastName { get; set; }
    
    [MaxLength(100)]
    public string? MiddleName { get; set; }
    
    [Required]
    public DateOnly DateOfBirth { get; set; }
    
    [Required]
    public Gender Gender { get; set; }
    
    [Required]
    public CivilStatus CivilStatus { get; set; }
}
```

### UpdatePersonDto

```csharp
public class UpdatePersonDto
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string LastName { get; set; }
    
    [MaxLength(100)]
    public string? MiddleName { get; set; }
    
    [Required]
    public DateOnly DateOfBirth { get; set; }
    
    [Required]
    public Gender Gender { get; set; }
    
    [Required]
    public CivilStatus CivilStatus { get; set; }
    
    public List<CreateAddressDto>? Addresses { get; set; }
    public List<CreateContactDto>? Contacts { get; set; }
}
```

### PersonResponseDto

Complete person data including related entities.

```csharp
public class PersonResponseDto : BaseResponseDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? MiddleName { get; set; }
    public string FullName { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public Gender Gender { get; set; }
    public CivilStatus CivilStatus { get; set; }
    public string? ProfileImageUrl { get; set; }
    public List<AddressResponseDto> Addresses { get; set; }
    public List<ContactResponseDto> Contacts { get; set; }
}
```

### PersonListDto

Lightweight DTO for list views.

```csharp
public class PersonListDto
{
    public long DisplayId { get; set; }
    public string FullName { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public Gender Gender { get; set; }
    public CivilStatus CivilStatus { get; set; }
    public string? ProfileImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

---

## Employment DTOs

### EmploymentPaginationQuery

Query parameters for employment list with column filtering support.

```csharp
public class EmploymentPaginationQuery : PaginationQuery
{
    /// <summary>
    /// Filter by employment status (Regular, Permanent).
    /// </summary>
    public EmploymentStatus? EmploymentStatus { get; set; }

    /// <summary>
    /// Filter by active status.
    /// </summary>
    public bool? IsActive { get; set; }

    /// <summary>
    /// Filter by display ID (contains search).
    /// </summary>
    public string? DisplayIdFilter { get; set; }

    /// <summary>
    /// Filter by employee name (contains search, supports multi-word).
    /// </summary>
    public string? EmployeeNameFilter { get; set; }

    /// <summary>
    /// Filter by DepEd ID (contains search).
    /// </summary>
    public string? DepEdIdFilter { get; set; }

    /// <summary>
    /// Filter by position title (contains search).
    /// </summary>
    public string? PositionFilter { get; set; }
}
```

**Note:** Multi-word filters (e.g., "John Doe") are split by spaces, and ALL terms must match. This allows filtering by full name even when names are stored separately.

### CreateEmploymentDto

```csharp
public class CreateEmploymentDto
{
    [Required]
    public long PersonDisplayId { get; set; }
    
    [Required]
    public long PositionDisplayId { get; set; }
    
    [Required]
    public long SalaryGradeDisplayId { get; set; }
    
    [Required]
    public long ItemDisplayId { get; set; }
    
    [MaxLength(50)]
    public string? DepEdId { get; set; }
    
    [MaxLength(50)]
    public string? PSIPOPItemNumber { get; set; }
    
    [MaxLength(20)]
    public string? TINId { get; set; }
    
    [MaxLength(20)]
    public string? GSISId { get; set; }
    
    [MaxLength(20)]
    public string? PhilHealthId { get; set; }
    
    public DateOnly? DateOfOriginalAppointment { get; set; }
    
    [Required]
    public AppointmentStatus AppointmentStatus { get; set; }
    
    [Required]
    public EmploymentStatus EmploymentStatus { get; set; }
    
    [Required]
    public Eligibility Eligibility { get; set; }
    
    public bool IsActive { get; set; } = true;
}
```

### EmploymentResponseDto

```csharp
public class EmploymentResponseDto : BaseResponseDto
{
    public string? DepEdId { get; set; }
    public string? PSIPOPItemNumber { get; set; }
    public string? TINId { get; set; }
    public string? GSISId { get; set; }
    public string? PhilHealthId { get; set; }
    public DateOnly? DateOfOriginalAppointment { get; set; }
    public AppointmentStatus AppointmentStatus { get; set; }
    public EmploymentStatus EmploymentStatus { get; set; }
    public Eligibility Eligibility { get; set; }
    public bool IsActive { get; set; }
    
    // Related entities
    public PersonListDto Person { get; set; }
    public PositionResponseDto Position { get; set; }
    public SalaryGradeResponseDto SalaryGrade { get; set; }
    public ItemResponseDto Item { get; set; }
    public List<EmploymentSchoolResponseDto> Schools { get; set; }
}
```

### EmploymentListDto

```csharp
public class EmploymentListDto
{
    public long DisplayId { get; set; }
    public string PersonFullName { get; set; }
    public string PositionTitle { get; set; }
    public string SalaryGradeName { get; set; }
    public string ItemName { get; set; }
    public EmploymentStatus EmploymentStatus { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

### CreateEmploymentSchoolDto

```csharp
public class CreateEmploymentSchoolDto
{
    [Required]
    public long SchoolDisplayId { get; set; }
    
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public bool IsCurrent { get; set; } = false;
}
```

### EmploymentSchoolResponseDto

```csharp
public class EmploymentSchoolResponseDto
{
    public long EmploymentDisplayId { get; set; }
    public long SchoolDisplayId { get; set; }
    public string SchoolName { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public bool IsCurrent { get; set; }
    public bool IsActive { get; set; }
}
```

---

## School DTOs

### CreateSchoolDto

```csharp
public class CreateSchoolDto
{
    [Required]
    [MaxLength(200)]
    public string SchoolName { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public List<CreateAddressDto>? Addresses { get; set; }
    public List<CreateContactDto>? Contacts { get; set; }
}
```

### SchoolResponseDto

```csharp
public class SchoolResponseDto : BaseResponseDto
{
    public string SchoolName { get; set; }
    public bool IsActive { get; set; }
    public List<AddressResponseDto> Addresses { get; set; }
    public List<ContactResponseDto> Contacts { get; set; }
}
```

### SchoolListDto

```csharp
public class SchoolListDto
{
    public long DisplayId { get; set; }
    public string SchoolName { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

---

## Position DTOs

### CreatePositionDto

```csharp
public class CreatePositionDto
{
    [Required]
    [MaxLength(100)]
    public string TitleName { get; set; }
    
    [MaxLength(500)]
    public string? Description { get; set; }
    
    public bool IsActive { get; set; } = true;
}
```

### PositionResponseDto

```csharp
public class PositionResponseDto : BaseResponseDto
{
    public string TitleName { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}
```

---

## Salary Grade DTOs

### CreateSalaryGradeDto

```csharp
public class CreateSalaryGradeDto
{
    [Required]
    [MaxLength(50)]
    public string SalaryGradeName { get; set; }
    
    [MaxLength(500)]
    public string? Description { get; set; }
    
    [Required]
    [Range(1, 8)]
    public int Step { get; set; }
    
    [Required]
    [Range(0, double.MaxValue)]
    public decimal MonthlySalary { get; set; }
    
    public bool IsActive { get; set; } = true;
}
```

### SalaryGradeResponseDto

```csharp
public class SalaryGradeResponseDto : BaseResponseDto
{
    public string SalaryGradeName { get; set; }
    public string? Description { get; set; }
    public int Step { get; set; }
    public decimal MonthlySalary { get; set; }
    public bool IsActive { get; set; }
}
```

---

## Item DTOs

### CreateItemDto

```csharp
public class CreateItemDto
{
    [Required]
    [MaxLength(100)]
    public string ItemName { get; set; }
    
    [MaxLength(500)]
    public string? Description { get; set; }
    
    public bool IsActive { get; set; } = true;
}
```

### ItemResponseDto

```csharp
public class ItemResponseDto : BaseResponseDto
{
    public string ItemName { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}
```

---

## Document DTOs

### UploadDocumentDto

```csharp
public class UploadDocumentDto
{
    [Required]
    public IFormFile File { get; set; }
    
    [Required]
    public DocumentType DocumentType { get; set; }
    
    [MaxLength(500)]
    public string? Description { get; set; }
}
```

### UpdateDocumentDto

```csharp
public class UpdateDocumentDto
{
    [MaxLength(500)]
    public string? Description { get; set; }
    
    public DocumentType? DocumentType { get; set; }
}
```

### DocumentResponseDto

```csharp
public class DocumentResponseDto : BaseResponseDto
{
    public string FileName { get; set; }
    public string FileExtension { get; set; }
    public string ContentType { get; set; }
    public long FileSizeBytes { get; set; }
    public DocumentType DocumentType { get; set; }
    public string BlobUrl { get; set; }
    public string? Description { get; set; }
    public long PersonDisplayId { get; set; }
}
```

### DocumentListDto

```csharp
public class DocumentListDto
{
    public long DisplayId { get; set; }
    public string FileName { get; set; }
    public string FileExtension { get; set; }
    public DocumentType DocumentType { get; set; }
    public long FileSizeBytes { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

---

## Address DTOs

### CreateAddressDto

```csharp
public class CreateAddressDto
{
    [Required]
    [MaxLength(200)]
    public string Address1 { get; set; }
    
    [MaxLength(200)]
    public string? Address2 { get; set; }
    
    [MaxLength(100)]
    public string? Barangay { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string City { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Province { get; set; }
    
    [MaxLength(100)]
    public string Country { get; set; } = "Philippines";
    
    [MaxLength(20)]
    public string? ZipCode { get; set; }
    
    public bool IsCurrent { get; set; } = false;
    public bool IsPermanent { get; set; } = false;
    public bool IsActive { get; set; } = true;
    
    [Required]
    public AddressType AddressType { get; set; }
}
```

### AddressResponseDto

```csharp
public class AddressResponseDto
{
    public long DisplayId { get; set; }
    public string Address1 { get; set; }
    public string? Address2 { get; set; }
    public string? Barangay { get; set; }
    public string City { get; set; }
    public string Province { get; set; }
    public string Country { get; set; }
    public string? ZipCode { get; set; }
    public string FullAddress { get; set; }
    public bool IsCurrent { get; set; }
    public bool IsPermanent { get; set; }
    public bool IsActive { get; set; }
    public AddressType AddressType { get; set; }
}
```

---

## Contact DTOs

### CreateContactDto

```csharp
public class CreateContactDto
{
    [MaxLength(20)]
    public string? Mobile { get; set; }
    
    [MaxLength(20)]
    public string? LandLine { get; set; }
    
    [MaxLength(20)]
    public string? Fax { get; set; }
    
    [MaxLength(256)]
    [EmailAddress]
    public string? Email { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    [Required]
    public ContactType ContactType { get; set; }
}
```

### ContactResponseDto

```csharp
public class ContactResponseDto
{
    public long DisplayId { get; set; }
    public string? Mobile { get; set; }
    public string? LandLine { get; set; }
    public string? Fax { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; }
    public ContactType ContactType { get; set; }
}
```

---

## Dashboard DTOs

### DashboardStatsDto

```csharp
public record DashboardStatsDto
{
    public int TotalPersons { get; init; }
    public int ActiveEmployments { get; init; }
    public int TotalSchools { get; init; }
    public int TotalPositions { get; init; }
    public int TotalSalaryGrades { get; init; }
    public int TotalItems { get; init; }
    public IReadOnlyList<BirthdayCelebrantDto> BirthdayCelebrants { get; init; } = [];
    public IReadOnlyList<RecentActivityDto> RecentActivities { get; init; } = [];
}
```

### BirthdayCelebrantDto

Represents a person with a birthday in the current month.

```csharp
public record BirthdayCelebrantDto
{
    public long DisplayId { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string? MiddleName { get; init; }
    public string FullName { get; init; } = string.Empty;
    public DateOnly DateOfBirth { get; init; }
    public string? ProfileImageUrl { get; init; }
    public bool HasProfileImage { get; init; }
}
```

### RecentActivityDto

Represents a recent activity entry from the database.

```csharp
public record RecentActivityDto
{
    public long Id { get; init; }
    public string EntityType { get; init; } = string.Empty;
    public string EntityId { get; init; } = string.Empty;
    public string Operation { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; }
    public string? UserId { get; init; }
}
```

---

## DTO Mapping Conventions

### Entity to Response DTO

```csharp
// In Service layer
public PersonResponseDto MapToResponseDto(Person entity)
{
    return new PersonResponseDto
    {
        DisplayId = entity.DisplayId,
        FirstName = entity.FirstName,
        LastName = entity.LastName,
        MiddleName = entity.MiddleName,
        FullName = entity.FullName,
        DateOfBirth = entity.DateOfBirth,
        Gender = entity.Gender,
        CivilStatus = entity.CivilStatus,
        ProfileImageUrl = entity.ProfileImageUrl,
        Addresses = entity.Addresses.Select(MapToAddressDto).ToList(),
        Contacts = entity.Contacts.Select(MapToContactDto).ToList(),
        CreatedAt = entity.CreatedAt,
        CreatedBy = entity.CreatedBy,
        ModifiedAt = entity.ModifiedAt,
        ModifiedBy = entity.ModifiedBy
    };
}
```

### Create DTO to Entity

```csharp
// In Service layer
public Person MapToEntity(CreatePersonDto dto)
{
    return new Person
    {
        FirstName = dto.FirstName,
        LastName = dto.LastName,
        MiddleName = dto.MiddleName,
        DateOfBirth = dto.DateOfBirth,
        Gender = dto.Gender,
        CivilStatus = dto.CivilStatus,
        CreatedBy = "System",  // TODO: Get from auth context
        CreatedAt = DateTime.UtcNow
    };
}
```
