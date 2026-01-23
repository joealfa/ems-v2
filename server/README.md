# Employee Management System - Backend

ASP.NET Core Web API built with Clean Architecture principles.

## Architecture

The backend follows Clean Architecture with the following layers:

### Domain Layer (`EmployeeManagementSystem.Domain`)
- Contains core business entities and domain logic
- No dependencies on other layers
- Framework-independent

### Application Layer (`EmployeeManagementSystem.Application`)
- Contains business logic and use cases
- Defines interfaces for external services
- Depends only on Domain layer
- Contains DTOs and mappings

### Infrastructure Layer (`EmployeeManagementSystem.Infrastructure`)
- Implements data access using Entity Framework Core
- Implements external service integrations (Azure Blob Storage)
- Depends on Application layer

### API Layer (`EmployeeManagementSystem.Api`)
- Contains controllers and middleware
- Handles HTTP requests/responses
- API versioning (v1, v2)
- Swagger/OpenAPI documentation

## Building and Running

```bash
# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Run the API
cd EmployeeManagementSystem.Api
dotnet run

# Run tests
cd ../tests/EmployeeManagementSystem.Tests
dotnet test
```

## API Documentation

Once running, access Swagger UI at: `https://localhost:5001/swagger`

## Database Setup

Coming soon...

## Configuration

Coming soon...
