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
        return MapToResponseDto(person);
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
            Items = result.Items.Select(MapToListDto).ToList(),
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
        return MapToResponseDto(document);
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
public class ReportsService : IReportsService
{
    private readonly IRepository<Person> _personRepository;
    private readonly IRepository<Employment> _employmentRepository;
    private readonly IRepository<School> _schoolRepository;
    private readonly IRepository<Position> _positionRepository;
    private readonly IRepository<SalaryGrade> _salaryGradeRepository;
    private readonly IRepository<Item> _itemRepository;
    
    public async Task<DashboardStatsDto> GetDashboardStatsAsync()
    {
        return new DashboardStatsDto
        {
            TotalPersons = await _personRepository.Query().CountAsync(),
            ActiveEmployments = await _employmentRepository.Query()
                .Where(e => e.IsActive)
                .CountAsync(),
            TotalSchools = await _schoolRepository.Query().CountAsync(),
            TotalPositions = await _positionRepository.Query().CountAsync(),
            TotalSalaryGrades = await _salaryGradeRepository.Query().CountAsync(),
            TotalItems = await _itemRepository.Query().CountAsync()
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

// Entity Services
builder.Services.AddScoped<IPersonService, PersonService>();
builder.Services.AddScoped<ISchoolService, SchoolService>();
builder.Services.AddScoped<IPositionService, PositionService>();
builder.Services.AddScoped<ISalaryGradeService, SalaryGradeService>();
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<IEmploymentService, EmploymentService>();

// Document Services
builder.Services.AddScoped<IBlobStorageService, BlobStorageService>();
builder.Services.AddScoped<IDocumentService, DocumentService>();

// Reports
builder.Services.AddScoped<IReportsService, ReportsService>();
```

---

## Service Patterns

### 1. DTO Mapping

Services handle all DTO mapping internally:

```csharp
private PersonResponseDto MapToResponseDto(Person entity)
{
    return new PersonResponseDto
    {
        DisplayId = entity.DisplayId,
        FirstName = entity.FirstName,
        // ... other properties
    };
}

private PersonListDto MapToListDto(Person entity)
{
    return new PersonListDto
    {
        DisplayId = entity.DisplayId,
        FullName = entity.FullName,
        // ... other properties
    };
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
    
    return person == null ? null : MapToResponseDto(person);
}
```
