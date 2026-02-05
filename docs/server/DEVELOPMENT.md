# Development Guide

This document provides guidelines for developing and contributing to the EMS backend.

---

## Prerequisites

- **.NET 10.0 SDK** or higher
- **SQL Server** (LocalDB, SQL Server, or Azure SQL)
- **Azure Storage Account** (for blob storage)
- **Visual Studio 2022** or **VS Code** with C# extension
- **Google Cloud Console** project with OAuth2 credentials

---

## Getting Started

### 1. Clone and Setup

```bash
git clone <repository-url>
cd ems-v2/server
```

### 2. Configure Application Settings

Create or update `appsettings.json` with the required configuration:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=EmployeeManagementSystem;Trusted_Connection=True;",
    "BlobStorage": "UseDevelopmentStorage=true"
  },
  "Authentication": {
    "Jwt": {
      "Secret": "your-super-secret-key-at-least-32-characters-long",
      "Issuer": "EmployeeManagementSystem",
      "Audience": "EmployeeManagementSystem",
      "AccessTokenExpirationMinutes": 15,
      "RefreshTokenExpirationDays": 7
    },
    "Google": {
      "ClientId": "your-google-client-id.apps.googleusercontent.com",
      "ClientSecret": "your-google-client-secret"
    }
  }
}
```

For local blob storage, install [Azurite](https://docs.microsoft.com/en-us/azure/storage/common/storage-use-azurite).

### 3. Google OAuth2 Setup

1. Go to [Google Cloud Console](https://console.cloud.google.com/)
2. Create a new project or select existing
3. Navigate to **APIs & Services** > **Credentials**
4. Create **OAuth 2.0 Client ID** (Web application)
5. Add authorized redirect URIs:
   - `https://localhost:7166/swagger/oauth2-redirect.html` (Swagger)
6. Copy Client ID and Client Secret to `appsettings.json`

### 4. Apply Migrations

```bash
dotnet ef database update --project EmployeeManagementSystem.Infrastructure --startup-project EmployeeManagementSystem.Api
```

### 5. Run the Application

```bash
dotnet run --project EmployeeManagementSystem.Api
```

The API will be available at:
- HTTPS: `https://localhost:7166`
- Swagger: `https://localhost:7166/swagger`

---

## Project Structure

```
server/
├── EmployeeManagementSystem.slnx          # Solution file
├── EmployeeManagementSystem.Api/          # Presentation layer
│   ├── Program.cs                         # Application entry point
│   ├── appsettings.json                   # Configuration
│   ├── v1/Controllers/                    # API v1 controllers
│   ├── v2/Controllers/                    # API v2 controllers
│   └── Properties/launchSettings.json     # Launch profiles
├── EmployeeManagementSystem.Application/  # Application layer
│   ├── DTOs/                              # Data transfer objects
│   │   └── Auth/                          # Authentication DTOs
│   ├── Interfaces/                        # Service interfaces
│   └── Services/                          # Service implementations
├── EmployeeManagementSystem.Domain/       # Domain layer
│   ├── Entities/                          # Domain entities
│   └── Enums/                             # Domain enums
├── EmployeeManagementSystem.Infrastructure/ # Infrastructure layer
│   ├── Data/                              # DbContext, configurations
│   ├── Migrations/                        # EF Core migrations
│   ├── Repositories/                      # Repository implementations
│   └── Services/                          # External service implementations
└── tests/
    └── EmployeeManagementSystem.Tests/    # Unit & integration tests
```

---

## Development Workflow

### Adding a New Entity

1. **Create Entity** in Domain layer:
   ```csharp
   // Domain/Entities/NewEntity.cs
   public class NewEntity : BaseEntity
   {
       public string Name { get; set; }
       public bool IsActive { get; set; } = true;
   }
   ```

2. **Add DbSet** to ApplicationDbContext:
   ```csharp
   public DbSet<NewEntity> NewEntities { get; set; }
   ```

3. **Create Configuration**:
   ```csharp
   // Infrastructure/Data/Configurations/NewEntityConfiguration.cs
   public class NewEntityConfiguration : IEntityTypeConfiguration<NewEntity>
   {
       public void Configure(EntityTypeBuilder<NewEntity> builder)
       {
           builder.ToTable("NewEntities");
           builder.HasKey(e => e.Id);
           builder.HasIndex(e => e.DisplayId).IsUnique();
           builder.Property(e => e.Name).IsRequired().HasMaxLength(100);
       }
   }
   ```

4. **Create Migration**:
   ```bash
   dotnet ef migrations add AddNewEntity --project EmployeeManagementSystem.Infrastructure --startup-project EmployeeManagementSystem.Api
   ```

5. **Create DTOs**:
   ```csharp
   // Application/DTOs/NewEntity/CreateNewEntityDto.cs
   public class CreateNewEntityDto { ... }
   
   // Application/DTOs/NewEntity/NewEntityResponseDto.cs
   public class NewEntityResponseDto : BaseResponseDto { ... }
   ```

6. **Create Service Interface**:
   ```csharp
   // Application/Interfaces/INewEntityService.cs
   public interface INewEntityService
   {
       Task<NewEntityResponseDto?> GetByDisplayIdAsync(long displayId);
       Task<PagedResult<NewEntityResponseDto>> GetPagedAsync(int page, int pageSize, string? search);
       Task<NewEntityResponseDto> CreateAsync(CreateNewEntityDto dto);
       Task<NewEntityResponseDto?> UpdateAsync(long displayId, UpdateNewEntityDto dto);
       Task<bool> DeleteAsync(long displayId);
   }
   ```

7. **Implement Service**:
   ```csharp
   // Application/Services/NewEntityService.cs
   public class NewEntityService : INewEntityService { ... }
   ```

8. **Register Service** in Program.cs:
   ```csharp
   builder.Services.AddScoped<INewEntityService, NewEntityService>();
   ```

9. **Create Controller**:
   ```csharp
   // Api/v1/Controllers/NewEntitiesController.cs
   [ApiController]
   [Route("api/v1/[controller]")]
   public class NewEntitiesController : ControllerBase { ... }
   ```

---

### Adding a New Endpoint

1. **Add method to service interface**
2. **Implement method in service**
3. **Add action to controller**
4. **Test with Swagger**

---

## Coding Standards

### Modern C# Features

The codebase uses modern C# 12+ features:

**Primary Constructors** (C# 12):
```csharp
// Modern approach - used in all controllers and services
public class PersonService(IRepository<Person> repository) : IPersonService
{
    public async Task<PersonResponseDto?> GetByDisplayIdAsync(long displayId)
    {
        var person = await repository.GetByDisplayIdAsync(displayId);
        return person?.MapToResponseDto();
    }
}
```

**Collection Expressions** (C# 12):
```csharp
// Modern collection initialization
var items = [item1, item2, item3];
var numbers = [1, 2, 3, 4, 5];
```

**Record Types** (C# 9+):
```csharp
// Used for DTOs and immutable data
public record PersonResponseDto
{
    public long DisplayId { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
}
```

### API Routes

All API routes follow REST conventions with lowercase URLs:
- `/api/v1/persons`
- `/api/v1/employments`
- `/api/v1/salarygrades` (single word, no hyphens)
- `/api/v1/schools`
- `/api/v1/positions`
- `/api/v1/items`
- `/api/v1/reports`

This is configured in `Program.cs`:
```csharp
builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = false;
});
```

### Naming Conventions

|      Type      | Convention  |       Example         |
|----------------|-------------|-----------------------|
| Classes        | PascalCase  | `PersonService`       |
| Interfaces     | IPascalCase | `IPersonService`      |
| Methods        | PascalCase  | `GetByDisplayIdAsync` |
| Properties     | PascalCase  | `FirstName`           |
| Parameters     | camelCase   | `displayId`           |
| Private fields | _camelCase  | `_repository`         |
| Constants      | PascalCase  | `MaxPageSize`         |

### Async/Await

- All I/O operations should be async
- Suffix async methods with `Async`
- Use `ConfigureAwait(false)` in library code

```csharp
public async Task<PersonResponseDto?> GetByDisplayIdAsync(long displayId)
{
    var person = await _repository.GetByDisplayIdAsync(displayId);
    return person == null ? null : MapToResponseDto(person);
}
```

### Null Handling

Use nullable reference types and null-conditional operators:

```csharp
public async Task<PersonResponseDto?> GetByDisplayIdAsync(long displayId)
{
    var person = await _repository.GetByDisplayIdAsync(displayId);
    return person?.MapToResponseDto();  // Returns null if person is null
}
```

### Exception Handling

Let exceptions propagate to controllers; use global error handling:

```csharp
// In service - let exception propagate
var person = await _repository.GetByDisplayIdAsync(displayId)
    ?? throw new InvalidOperationException($"Person {displayId} not found");

// In controller - return appropriate status
[HttpGet("{displayId}")]
public async Task<ActionResult<PersonResponseDto>> Get(long displayId)
{
    var result = await _service.GetByDisplayIdAsync(displayId);
    return result == null ? NotFound() : Ok(result);
}
```

---

## Testing

### Test Project Structure

```
tests/EmployeeManagementSystem.Tests/
├── Unit/
│   ├── Services/
│   │   ├── PersonServiceTests.cs
│   │   └── EmploymentServiceTests.cs
│   └── Controllers/
│       └── PersonsControllerTests.cs
└── Integration/
    └── Api/
        └── PersonsApiTests.cs
```

### Unit Test Example

```csharp
public class PersonServiceTests
{
    private readonly Mock<IRepository<Person>> _mockRepository;
    private readonly PersonService _service;
    
    public PersonServiceTests()
    {
        _mockRepository = new Mock<IRepository<Person>>();
        _service = new PersonService(_mockRepository.Object);
    }
    
    [Fact]
    public async Task GetByDisplayIdAsync_ExistingPerson_ReturnsPersonDto()
    {
        // Arrange
        var person = new Person
        {
            DisplayId = 123456789012,
            FirstName = "John",
            LastName = "Doe"
        };
        _mockRepository.Setup(r => r.GetByDisplayIdAsync(123456789012))
            .ReturnsAsync(person);
        
        // Act
        var result = await _service.GetByDisplayIdAsync(123456789012);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal("John", result.FirstName);
        Assert.Equal("Doe", result.LastName);
    }
    
    [Fact]
    public async Task GetByDisplayIdAsync_NonExistingPerson_ReturnsNull()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByDisplayIdAsync(It.IsAny<long>()))
            .ReturnsAsync((Person?)null);
        
        // Act
        var result = await _service.GetByDisplayIdAsync(999999999999);
        
        // Assert
        Assert.Null(result);
    }
}
```

### Running Tests

```bash
cd tests/EmployeeManagementSystem.Tests
dotnet test
```

With coverage:

```bash
dotnet test --collect:"XPlat Code Coverage"
```

---

## Database Operations

### Creating Migrations

```bash
dotnet ef migrations add MigrationName \
  --project EmployeeManagementSystem.Infrastructure \
  --startup-project EmployeeManagementSystem.Api
```

### Applying Migrations

```bash
dotnet ef database update \
  --project EmployeeManagementSystem.Infrastructure \
  --startup-project EmployeeManagementSystem.Api
```

### Reverting Migrations

```bash
dotnet ef database update PreviousMigrationName \
  --project EmployeeManagementSystem.Infrastructure \
  --startup-project EmployeeManagementSystem.Api
```

### Removing Last Migration

```bash
dotnet ef migrations remove \
  --project EmployeeManagementSystem.Infrastructure \
  --startup-project EmployeeManagementSystem.Api
```

### Generating SQL Script

```bash
dotnet ef migrations script \
  --project EmployeeManagementSystem.Infrastructure \
  --startup-project EmployeeManagementSystem.Api \
  -o migration.sql
```

---

## API Documentation

### Swagger Annotations

Use XML comments for API documentation:

```csharp
/// <summary>
/// Gets a person by their display ID.
/// </summary>
/// <param name="displayId">The 12-digit display ID</param>
/// <returns>The person details</returns>
/// <response code="200">Returns the person</response>
/// <response code="404">Person not found</response>
[HttpGet("{displayId}")]
[ProducesResponseType(typeof(PersonResponseDto), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<ActionResult<PersonResponseDto>> Get(long displayId)
{
    // ...
}
```

### Enable XML Documentation

In `.csproj`:

```xml
<PropertyGroup>
  <GenerateDocumentationFile>true</GenerateDocumentationFile>
  <NoWarn>$(NoWarn);1591</NoWarn>
</PropertyGroup>
```

In `Program.cs`:

```csharp
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});
```

---

## Debugging

### Launch Profiles

**launchSettings.json:**
```json
{
  "profiles": {
    "https": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "launchUrl": "swagger",
      "applicationUrl": "https://localhost:7166",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

### Logging

Configure logging in `appsettings.Development.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore.Database.Command": "Information"
    }
  }
}
```

---

## Troubleshooting

### Common Issues

**Database connection failed:**
- Check connection string in appsettings
- Ensure SQL Server is running
- Verify database exists

**Migration errors:**
- Clear bin/obj folders
- Rebuild solution
- Check for pending model changes

**CORS errors:**
- Verify CORS policy in Program.cs
- Check frontend origin URL

**Blob storage errors:**
- Verify connection string
- Check container permissions
- Ensure Azurite is running (for local dev)

### Useful Commands

```bash
# Clean and rebuild
dotnet clean
dotnet build

# Check for outdated packages
dotnet list package --outdated

# Update packages
dotnet add package PackageName

# View EF migrations
dotnet ef migrations list --project EmployeeManagementSystem.Infrastructure --startup-project EmployeeManagementSystem.Api
```
