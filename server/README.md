# Employee Management System - Backend

ASP.NET Core Web API built with Clean Architecture principles using .NET 10.

## Architecture

The backend follows Clean Architecture with the following layers:

### Domain Layer (`EmployeeManagementSystem.Domain`)
- Contains core business entities and domain logic
- No dependencies on other layers
- Framework-independent

**Entities:**
- `Person` - Core person information
- `Employment` - Employment records
- `Position` - Job positions
- `SalaryGrade` - Salary grade definitions
- `School` - Educational institutions
- `Document` - Document attachments
- `Item` - Inventory items
- `Address`, `Contact` - Supporting entities
- `User`, `RefreshToken` - Authentication entities

**Enums:**
- `AddressType`, `ContactType`, `DocumentType`
- `Gender`, `CivilStatus`, `Eligibility`
- `EmploymentStatus`, `AppointmentStatus`

### Application Layer (`EmployeeManagementSystem.Application`)
- Contains business logic and use cases
- Defines interfaces for external services
- Depends only on Domain layer
- Contains DTOs and mappings

**Services:**
- `PersonService` - Person management
- `EmploymentService` - Employment records
- `PositionService` - Position management
- `SalaryGradeService` - Salary grades
- `SchoolService` - School management
- `DocumentService` - Document handling
- `ItemService` - Item management
- `ReportsService` - Reporting

**Interfaces:**
- `IRepository<T>` - Generic repository pattern
- `IAuthService` - Authentication
- `IBlobStorageService` - Azure Blob Storage

### Infrastructure Layer (`EmployeeManagementSystem.Infrastructure`)
- Implements data access using Entity Framework Core
- Implements external service integrations
- Depends on Application layer

**Components:**
- `ApplicationDbContext` - EF Core DbContext
- `Repository<T>` - Generic repository implementation
- `AuthService` - JWT authentication
- `BlobStorageService` - Azure Blob Storage integration
- `DataSeeder` - Database seeding

### API Layer (`EmployeeManagementSystem.Api`)
- Contains controllers and middleware
- Handles HTTP requests/responses
- API versioning (v1, v2)
- Swagger/OpenAPI documentation
- Uses lowercase routes for REST API best practices

**Controllers (v1):**
- `AuthController` - Authentication endpoints
- `PersonsController` - Person CRUD operations
- `EmploymentsController` - Employment management
- `PositionsController` - Position management
- `SalaryGradesController` - Salary grade operations
- `SchoolsController` - School management
- `DocumentsController` - Document uploads/downloads
- `ItemsController` - Item management
- `ReportsController` - Report generation

### API Client Layer (`EmployeeManagementSystem.ApiClient`)
- NSwag-generated HTTP client for the API
- Used by the GraphQL Gateway to communicate with the Backend
- Auto-generated from OpenAPI specification

**Components:**
- `Generated/EmsApiClient.cs` - Auto-generated client (DO NOT EDIT)
- `nswag.json` - NSwag configuration file

## Project Structure

```
server/
├── EmployeeManagementSystem.Domain/
│   ├── Entities/           # Domain entities
│   └── Enums/              # Domain enumerations
├── EmployeeManagementSystem.Application/
│   ├── Common/             # Shared utilities (Result pattern)
│   ├── DTOs/               # Data transfer objects
│   ├── Interfaces/         # Service interfaces
│   ├── Mappings/           # Entity to DTO mapping extensions
│   └── Services/           # Business logic services
├── EmployeeManagementSystem.Infrastructure/
│   ├── Data/               # DbContext and seeding
│   ├── Migrations/         # EF Core migrations
│   ├── Repositories/       # Repository implementations
│   └── Services/           # External service implementations
├── EmployeeManagementSystem.Api/
│   ├── Controllers/        # Base controller
│   ├── v1/Controllers/     # API v1 endpoints
│   ├── v2/                 # API v2 endpoints (future)
│   └── Properties/         # Launch settings
├── EmployeeManagementSystem.ApiClient/
│   ├── Generated/          # NSwag-generated client (DO NOT EDIT)
│   └── nswag.json          # NSwag configuration
└── tests/
    └── EmployeeManagementSystem.Tests/
```

## Building and Running

```bash
# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Run the API
cd EmployeeManagementSystem.Api
dotnet run

# Run with HTTPS profile
dotnet run --launch-profile https

# Run tests
cd ../tests/EmployeeManagementSystem.Tests
dotnet test
```

## API Documentation

Once running, access Swagger UI at: `https://localhost:5001/swagger`

## Dependencies

- **Microsoft.AspNetCore.Authentication.JwtBearer** - JWT authentication
- **Microsoft.AspNetCore.OpenApi** - OpenAPI support
- **Swashbuckle.AspNetCore** - Swagger documentation
- **Microsoft.EntityFrameworkCore** - ORM
- **NSwag.ApiDescription.Client** - API client generation
- **Serilog.AspNetCore** - Structured logging framework
- **Serilog.Sinks.Seq** - Seq sink for centralized log monitoring
- **Serilog.Sinks.Async** - Async logging for better performance
- **Serilog.Enrichers.*** - Log enrichment (Machine, Thread, Environment)

## Modern C# Features

The codebase leverages modern C# 12+ features:
- **Primary Constructors** - Used in all controllers and services for cleaner dependency injection
- **Collection Expressions** - Simplified collection initialization
- **Lowercase URLs** - Configured routing for REST API best practices
- **Record Types** - Used for DTOs and immutable data structures

## Configuration

Configuration is managed through:
- `appsettings.json` - Base configuration (including Serilog settings)
- `appsettings.Development.json` - Development overrides
- User Secrets - Sensitive data (connection strings, API keys, Seq API key)

### Logging

The backend uses **Serilog** for structured logging with **Seq** as the centralized logging platform:

- **Console Sink**: Local development output with colored themes
- **Seq Sink**: Centralized log aggregation at `http://localhost:5341`
- **Async Sinks**: Non-blocking logging for better performance
- **Enrichers**: Automatic context (MachineName, ThreadId, EnvironmentName, UserId)
- **Request Logging**: Structured HTTP request/response logging

See [Logging Documentation](../docs/server/LOGGING.md) for detailed information.

## Regenerating API Client

The NSwag-generated API client is used by the Gateway. To regenerate it after API changes:

```bash
# Ensure the API is running first
cd EmployeeManagementSystem.Api
dotnet run --launch-profile https

# In another terminal, regenerate the client
cd ../EmployeeManagementSystem.ApiClient
nswag run nswag.json
```

The client is generated from the OpenAPI specification at `https://localhost:7166/openapi/v1.json`.
