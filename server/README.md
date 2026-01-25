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

## Configuration

Configuration is managed through:
- `appsettings.json` - Base configuration
- `appsettings.Development.json` - Development overrides
- User Secrets - Sensitive data (connection strings, API keys)
