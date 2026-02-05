# GitHub Copilot Instructions for Employee Management System

## Project Overview

**Employee Management System** is a comprehensive full-stack application for managing employees, persons, schools, and related records:
- **Backend**: Clean Architecture ASP.NET Core Web API using Entity Framework Core, Azure Blob Storage and SQL database
- **Frontend**: Modern desktop and web application built with React, TypeScript and Vite


### Technology Stack

**Backend**:
- **ASP.NET Core** - Web API framework
- **Clean Architecture** - Layered approach with strict separation of concerns
- **Swagger/OpenAPI** - API documentation
- **Entity Framework Core** - SQL database
- **Azure Blob Storage** - File storage
- **xUnit** - Testing framework

**Frontend**:
- **React** - UI framework
- **CSS** - Styling
- **TypeScript** - Static typing
- **Vite** - Build tool
- **Chakra-UI** - Component library
- **Apollo Client** - GraphQL client
- **GraphQL Code Generator** - Auto-generated types and hooks
- **AG Grid** - Data grid component

**Gateway**:
- **HotChocolate** - GraphQL server for .NET
- **Redis** - Caching layer


### Folder Structure
```
ems-v2/
├── .github/                                        # GitHub configuration files and copilot instructions
├── server/                                         # Backend API (ASP.NET Core)
│   ├── EmployeeManagementSystem.Domain/            # Entities and domain logic
│   ├── EmployeeManagementSystem.Application/       # Business logic and DTOs
│   ├── EmployeeManagementSystem.Infrastructure/    # Data access and external services
│   ├── EmployeeManagementSystem.Api/               # API controllers and middleware
│   │   ├── v1/                                     # Version 1 of the API
│   │   ├── v2/                                     # Version 2 of the API
│   ├── EmployeeManagementSystem.ApiClient/         # NSwag-generated API client for Gateway
│   │   ├── Generated/                              # Auto-generated client code (DO NOT EDIT)
│   │   └── nswag.json                              # NSwag configuration
│   └── tests/
│       ├── EmployeeManagementSystem.Tests/         # Unit and integration tests
├── gateway/                                        # GraphQL Gateway (HotChocolate)
│   └── EmployeeManagementSystem.Gateway/           # GraphQL types, queries, mutations
│       ├── Types/                                  # Query.cs, Mutation.cs, TypeExtensions.cs
│       ├── Controllers/                            # REST proxy controllers for file operations
│       ├── Caching/                                # Redis caching service and keys
│       ├── DataLoaders/                            # HotChocolate DataLoaders for batching
│       └── Mappings/                               # Input type to DTO mappings
├── application/                                    # Frontend Application (React/TypeScript/Vite/Chakra-UI)
│   └── src/                                        # Source code
│       ├── components/                             # Reusable UI components
│       ├── graphql/                                # GraphQL operations and generated code
│       │   ├── operations/                         # .graphql query/mutation files
│       │   └── generated/                          # Auto-generated types and hooks
│       ├── hooks/                                  # Custom React hooks for GraphQL
│       └── contexts/                               # React context providers (AuthContext)
├── docs/                                           # Shared documentation
│   ├── server/                                     # Backend related documentation
│   └── application/                                # Frontend related documentation
```


### Coding Standards
- Use semicolons at the end of each statement.
- Use single quotes for strings.
- Use function based components in React.
- Use arrow functions for callbacks.


### Best Practices
- Write unit tests for all new features and bug fixes.


### DTO Guidelines (Data Transfer Objects)

This project uses a **hybrid approach** for DTOs combining C# records and classes:

#### Use `record` types for:
- **Response DTOs** (`*ResponseDto`, `*ListDto`) - Data returned from API endpoints
- **Read-only DTOs** - Any DTO that represents immutable data snapshots
- **Nested response objects** - Simplified DTOs embedded in response types

#### Use `class` types for:
- **Input DTOs** (`Create*Dto`, `Update*Dto`, `Upsert*Dto`) - Data received from API requests
- **Query DTOs** (`*PaginationQuery`, `*Query`) - DTOs with setter validation logic
- **DTOs with Stream properties** - Records don't work well with disposable resources

#### Record Benefits:
- Built-in value equality (two records with same values are equal)
- Immutability by default (use `init` accessors)
- Built-in `ToString()` for debugging
- Non-destructive mutation with `with` expressions

#### Pattern Examples:
```csharp
// Response DTO - use record
public record PersonResponseDto : BaseResponseDto
{
    public string FirstName { get; init; } = string.Empty;
    public IReadOnlyList<AddressResponseDto> Addresses { get; init; } = [];
}

// Input DTO - use class (for model binding)
public class CreatePersonDto
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;
}

// Query DTO - use class (for setter validation)
public class PersonPaginationQuery : PaginationQuery
{
    public Gender? Gender { get; set; }
}
```

#### Conventions:
- Response records inherit from `BaseResponseDto` (abstract record)
- Use `IReadOnlyList<T>` for collections in response records
- Use `List<T>` for collections in input classes
- Use `init` accessors in records, `set` accessors in classes
- XML documentation: use "Gets" for record properties, "Gets or sets" for class properties


### Entity to DTO Mapping Guidelines

This project uses **extension methods** for mapping entities to DTOs, located in `EmployeeManagementSystem.Application/Mappings/`.

#### Naming Conventions:
- `ToResponseDto()` - Maps entity to full response DTO
- `ToListDto()` - Maps entity to simplified list DTO
- `ToDto()` - Maps entity to a DTO (for simple cases like `User.ToDto()`)
- `ToResponseDtoList()` - Maps collection of entities to list of response DTOs

#### Location:
All mapping extensions are in `EmployeeManagementSystem.Application/Mappings/`:
- `AddressMappingExtensions.cs`
- `ContactMappingExtensions.cs`
- `PersonMappingExtensions.cs`
- `EmploymentMappingExtensions.cs`
- `SchoolMappingExtensions.cs`
- `PositionMappingExtensions.cs`
- `SalaryGradeMappingExtensions.cs`
- `ItemMappingExtensions.cs`
- `DocumentMappingExtensions.cs`
- `UserMappingExtensions.cs`

#### Usage Examples:
```csharp
// In services, use extension methods instead of private MapTo methods
using EmployeeManagementSystem.Application.Mappings;

// Single entity mapping
var dto = person.ToResponseDto();
var userDto = user.ToDto();

// Collection mapping
var addressDtos = addresses.ToResponseDtoList();

// Inline with LINQ
var dtos = persons.Select(p => p.ToResponseDto()).ToList();
```

#### Creating New Mappings:
When adding a new entity, create a corresponding mapping extension class:
```csharp
namespace EmployeeManagementSystem.Application.Mappings;

public static class NewEntityMappingExtensions
{
    public static NewEntityResponseDto ToResponseDto(this NewEntity entity)
    {
        return new NewEntityResponseDto
        {
            // Map properties
        };
    }
}
```

#### Benefits:
- **Reusability**: Same mapping logic shared across all services
- **Discoverability**: IntelliSense shows available mappings on entities
- **Testability**: Mapping logic can be unit tested independently
- **Clean services**: No private `MapToXxx` methods cluttering service classes


### API Client Generation

**NSwag (Backend → Gateway)**:
The `EmployeeManagementSystem.ApiClient` project contains the NSwag-generated client used by the Gateway to communicate with the Backend API.

```bash
# Regenerate the API client (requires API to be running)
cd server/EmployeeManagementSystem.ApiClient
nswag run nswag.json
```

**GraphQL Code Generator (Gateway → Frontend)**:
The frontend uses GraphQL Code Generator to create typed queries, mutations, and hooks from the Gateway schema.

```bash
# Regenerate frontend GraphQL types (requires Gateway to be running)
cd application
npm run codegen
```


### Gateway Architecture

The GraphQL Gateway serves as the BFF (Backend-for-Frontend) layer:

**GraphQL Types** (`gateway/.../Types/`):
- `Query.cs` - All GraphQL queries with Redis caching
- `Mutation.cs` - All GraphQL mutations with cache invalidation
- `TypeExtensions.cs` - GraphQL type extensions for nested resolvers

**REST Controllers** (`gateway/.../Controllers/`):
- `ProfileImageController.cs` - Proxy for profile image operations
- These REST controllers exist because GraphQL doesn't natively support binary file operations
- Frontend calls these endpoints directly for file operations

**DataLoaders** (`gateway/.../DataLoaders/`):
- Batch and cache individual entity fetches to prevent N+1 queries
- Example: `PersonDataLoader`, `EmploymentDataLoader`, `SchoolDataLoader`
- Each DataLoader checks Redis cache first, then batches API calls

**Caching** (`gateway/.../Caching/`):
- `IRedisCacheService` - Redis cache interface with GetAsync, SetAsync, RemoveAsync methods
- `RedisCacheService` - Implementation using StackExchange.Redis and IDistributedCache
- `CacheKeys` - Centralized cache key generation with hash-based approach
  - Individual entities: `person:123`, `school:456`
  - List queries: `persons:list:a3f2e1b4c5d6e7f8` (SHA256 hash of all filter parameters)
  - Hash includes ALL filter parameters (pageNumber, pageSize, searchTerm, sorting, filtering)
  - First 16 characters of hash used for brevity while maintaining uniqueness

**Cache Key Generation Pattern:**
```csharp
// Include ALL filter parameters in cache key
string cacheKey = CacheKeys.PersonsList(
    pageNumber,
    pageSize,
    searchTerm,
    fullNameFilter,     // Don't omit any filters!
    displayIdFilter,
    gender,
    civilStatus,
    sortBy,
    sortDescending
);

// CacheKeys.PersonsList internally:
// 1. Serializes all parameters to JSON
// 2. Generates SHA256 hash
// 3. Returns prefix + first 16 chars of hash
```

**Cache Invalidation:**
- After mutations (create/update/delete), invalidate related cache entries
- Use `RemoveByPrefixAsync()` for list caches
- Use `RemoveAsync()` for individual entity caches


### UI guidelines
- A toggle is provided to switch between light and dark mode.
- Application should have a modern and clean design.
- Use Chakra-UI components for consistent styling.
- Ensure the application is responsive and works well on different screen sizes.
- Use AG Grid for displaying tabular data with features like sorting, filtering, and pagination. See reference: https://www.ag-grid.com/example-inventory/


### Security Guidelines

This project follows security best practices:

**Authentication & Authorization:**
- JWT Bearer tokens with 15-minute expiration
- Refresh tokens stored in HttpOnly cookies (7-day expiration)
- All token validation flags enabled (issuer, audience, lifetime, signing key)
- Clock skew set to zero for strict expiration enforcement
- Google OAuth2 for user authentication

**API Security:**
- `[Authorize]` attribute on all protected endpoints
- CORS configured with specific allowed origins (no wildcards in production)
- Input validation with Data Annotations
- Entity Framework Core (automatic parameterization prevents SQL injection)

**Data Protection:**
- Soft deletes (IsDeleted flag) prevent accidental data loss
- Audit trail (CreatedBy, ModifiedBy, timestamps) on all entities
- Azure Blob Storage for file uploads
- Redis caching with TTL

**Frontend Security:**
- React auto-escaping prevents XSS
- No use of `dangerouslySetInnerHTML` without sanitization
- TypeScript for type safety
- Token auto-refresh mechanism

**Secrets Management:**
- Development: User secrets (`dotnet user-secrets`)
- Production: Environment variables or Azure Key Vault
- Never commit secrets to repository
- `.env` files in `.gitignore`

**Security Documentation:**
- See `docs/SECURITY.md` for comprehensive security guidelines
- See `docs/DEPLOYMENT.md` for production deployment checklist


### References
- [ASP.NET Core Documentation](https://learn.microsoft.com/en-us/aspnet/core/?view=aspnetcore-10.0)
- [React Reference](https://react.dev/reference/react)
- [TypeScript Documentation](https://www.typescriptlang.org/docs/handbook/)
- [Vite Documentation](https://vite.dev/guide/)
- [Chakra-UI Documentation](https://chakra-ui.com/docs/get-started/installation)
- [Apollo Client Documentation](https://www.apollographql.com/docs/react/)
- [GraphQL Code Generator Documentation](https://the-guild.dev/graphql/codegen)
- [HotChocolate GraphQL Documentation](https://chillicream.com/docs/hotchocolate)
- [Clean Architecture Principles](https://8thlight.com/blog/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Entity Framework Core Documentation](https://learn.microsoft.com/en-us/ef/core/)
- [Azure Blob Storage Documentation](https://learn.microsoft.com/en-us/azure/storage/blobs/)
- [xUnit Documentation](https://xunit.net/docs/getting-started/netcore/cmdline)
- [AG Grid Documentation](https://www.ag-grid.com/react-data-grid/getting-started/)
- [NSwag Documentation](https://github.com/RicoSuter/NSwag)
- [StackExchange.Redis Documentation](https://stackexchange.github.io/StackExchange.Redis/)
