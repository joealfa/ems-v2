# Gateway Project Structure

## Overview
The Gateway GraphQL project has been reorganized for better maintainability and clarity. All REST controllers have been migrated to GraphQL operations.

## Folder Structure

```
gateway/EmployeeManagementSystem.Gateway/
├── Program.cs                          # Application entry point
├── appsettings.json                    # Configuration settings
├── appsettings.Development.json        # Development configuration
├── Caching/                            # Redis caching implementation
│   ├── IRedisCacheService.cs           # Cache service interface
│   ├── RedisCacheService.cs            # Redis cache implementation
│   ├── NoOpCacheService.cs             # No-op cache for debugging
│   └── CacheKeys.cs                    # Centralized cache key generation
├── DataLoaders/                        # HotChocolate DataLoaders for batching
│   ├── PersonDataLoader.cs             # Person entity data loader
│   ├── EmploymentDataLoader.cs         # Employment entity data loader
│   ├── SchoolDataLoader.cs             # School entity data loader
│   ├── PositionDataLoader.cs           # Position entity data loader
│   ├── SalaryGradeDataLoader.cs        # Salary grade data loader
│   └── ItemDataLoader.cs               # Item entity data loader
├── Extensions/                         # Service registration extensions
│   └── ServiceCollectionExtensions.cs  # DI configuration
├── Mappings/                           # GraphQL input to DTO mappings
│   └── InputMappingExtensions.cs       # Extension methods for mapping inputs
├── Types/                              # GraphQL type definitions
│   ├── Query.cs                        # All GraphQL queries
│   ├── Mutation.cs                     # All GraphQL mutations
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

### ✅ Authentication (GraphQL)

**Mutations** (`Mutation.cs`):
- `googleLogin` - Login with Google ID token
- `googleTokenLogin` - Login with Google access token
- `refreshToken` - Refresh authentication token
- `logout` - Logout and revoke tokens

### ✅ Cleaned Up Folders
- **Removed**: Empty `Authentication/` folder
- **Removed**: Empty `Scalars/` folder
- **Removed**: `Controllers/` folder (no longer needed)
- **Moved**: `LongType.cs` to `Types/` root directory

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

## Benefits

1. **Consistency**: All API operations now use GraphQL
2. **Type Safety**: Auto-generated types from GraphQL schema
3. **Better Performance**: DataLoaders prevent N+1 queries
4. **Cleaner Architecture**: No mixing of REST and GraphQL
5. **Caching**: Redis caching at the GraphQL layer
