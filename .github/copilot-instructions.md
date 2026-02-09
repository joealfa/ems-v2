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
- **RabbitMQ** - Event publishing (Producer)
- **Serilog + Seq** - Structured logging
- **xUnit** - Testing framework

**Frontend**:
- **React** - UI framework
- **CSS** - Styling
- **TypeScript** - Static typing
- **Vite** - Build tool
- **Chakra-UI** - Component library
- **TanStack Query** - Server state management and data fetching
- **graphql-request** - Lightweight GraphQL client
- **graphql-ws** - WebSocket client for GraphQL subscriptions
- **GraphQL Code Generator** - Auto-generated types and documents
- **AG Grid** - Data grid component

**Gateway**:
- **HotChocolate** - GraphQL server for .NET
- **Redis** - Caching layer
- **RabbitMQ** - Event consumption (Consumer) for cache invalidation
- **GraphQL Subscriptions** - WebSocket support for real-time updates
- **Serilog + Seq** - Structured logging


### Folder Structure
```
ems-v2/
├── .github/                                        # GitHub configuration files and copilot instructions
├── server/                                         # Backend API (ASP.NET Core)
│   ├── EmployeeManagementSystem.Domain/            # Entities and domain logic
│   ├── EmployeeManagementSystem.Application/       # Business logic and DTOs
│   ├── EmployeeManagementSystem.Infrastructure/    # Data access, external services, RabbitMQ publisher
│   │   ├── Messaging/                              # Event publishing (ActivityPersistingEventPublisher + RabbitMQ)
│   │   └── Repositories/                           # Repository implementations (incl. RecentActivityRepository)
│   ├── EmployeeManagementSystem.Api/               # API controllers and middleware
│   │   └── v1/                                     # API v1 controllers
│   ├── EmployeeManagementSystem.ApiClient/         # NSwag-generated API client for Gateway
│   │   ├── Generated/                              # Auto-generated client code (DO NOT EDIT)
│   │   └── nswag.json                              # NSwag configuration
│   ├── scripts/                                    # SQL and setup scripts
│   │   ├── create-database.sql                     # Database creation script
│   │   ├── seed-data.sql                           # Mock data seed (5,000 persons)
│   │   └── setup-rabbitmq-queues.ps1               # RabbitMQ setup script
│   └── tests/
│       ├── EmployeeManagementSystem.Tests/         # Unit and integration tests
├── gateway/                                        # GraphQL Gateway (HotChocolate), subscriptions
│   └── EmployeeManagementSystem.Gateway/
│       ├── Types/                                  # Query.cs, Mutation.cs, Subscription.cs
│       │   └── Extensions/                         # TypeExtensions.cs
│       ├── Controllers/                            # REST proxy controllers for file operations
│       ├── Caching/                                # RedisCacheService and CacheKeys
│       ├── DataLoaders/                            # HotChocolate DataLoaders for batching
│       ├── Messaging/                              # RabbitMQ event consumer (Consumer)
│       ├── Services/                               # ActivityEventBuffer for subscription buffering
│       └── Mappings/                               # Input type to DTO mappings
├── application/                                    # Frontend Application (React/TypeScript/Vite/Chakra-UI)
│   └── src/                                        # Source code
│       ├── components/                             # Reusable UI components
│       ├── graphql/                                # GraphQL operations and generated code
│       │   ├── subscription-client.ts              # graphql-ws WebSocket client
│       │   ├── query-client.ts                     # TanStack QueryClient configuration
│       │   ├── QueryProvider.tsx                    # TanStack Query provider wrapper
│       │   ├── query-keys.ts                       # Query key factory for cache management
│       │   ├── operations/                         # .graphql query/mutation/subscription files
│       │   └── generated/                          # Auto-generated types and documents
│       ├── hooks/                                  # Custom React hooks (TanStack Query, subscriptions)
│       ├── utils/                                  # Utility functions (formatters, helpers, mappers)
│       │   ├── index.ts                            # Centralized exports for all utils
│       │   ├── formatters.ts                       # Format functions (currency, date, enum, etc.)
│       │   ├── helper.ts                           # Helper functions (getInitials, getActivityIcon, etc.)
│       │   ├── mapper.ts                           # Enum options for forms
│       │   └── devAuth.ts                          # Development authentication utilities
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
- `Subscription.cs` - GraphQL subscriptions for real-time updates
- `TypeExtensions.cs` - GraphQL type extensions for nested resolvers
- `ActivityEventDto.cs` - DTO for activity events in subscriptions

**REST Controllers** (`gateway/.../Controllers/`):
- `ProfileImageController.cs` - Proxy for profile image operations
- These REST controllers exist because GraphQL doesn't natively support binary file operations
- Frontend calls these endpoints directly for file operations

**DataLoaders** (`gateway/.../DataLoaders/`):
- Batch and cache individual entity fetches to prevent N+1 queries
- Example: `PersonDataLoader`, `EmploymentDataLoader`, `SchoolDataLoader`
- Each DataLoader checks Redis cache first, then batches API calls

**Caching** (`gateway/.../Caching/`):
- `RedisCacheService.cs` - Contains `IRedisCacheService` interface and implementation using StackExchange.Redis
- `CacheKeys.cs` - Centralized cache key generation with hash-based approach
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

**Cache Invalidation (Event-Driven):**
- RabbitMQ consumer listens for domain events from the Backend
- Events are published in CloudEvents format: `com.ems.{entity}.{operation}`
- Gateway automatically invalidates cache when events arrive
- No need for explicit cache invalidation in mutations - it's handled by the event consumer
- Use `RemoveByPrefixAsync()` for list caches
- Use `RemoveAsync()` for individual entity caches

**RabbitMQ Event Consumer** (`gateway/.../Messaging/`):
- `RabbitMQEventConsumer.cs` - Listens for domain events, invalidates cache, and broadcasts to subscribers
- `RabbitMQBackgroundService.cs` - Background service lifecycle management
- Events invalidate related caches (e.g., person events → persons:list:*, employments:list:*)
- Events publish to GraphQL subscriptions for real-time updates
- Dashboard stats are always invalidated on any entity change

**Event Publishing (Decorator Pattern)** (`server/.../Infrastructure/Messaging/`):
- `ActivityPersistingEventPublisher` wraps `RabbitMQEventPublisher` using the Decorator pattern
- On `PublishAsync`: saves a `RecentActivity` record to the database, then delegates to RabbitMQ
- Activities persist across server restarts and are served via the Dashboard API
- Registration: `RabbitMQEventPublisher` as concrete type, `ActivityPersistingEventPublisher` as `IEventPublisher`

**GraphQL Subscriptions** (`gateway/.../Types/` and `gateway/.../Services/`):
- `Subscription.cs` - Contains `subscribeToActivityEvents` subscription endpoint
- `ActivityEventBuffer.cs` - In-memory circular buffer (50 events) for new subscribers
- New subscribers immediately receive buffered history + live events
- WebSocket connection at `/graphql` using graphql-ws protocol
- Automatic reconnection and keep-alive (10s ping interval)

**Event Types:**
| Event Pattern | Cache Invalidated | Subscription Broadcast |
|---------------|-------------------|------------------------|
| `com.ems.person.*` | `persons:list:*`, `employments:list:*` | Activity Feed |
| `com.ems.school.*` | `schools:list:*`, `employments:list:*` | Activity Feed |
| `com.ems.item.*` | `items:list:*` | Activity Feed |
| `com.ems.position.*` | `positions:list:*`, `employments:list:*` | Activity Feed |


### Frontend Architecture Guidelines

**Utils Organization**:
All utility functions are centralized in `application/src/utils/` with a single entry point:

```typescript
// Import from centralized index
import { formatCurrency, getActivityIcon, formatTimestamp } from '@/utils';
```

**Utils Structure**:
- `index.ts` - Centralized exports for all utilities
- `formatters.ts` - Format functions (formatCurrency, formatAddress, formatFileSize, formatTimestamp, formatEnumLabel)
- `helper.ts` - Helper functions (getDocumentTypeColor, getInitials, getActivityIcon)
- `mapper.ts` - Enum option arrays for forms (AppointmentStatusOptions, EmploymentStatusOptions, etc.)
- `devAuth.ts` - Development authentication utilities (only in dev mode)

**Benefits**:
- Single import source prevents circular dependencies
- Easier to discover available utilities via IntelliSense
- Consistent import paths across the application
- Clear separation of concerns (formatting vs helpers vs mappers)

**GraphQL Subscriptions (Frontend)**:
- `subscription-client.ts` - WebSocket client using `graphql-ws`
- `useRecentActivities` hook - Custom hook for activity feed subscription
- Automatic reconnection on connection loss
- Local buffer of last 50 events for resilience
- Connection status indicator in Dashboard

**Subscription Usage Example**:
```typescript
import { useRecentActivities } from '../hooks/useRecentActivities';

function Dashboard() {
  const { activities, isConnected, error } = useRecentActivities();

  return (
    <div>
      {isConnected && <Badge colorScheme="green">Live</Badge>}
      {activities.map(activity => (
        <ActivityItem key={activity.id} activity={activity} />
      ))}
    </div>
  );
}
```


**Dashboard Data Sources:**
- **Statistics**: Entity counts from `DashboardStatsDto`
- **Birthday Celebrants**: `BirthdayCelebrantDto[]` — persons with birthdays in the current month, queried from `Person` table
- **Recent Activities (fallback)**: `RecentActivityDto[]` — last 10 activities from `RecentActivities` database table
- **Recent Activities (live)**: WebSocket subscription data takes priority when available
- The Dashboard renders live subscription data if available, otherwise falls back to persisted activities from the API

**RecentActivity Entity:**
- Standalone entity (no BaseEntity inheritance, no soft deletes)
- Persisted via `ActivityPersistingEventPublisher` decorator before RabbitMQ publish
- Fields: `Id` (auto-increment), `EntityType`, `EntityId`, `Operation`, `Message`, `Timestamp`, `UserId`
- Repository: `IRecentActivityRepository` with `AddAsync` and `GetLatestAsync(count)` methods


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
- [TanStack Query Documentation](https://tanstack.com/query/latest)
- [graphql-request Documentation](https://github.com/graffle-js/graffle)
- [GraphQL Code Generator Documentation](https://the-guild.dev/graphql/codegen)
- [HotChocolate GraphQL Documentation](https://chillicream.com/docs/hotchocolate)
- [Clean Architecture Principles](https://8thlight.com/blog/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Entity Framework Core Documentation](https://learn.microsoft.com/en-us/ef/core/)
- [Azure Blob Storage Documentation](https://learn.microsoft.com/en-us/azure/storage/blobs/)
- [xUnit Documentation](https://xunit.net/docs/getting-started/netcore/cmdline)
- [AG Grid Documentation](https://www.ag-grid.com/react-data-grid/getting-started/)
- [NSwag Documentation](https://github.com/RicoSuter/NSwag)
- [StackExchange.Redis Documentation](https://stackexchange.github.io/StackExchange.Redis/)
- [RabbitMQ .NET Client Documentation](https://www.rabbitmq.com/dotnet.html)
- [CloudEvents Specification](https://cloudevents.io/)
- [Serilog Documentation](https://serilog.net/)
- [Seq Documentation](https://datalust.co/seq)
