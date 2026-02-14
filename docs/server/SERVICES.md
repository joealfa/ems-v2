# Services Documentation

This document describes the service layer architecture in the EMS backend.

---

## Service Layer Overview

The service layer contains all business logic and acts as an intermediary between controllers and repositories.

```
Controller → Service → Repository → Database
                ↓
           Blob Storage
```

---

## Service Interfaces

All services are defined by interfaces in the Application layer and implemented in the Application.Services namespace.

### Location

- **Interfaces**: `EmployeeManagementSystem.Application/Interfaces/`
- **Implementations**: `EmployeeManagementSystem.Application/Services/`

---

## Generic Repository Interface

### IRepository<T>

Base repository interface for all entities.

```csharp
public interface IRepository<T> where T : BaseEntity
{
    // Read Operations
    Task<T?> GetByIdAsync(Guid id);
    Task<T?> GetByDisplayIdAsync(long displayId);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<PagedResult<T>> GetPagedAsync(
        int page, 
        int pageSize, 
        Expression<Func<T, bool>>? filter = null,
        Expression<Func<T, object>>? orderBy = null,
        bool descending = false);
    
    // Write Operations
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(Guid id);  // Soft delete
    
    // Utility
    Task<bool> ExistsAsync(Guid id);
    Task<bool> ExistsAsync(long displayId);
    IQueryable<T> Query();
}
```

---

## Authentication Service

### IAuthService

Handles Google OAuth2 authentication and JWT token management.

```csharp
public interface IAuthService
{
    /// <summary>
    /// Authenticates a user using a Google OAuth2 ID token.
    /// Creates a new user if one doesn't exist.
    /// </summary>
    Task<AuthResponseDto> AuthenticateWithGoogleAsync(string idToken, string ipAddress);

    /// <summary>
    /// Authenticates a user using a Google OAuth2 access token.
    /// Used for Swagger UI OAuth2 flow.
    /// </summary>
    Task<AuthResponseDto> AuthenticateWithGoogleAccessTokenAsync(string accessToken, string ipAddress);

    /// <summary>
    /// Refreshes an access token using a valid refresh token.
    /// Implements token rotation for security.
    /// </summary>
    Task<AuthResponseDto?> RefreshTokenAsync(string refreshToken, string ipAddress);

    /// <summary>
    /// Revokes a refresh token (logout).
    /// </summary>
    Task<bool> RevokeTokenAsync(string refreshToken, string ipAddress);

    /// <summary>
    /// Gets a user by their ID.
    /// </summary>
    Task<UserDto?> GetUserByIdAsync(Guid userId);
}
```

**Implementation Details:**

- Validates Google ID tokens using Google's public keys
- Creates new users on first authentication
- Generates JWT access tokens with configurable expiration
- Implements refresh token rotation (old token revoked when new one issued)
- Tracks token creation/revocation IP addresses for security

---

## Entity Services

### IPersonService

```csharp
public interface IPersonService
{
    Task<PersonResponseDto?> GetByDisplayIdAsync(long displayId);
    Task<PagedResult<PersonListDto>> GetPagedAsync(
        int page, 
        int pageSize, 
        string? search = null);
    Task<PersonResponseDto> CreateAsync(CreatePersonDto dto);
    Task<PersonResponseDto?> UpdateAsync(long displayId, UpdatePersonDto dto);
    Task<bool> DeleteAsync(long displayId);
}
```

**Implementation Highlights:**

```csharp
public class PersonService : IPersonService
{
    private readonly IRepository<Person> _repository;
    private readonly IRepository<Address> _addressRepository;
    private readonly IRepository<Contact> _contactRepository;

    public async Task<PersonResponseDto> CreateAsync(CreatePersonDto dto)
    {
        var person = new Person
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            MiddleName = dto.MiddleName,
            DateOfBirth = dto.DateOfBirth,
            Gender = dto.Gender,
            CivilStatus = dto.CivilStatus,
            CreatedBy = "System",
            CreatedAt = DateTime.UtcNow
        };
        
        await _repository.AddAsync(person);
        return person.ToResponseDto();
    }
    
    public async Task<PagedResult<PersonListDto>> GetPagedAsync(
        int page, int pageSize, string? search)
    {
        var query = _repository.Query()
            .Include(p => p.Addresses)
            .Include(p => p.Contacts);
        
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(p => 
                p.FirstName.Contains(search) ||
                p.LastName.Contains(search) ||
                p.FullName.Contains(search));
        }
        
        var result = await GetPagedAsync(query, page, pageSize);
        return new PagedResult<PersonListDto>
        {
            Items = result.Items.Select(p => p.ToListDto()).ToList(),
            TotalCount = result.TotalCount,
            Page = page,
            PageSize = pageSize
        };
    }
}
```

---

### IEmploymentService

```csharp
public interface IEmploymentService
{
    Task<EmploymentResponseDto?> GetByDisplayIdAsync(long displayId);
    Task<PagedResult<EmploymentListDto>> GetPagedAsync(
        int page, 
        int pageSize, 
        string? search = null);
    Task<EmploymentResponseDto> CreateAsync(CreateEmploymentDto dto);
    Task<EmploymentResponseDto?> UpdateAsync(long displayId, UpdateEmploymentDto dto);
    Task<bool> DeleteAsync(long displayId);
    
    // School assignments
    Task<EmploymentSchoolResponseDto?> AddSchoolAsync(
        long employmentDisplayId, 
        CreateEmploymentSchoolDto dto);
    Task<bool> RemoveSchoolAsync(
        long employmentDisplayId, 
        long schoolDisplayId);
}
```

**Key Implementation:**

```csharp
public class EmploymentService : IEmploymentService
{
    public async Task<EmploymentResponseDto> CreateAsync(CreateEmploymentDto dto)
    {
        // Validate related entities exist
        var person = await _personRepository.GetByDisplayIdAsync(dto.PersonDisplayId)
            ?? throw new InvalidOperationException("Person not found");
        var position = await _positionRepository.GetByDisplayIdAsync(dto.PositionDisplayId)
            ?? throw new InvalidOperationException("Position not found");
        var salaryGrade = await _salaryGradeRepository.GetByDisplayIdAsync(dto.SalaryGradeDisplayId)
            ?? throw new InvalidOperationException("Salary Grade not found");
        var item = await _itemRepository.GetByDisplayIdAsync(dto.ItemDisplayId)
            ?? throw new InvalidOperationException("Item not found");
        
        var employment = new Employment
        {
            PersonId = person.Id,
            PositionId = position.Id,
            SalaryGradeId = salaryGrade.Id,
            ItemId = item.Id,
            // ... other properties
        };
        
        await _repository.AddAsync(employment);
        return await GetByDisplayIdAsync(employment.DisplayId);
    }
}
```

---

### ISchoolService

```csharp
public interface ISchoolService
{
    Task<SchoolResponseDto?> GetByDisplayIdAsync(long displayId);
    Task<PagedResult<SchoolListDto>> GetPagedAsync(
        int page, 
        int pageSize, 
        string? search = null);
    Task<SchoolResponseDto> CreateAsync(CreateSchoolDto dto);
    Task<SchoolResponseDto?> UpdateAsync(long displayId, UpdateSchoolDto dto);
    Task<bool> DeleteAsync(long displayId);
}
```

---

### IPositionService

```csharp
public interface IPositionService
{
    Task<PositionResponseDto?> GetByDisplayIdAsync(long displayId);
    Task<PagedResult<PositionResponseDto>> GetPagedAsync(
        int page, 
        int pageSize, 
        string? search = null);
    Task<PositionResponseDto> CreateAsync(CreatePositionDto dto);
    Task<PositionResponseDto?> UpdateAsync(long displayId, UpdatePositionDto dto);
    Task<bool> DeleteAsync(long displayId);
}
```

---

### ISalaryGradeService

```csharp
public interface ISalaryGradeService
{
    Task<SalaryGradeResponseDto?> GetByDisplayIdAsync(long displayId);
    Task<PagedResult<SalaryGradeResponseDto>> GetPagedAsync(
        int page, 
        int pageSize, 
        string? search = null);
    Task<SalaryGradeResponseDto> CreateAsync(CreateSalaryGradeDto dto);
    Task<SalaryGradeResponseDto?> UpdateAsync(long displayId, UpdateSalaryGradeDto dto);
    Task<bool> DeleteAsync(long displayId);
}
```

---

### IItemService

```csharp
public interface IItemService
{
    Task<ItemResponseDto?> GetByDisplayIdAsync(long displayId);
    Task<PagedResult<ItemResponseDto>> GetPagedAsync(
        int page, 
        int pageSize, 
        string? search = null);
    Task<ItemResponseDto> CreateAsync(CreateItemDto dto);
    Task<ItemResponseDto?> UpdateAsync(long displayId, UpdateItemDto dto);
    Task<bool> DeleteAsync(long displayId);
}
```

---

## Document Services

### IDocumentService

```csharp
public interface IDocumentService
{
    Task<DocumentResponseDto?> GetByDisplayIdAsync(
        long personDisplayId, 
        long documentDisplayId);
    Task<PagedResult<DocumentListDto>> GetPagedAsync(
        long personDisplayId,
        int page, 
        int pageSize);
    Task<DocumentResponseDto?> UploadAsync(
        long personDisplayId, 
        UploadDocumentDto dto);
    Task<DocumentResponseDto?> UpdateAsync(
        long personDisplayId,
        long documentDisplayId, 
        UpdateDocumentDto dto);
    Task<BlobDownloadResultDto?> DownloadAsync(
        long personDisplayId, 
        long documentDisplayId);
    Task<bool> DeleteAsync(
        long personDisplayId, 
        long documentDisplayId);
    
    // Profile Image
    Task<string?> UploadProfileImageAsync(
        long personDisplayId, 
        IFormFile file);
    Task<bool> DeleteProfileImageAsync(long personDisplayId);
}
```

**Key Implementation:**

```csharp
public class DocumentService : IDocumentService
{
    private readonly IRepository<Document> _repository;
    private readonly IRepository<Person> _personRepository;
    private readonly IBlobStorageService _blobStorage;
    
    private static readonly string[] AllowedDocumentExtensions = 
        { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".jpg", ".jpeg", ".png" };
    private static readonly string[] AllowedImageExtensions = 
        { ".jpg", ".jpeg", ".png" };
    private const long MaxDocumentSize = 50 * 1024 * 1024; // 50 MB
    private const long MaxImageSize = 5 * 1024 * 1024; // 5 MB
    
    public async Task<DocumentResponseDto?> UploadAsync(
        long personDisplayId, 
        UploadDocumentDto dto)
    {
        var person = await _personRepository.GetByDisplayIdAsync(personDisplayId)
            ?? throw new InvalidOperationException("Person not found");
        
        // Validate file
        var extension = Path.GetExtension(dto.File.FileName).ToLower();
        if (!AllowedDocumentExtensions.Contains(extension))
            throw new InvalidOperationException("File type not allowed");
        
        if (dto.File.Length > MaxDocumentSize)
            throw new InvalidOperationException("File too large");
        
        // Upload to blob storage
        var blobName = $"persons/{person.DisplayId}/documents/{Guid.NewGuid()}{extension}";
        var blobUrl = await _blobStorage.UploadAsync(
            "documents", 
            blobName, 
            dto.File.OpenReadStream(),
            dto.File.ContentType);
        
        // Create document record
        var document = new Document
        {
            FileName = dto.File.FileName,
            FileExtension = extension,
            ContentType = dto.File.ContentType,
            FileSizeBytes = dto.File.Length,
            DocumentType = dto.DocumentType,
            BlobUrl = blobUrl,
            BlobName = blobName,
            ContainerName = "documents",
            Description = dto.Description,
            PersonId = person.Id,
            CreatedBy = "System",
            CreatedAt = DateTime.UtcNow
        };
        
        await _repository.AddAsync(document);
        return document.ToResponseDto();
    }
}
```

---

### IBlobStorageService

Interface for Azure Blob Storage operations.

```csharp
public interface IBlobStorageService
{
    Task<string> UploadAsync(
        string containerName, 
        string blobName, 
        Stream content, 
        string contentType);
    Task<Stream> DownloadAsync(
        string containerName, 
        string blobName);
    Task<bool> DeleteAsync(
        string containerName, 
        string blobName);
    Task<bool> ExistsAsync(
        string containerName, 
        string blobName);
    string GetBlobUrl(
        string containerName, 
        string blobName);
}
```

**Implementation (in Infrastructure layer):**

```csharp
public class BlobStorageService : IBlobStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    
    public BlobStorageService(string connectionString)
    {
        _blobServiceClient = new BlobServiceClient(connectionString);
    }
    
    public async Task<string> UploadAsync(
        string containerName, 
        string blobName, 
        Stream content, 
        string contentType)
    {
        var container = _blobServiceClient.GetBlobContainerClient(containerName);
        await container.CreateIfNotExistsAsync(PublicAccessType.Blob);
        
        var blob = container.GetBlobClient(blobName);
        await blob.UploadAsync(content, new BlobHttpHeaders 
        { 
            ContentType = contentType 
        });
        
        return blob.Uri.ToString();
    }
}
```

---

## Reports Service

### IReportsService

```csharp
public interface IReportsService
{
    Task<DashboardStatsDto> GetDashboardStatsAsync();
}
```

**Implementation:**

```csharp
public class ReportsService(
    IRepository<Person> personRepository,
    IRepository<Employment> employmentRepository,
    IRepository<School> schoolRepository,
    IRepository<Position> positionRepository,
    IRepository<SalaryGrade> salaryGradeRepository,
    IRepository<Item> itemRepository,
    IRecentActivityRepository activityRepository) : IReportsService
{
    public async Task<DashboardStatsDto> GetDashboardStatsAsync(CancellationToken ct = default)
    {
        // Entity counts
        int totalPersons = await personRepository.Query().CountAsync(ct);
        int activeEmployments = await employmentRepository.Query()
            .Where(e => e.IsActive).CountAsync(ct);
        // ... other counts ...

        // Birthday celebrants for current month
        int currentMonth = DateTime.UtcNow.Month;
        var birthdayCelebrants = await personRepository.Query()
            .Where(p => p.DateOfBirth.Month == currentMonth)
            .OrderBy(p => p.DateOfBirth.Day)
            .Select(p => new BirthdayCelebrantDto { /* map fields */ })
            .ToListAsync(ct);

        // Recent activities (last 10 from database)
        var recentActivities = await activityRepository.GetLatestAsync(10, ct);
        var recentActivityDtos = recentActivities
            .Select(a => new RecentActivityDto { /* map fields */ })
            .ToList();

        return new DashboardStatsDto
        {
            TotalPersons = totalPersons,
            ActiveEmployments = activeEmployments,
            // ... other counts ...
            BirthdayCelebrants = birthdayCelebrants,
            RecentActivities = recentActivityDtos
        };
    }
}
```

---

## Dependency Injection

Services are registered in `Program.cs`:

```csharp
// Generic Repository
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Entity Services (with ILogger injected automatically)
builder.Services.AddScoped<IPersonService, PersonService>();
builder.Services.AddScoped<ISchoolService, SchoolService>();
builder.Services.AddScoped<IPositionService, PositionService>();
builder.Services.AddScoped<ISalaryGradeService, SalaryGradeService>();
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<IEmploymentService, EmploymentService>();

// Document Services (with logging)
builder.Services.AddScoped<IBlobStorageService, BlobStorageService>();
builder.Services.AddScoped<IDocumentService, DocumentService>();

// Authentication (with logging)
builder.Services.AddScoped<IAuthService, AuthService>();

// Reports
builder.Services.AddScoped<IReportsService, ReportsService>();

// Activity persistence
builder.Services.AddScoped<IRecentActivityRepository, RecentActivityRepository>();

// Event publishing (Decorator pattern)
// RabbitMQEventPublisher is the inner publisher, ActivityPersistingEventPublisher wraps it
// to persist activities to the database before publishing to RabbitMQ
builder.Services.AddSingleton<RabbitMQEventPublisher>();
builder.Services.AddScoped<IEventPublisher>(sp => new ActivityPersistingEventPublisher(
    sp.GetRequiredService<RabbitMQEventPublisher>(),
    sp.GetRequiredService<IRecentActivityRepository>(),
    sp.GetRequiredService<ILogger<ActivityPersistingEventPublisher>>()));
```

**Note**: All services use constructor injection for `ILogger<T>` automatically via ASP.NET Core's dependency injection.

---

## Service Patterns

### 1. DTO Mapping via Extension Methods

Services use extension methods from `Application/Mappings/` for DTO mapping:

```csharp
// Extension methods are defined in Application/Mappings/PersonMappingExtensions.cs
public static class PersonMappingExtensions
{
    public static PersonResponseDto ToResponseDto(this Person entity)
    {
        return new PersonResponseDto
        {
            DisplayId = entity.DisplayId,
            FirstName = entity.FirstName,
            // ... other properties
        };
    }

    public static PersonListDto ToListDto(this Person entity)
    {
        return new PersonListDto
        {
            DisplayId = entity.DisplayId,
            FullName = entity.FullName,
            // ... other properties
        };
    }
}

// Usage in service
public async Task<PersonResponseDto> GetByIdAsync(long displayId)
{
    Person? person = await _repository.GetByDisplayIdAsync(displayId);
    return person?.ToResponseDto();
}
```

### 2. Validation Pattern

Services validate business rules before operations:

```csharp
public async Task<EmploymentResponseDto> CreateAsync(CreateEmploymentDto dto)
{
    // Validate required relationships exist
    var person = await _personRepository.GetByDisplayIdAsync(dto.PersonDisplayId);
    if (person == null)
        throw new InvalidOperationException($"Person with DisplayId {dto.PersonDisplayId} not found");
    
    // Validate business rules
    var existingActiveEmployment = await _employmentRepository.Query()
        .Where(e => e.PersonId == person.Id && e.IsActive)
        .FirstOrDefaultAsync();
    
    if (existingActiveEmployment != null)
        throw new InvalidOperationException("Person already has an active employment");
    
    // Proceed with creation
    // ...
}
```

### 3. Transaction Pattern

For operations that modify multiple entities:

```csharp
public async Task<bool> DeleteAsync(long displayId)
{
    var employment = await _repository.Query()
        .Include(e => e.EmploymentSchools)
        .FirstOrDefaultAsync(e => e.DisplayId == displayId);
    
    if (employment == null)
        return false;
    
    // Delete related entities first
    foreach (var es in employment.EmploymentSchools)
    {
        await _employmentSchoolRepository.DeleteAsync(es.Id);
    }
    
    // Delete main entity
    await _repository.DeleteAsync(employment.Id);
    
    return true;
}
```

### 4. Eager Loading Pattern

Use Include for related entities:

```csharp
public async Task<PersonResponseDto?> GetByDisplayIdAsync(long displayId)
{
    var person = await _repository.Query()
        .Include(p => p.Addresses.Where(a => !a.IsDeleted))
        .Include(p => p.Contacts.Where(c => !c.IsDeleted))
        .Include(p => p.Documents.Where(d => !d.IsDeleted))
        .FirstOrDefaultAsync(p => p.DisplayId == displayId);
    
    return person == null ? null : person.ToResponseDto();
}
```
