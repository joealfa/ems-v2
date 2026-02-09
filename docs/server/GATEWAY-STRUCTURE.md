# Gateway Project Structure

## Overview
The Gateway GraphQL project has been reorganized for better maintainability and clarity. All REST controllers have been migrated to GraphQL operations.

## Folder Structure

```
gateway/EmployeeManagementSystem.Gateway/
├── Program.cs                          # Application entry point (with WebSocket support)
├── appsettings.json                    # Configuration settings
├── appsettings.Development.json        # Development configuration
├── Caching/                            # Redis caching implementation
│   ├── RedisCacheService.cs            # IRedisCacheService interface + implementation (with logging)
│   └── CacheKeys.cs                    # Centralized cache key generation
├── Controllers/                        # REST proxy controllers
│   ├── ProfileImageController.cs       # Profile image proxy (with logging)
│   └── DevAuthController.cs            # Development authentication (dev only)
├── DataLoaders/                        # HotChocolate DataLoaders for batching
│   ├── PersonDataLoader.cs             # Person entity data loader (with logging)
│   ├── EmploymentDataLoader.cs         # Employment entity data loader (with logging)
│   ├── SchoolDataLoader.cs             # School entity data loader (with logging)
│   ├── PositionDataLoader.cs           # Position entity data loader (with logging)
│   ├── SalaryGradeDataLoader.cs        # Salary grade data loader (with logging)
│   └── ItemDataLoader.cs               # Item entity data loader (with logging)
├── Errors/                             # Error handling
│   └── ApiExceptionErrorFilter.cs      # GraphQL error filter (with logging)
├── Extensions/                         # Service registration extensions
│   └── ServiceCollectionExtensions.cs  # DI configuration
├── Mappings/                           # GraphQL input to DTO mappings
│   └── InputMappingExtensions.cs       # Extension methods for mapping inputs
├── Messaging/                          # RabbitMQ event consumer
│   ├── RabbitMQEventConsumer.cs        # Event consumer (cache + subscriptions)
│   ├── RabbitMQBackgroundService.cs    # Background service lifecycle
│   ├── RabbitMQSettings.cs             # RabbitMQ configuration
│   └── CloudEvent.cs                   # CloudEvents message model
├── Services/                           # Application services
│   └── ActivityEventBuffer.cs          # Circular buffer for recent activity events
├── Types/                              # GraphQL type definitions
│   ├── Query.cs                        # All GraphQL queries
│   ├── Mutation.cs                     # All GraphQL mutations (with logging)
│   ├── Subscription.cs                 # GraphQL subscriptions (real-time updates)
│   ├── ActivityEventDto.cs             # Activity event DTO for subscriptions
│   ├── Configuration/                  # GraphQL configuration & scalars
│   │   ├── PascalCaseNamingConventions.cs  # Custom naming conventions for enums
│   │   └── LongType.cs                 # Custom Long scalar type
│   ├── Extensions/                     # GraphQL type extensions
│   │   └── TypeExtensions.cs           # Type extensions for nested resolvers
│   └── Inputs/                         # GraphQL input types
│       ├── PersonInputs.cs             # Person create/update inputs
│       ├── EmploymentInputs.cs         # Employment create/update inputs
│       ├── SchoolInputs.cs             # School create/update inputs
│       ├── PositionInputs.cs           # Position create/update inputs
│       ├── SalaryGradeInputs.cs        # Salary grade create/update inputs
│       ├── ItemInputs.cs               # Item create/update inputs
│       ├── AddressInputs.cs            # Address create/update inputs
│       ├── ContactInputs.cs            # Contact create/update inputs
│       └── DocumentInputs.cs           # Document upload/update inputs
└── Properties/                         # Project metadata
    └── launchSettings.json             # Launch configuration
```

## Changes Made

### ✅ Migrated to GraphQL
- **Removed**: REST controllers (`DocumentsController.cs`, `AuthController.cs`)
- **Added**: GraphQL mutations and queries for document operations
- **Added**: GraphQL mutations for authentication (already existed)

### ✅ Document Operations (Now GraphQL)

**Mutations** (`Mutation.cs`):
- `uploadDocument` - Upload a document for a person
- `updateDocument` - Update document metadata
- `deleteDocument` - Delete a document
- `uploadProfileImage` - Upload a profile image
- `deleteProfileImage` - Delete a profile image

**Queries** (`Query.cs`):
- `getPersonDocuments` - Get paginated list of documents
- `getDocument` - Get a single document by ID
- `getProfileImageUrl` - Get profile image URL

### ✅ GraphQL Subscriptions (Real-time Updates)

**Subscriptions** (`Subscription.cs`):
- `subscribeToActivityEvents` - Subscribe to real-time activity events via WebSocket

**Supporting Components**:
- `ActivityEventDto.cs` - DTO for activity events with message, metadata, and timestamp
- `ActivityEventBuffer.cs` - Circular buffer (50 events) for new subscribers to receive recent history
- WebSocket support enabled in Program.cs
- Uses HotChocolate.Subscriptions and graphql-ws protocol

**Subscription Example**:

```graphql
subscription OnActivityEvent {
  subscribeToActivityEvents {
    id
    eventType
    entityType
    entityId
    operation
    timestamp
    userId
    message
    metadata {
      key
      value
    }
  }
}
```

### ✅ Authentication (GraphQL)

**Mutations** (`Mutation.cs`):
- `googleLogin` - Login with Google ID token
- `googleTokenLogin` - Login with Google access token
- `refreshToken` - Refresh authentication token
- `logout` - Logout and revoke tokens

### ✅ Cleaned Up Folders
- **Removed**: Empty `Authentication/` folder
- **Removed**: Empty `Scalars/` folder
- **Moved**: `LongType.cs` to `Types/Configuration/` directory

## GraphQL File Upload

File uploads in GraphQL use HotChocolate's built-in `IFile` scalar type. Example:

```graphql
mutation UploadDocument($personId: Long!, $file: Upload!, $description: String) {
  uploadDocument(personDisplayId: $personId, file: $file, description: $description) {
    displayId
    fileName
    contentType
    description
  }
}
```

## Frontend Integration

The frontend should now use GraphQL Code Generator to create typed operations:

```bash
# In application directory
npm run codegen
```

This will generate hooks like:
- `useUploadDocumentMutation()`
- `useUploadProfileImageMutation()`
- `useGetPersonDocumentsQuery()`

## Logging and Monitoring

### Serilog Integration

All Gateway components include structured logging with Serilog and Seq.

**Log Destinations**:
- **Console**: Colored output for local development
- **Seq**: Centralized log aggregation at `http://localhost:5341`

See [Logging Documentation](./LOGGING.md) for detailed information.

---

## RabbitMQ Event Consumer

The Gateway includes a RabbitMQ consumer for event-driven cache invalidation.

### Components

- **RabbitMQEventConsumer**: Listens on `ems.gateway.cache-invalidation` queue
- **RabbitMQBackgroundService**: Manages consumer lifecycle
- **Cache Invalidation**: Automatically invalidates Redis cache based on event type

### Event Types Handled

| Event Pattern | Cache Invalidated |
|---------------|-------------------|
| `com.ems.person.*` | `persons:list:*`, `employments:list:*` |
| `com.ems.school.*` | `schools:list:*`, `employments:list:*` |
| `com.ems.item.*` | `items:list:*` |
| `com.ems.position.*` | `positions:list:*`, `employments:list:*` |
| `com.ems.salarygrade.*` | `salarygrades:list:*`, `employments:list:*` |
| `com.ems.employee.*` | `employments:list:*` |
| `com.ems.blob.*` | Depends on `relatedEntityType` |

All events also invalidate the dashboard stats cache.

### Configuration

```json
{
  "RabbitMQ": {
    "HostName": "localhost",
    "Port": 5672,
    "VirtualHost": "ems",
    "UserName": "emsadmin",
    "Password": "${RABBITMQ_PASSWORD}",
    "ExchangeName": "ems.events",
    "QueueName": "ems.gateway.cache-invalidation",
    "Enabled": true
  }
}
```

See [RabbitMQ Events Documentation](../TESTING_RABBITMQ_EVENTS.md) for complete details.

---

## Benefits

1. **Consistency**: All API operations now use GraphQL
2. **Type Safety**: Auto-generated types from GraphQL schema
3. **Better Performance**: DataLoaders prevent N+1 queries
4. **Real-time Updates**: WebSocket subscriptions for activity feed
5. **Event-driven Architecture**: RabbitMQ for cache invalidation and broadcasting
6. **Cleaner Architecture**: No mixing of REST and GraphQL (except file operations)
7. **Caching**: Redis caching at the GraphQL layer
8. **Observability**: Comprehensive structured logging with Serilog + Seq
