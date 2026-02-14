# GraphQL Usage Guide

This application uses **TanStack Query** with **graphql-request** and **GraphQL Code Generator** for type-safe API communication.

## Quick Start

### 1. Generate Types from Schema

Whenever the GraphQL schema changes on the gateway, run:

```bash
npm run codegen
```

This will:
- Download the schema from `https://localhost:5003/graphql`
- Generate TypeScript types in `src/graphql/generated/`
- Create typed queries and mutations

### 2. Using Custom Hooks (Recommended)

We've created custom hooks that wrap TanStack Query for easier usage:

#### Fetching Data

```tsx
import { usePersons, usePerson } from '../hooks';

function PersonsPage() {
  const { persons, loading, error, totalCount, refetch } = usePersons({
    pageNumber: 1,
    pageSize: 10,
    searchTerm: 'John'
  });

  if (loading) return <Spinner />;
  if (error) return <Error message={error.message} />;

  return (
    <div>
      <PersonsTable data={persons} />
      <Button onClick={() => refetch()}>Refresh</Button>
    </div>
  );
}
```

#### Single Item

```tsx
function PersonDetail({ displayId }: { displayId: number }) {
  const { person, loading, error } = usePerson(displayId);

  if (loading) return <Spinner />;
  if (error) return <Error />;

  return <PersonCard person={person} />;
}
```

#### Mutations

```tsx
import { useCreatePerson, useUpdatePerson, useDeletePerson } from '../hooks';

function PersonForm() {
  const { createPerson, loading } = useCreatePerson();

  const handleSubmit = async (formData) => {
    const result = await createPerson({
      firstName: formData.firstName,
      lastName: formData.lastName,
      // ... other fields
    });

    if (result) {
      navigate(`/persons/${result.displayId}`);
    }
  };

  return <form onSubmit={handleSubmit}>...</form>;
}
```

### 3. Using graphqlRequest Directly (Advanced)

You can also use the `graphqlRequest` function directly:

```tsx
import { graphqlRequest } from '../graphql/graphql-client';
import { GetPersonsDocument, type GetPersonsQuery, type GetPersonsQueryVariables } from '../graphql/generated/graphql';

async function fetchPersons() {
  const data = await graphqlRequest<GetPersonsQuery, GetPersonsQueryVariables>(
    GetPersonsDocument,
    { pageNumber: 1, pageSize: 10 }
  );
  return data.persons;
}
```

## Available Custom Hooks

### Persons
- `usePersons(variables?)` - Get paginated list of persons
- `usePerson(displayId)` - Get single person by display ID
- `useCreatePerson()` - Create a new person
- `useUpdatePerson()` - Update an existing person
- `useDeletePerson()` - Delete a person

### Employments
- `useEmployments(variables?)` - Get paginated list of employments
- `useEmployment(displayId)` - Get single employment by display ID
- `useCreateEmployment()` - Create a new employment
- `useUpdateEmployment()` - Update an existing employment
- `useDeleteEmployment()` - Delete an employment

### Documents
- `usePersonDocuments(personDisplayId, options?)` - Get paginated list of documents for a person
- `useDocument(personDisplayId, documentDisplayId)` - Get single document by display ID
- `useUpdateDocument()` - Update document metadata
- `useDeleteDocument()` - Delete a document
- `useDeleteProfileImage()` - Delete a person's profile image

**Document Helper Functions** (non-hook utilities):
- `getDocumentDownloadUrl(personDisplayId, documentDisplayId)` - Get download URL for a document
- `getProfileImageUrl(personDisplayId, version?)` - Get URL for a person's profile image
- `uploadDocument(personDisplayId, file, description, accessToken)` - Upload a document via Gateway REST endpoint
- `uploadProfileImage(personDisplayId, file, accessToken)` - Upload a profile image via Gateway REST endpoint

### Authentication
- `useGoogleLogin()` - Login with Google ID token
- `useGoogleTokenLogin()` - Login with Google access token
- `useRefreshToken()` - Refresh auth token
- `useLogout()` - Logout user
- `useCurrentUser()` - Get current user info

### Subscriptions (Real-time Updates)
- `useRecentActivities()` - Subscribe to real-time activity feed via WebSocket
  - Returns `{ activities, isConnected, error }`
  - Maintains local buffer of last 50 events
  - Automatic reconnection on connection loss
  - Connection status indicator

## TanStack Query Features

### WebSocket Subscriptions

The application uses GraphQL subscriptions over WebSocket for real-time activity updates.

#### Using the Activity Feed

```tsx
import { useRecentActivities } from '../hooks/useRecentActivities';
import { formatTimestamp, getActivityIcon } from '../utils';

function Dashboard() {
  const { activities, isConnected, error } = useRecentActivities();

  return (
    <div>
      {/* Connection indicator */}
      {isConnected && (
        <Badge colorScheme="green">Live</Badge>
      )}

      {/* Error handling */}
      {error && (
        <Alert status="error">
          Connection error: {error.message}
        </Alert>
      )}

      {/* Activity list */}
      {activities.map(activity => (
        <div key={activity.id}>
          <Text>
            {getActivityIcon(activity.entityType)} {activity.message}
          </Text>
          <Text fontSize="xs" color="gray.500">
            {formatTimestamp(activity.timestamp)}
          </Text>
        </div>
      ))}
    </div>
  );
}
```

#### Subscription Features

- **Real-time Updates**: Receives events instantly via WebSocket
- **Buffered History**: New subscribers get last 50 events immediately
- **Auto-reconnection**: Reconnects automatically if connection is lost
- **Connection Status**: Track connection state with `isConnected`
- **Error Handling**: Provides error details via `error` property
- **Local Buffer**: Maintains local copy of last 50 events for resilience

#### Activity Event Structure

```typescript
interface ActivityEventDto {
  entityType: string;        // e.g., "person", "employment"
  entityId: string;          // Display ID of the entity
  operation: string;         // "CREATE", "UPDATE", "DELETE"
  message: string;           // Human-readable description
  timestamp: string;         // ISO 8601 timestamp
  userId?: string | null;    // User who triggered the event
}
```

---

## Project Structure

### GraphQL Files Organization

```
src/
├── graphql/
│   ├── graphql-client.ts    # graphql-request client configuration
│   ├── subscription-client.ts # graphql-ws WebSocket client
│   ├── query-client.ts        # TanStack QueryClient configuration
│   ├── QueryProvider.tsx      # TanStack Query provider wrapper
│   ├── query-keys.ts         # Query key factory for cache management
│   ├── error-handler.ts      # Global error handling utilities
│   ├── types.ts              # Shared pagination types
│   ├── index.ts              # Barrel exports
│   ├── operations/            # GraphQL queries/mutations/subscriptions (.graphql files)
│   │   ├── auth.graphql       # Authentication mutations
│   │   ├── dashboard.graphql  # Dashboard statistics query
│   │   ├── documents.graphql  # Document queries and mutations
│   │   ├── employments.graphql
│   │   ├── items.graphql
│   │   ├── persons.graphql
│   │   ├── positions.graphql
│   │   ├── salary-grades.graphql
│   │   ├── schools.graphql
│   │   └── subscriptions.graphql  # Real-time subscription operations
│   └── generated/             # Auto-generated types (DO NOT EDIT)
│       ├── gql.ts             # Document exports
│       └── graphql.ts         # Types and operations
└── hooks/
    ├── index.ts               # Barrel export file
    ├── useAuth.ts             # Authentication hook
    ├── useAuthMutations.ts    # Auth mutations (login, logout)
    ├── useDashboard.ts        # Dashboard statistics hook
    ├── useDebounce.ts         # Debounce utility hook
    ├── useDocuments.ts        # Document operations and helpers
    ├── useEmployments.ts      # Employment CRUD hooks
    ├── useItems.ts            # Item CRUD hooks
    ├── usePersons.ts          # Person CRUD hooks
    ├── usePositions.ts        # Position CRUD hooks
    ├── useRecentActivities.ts # Real-time activity subscription hook
    ├── useSalaryGrades.ts     # Salary grade CRUD hooks
    ├── useSchools.ts          # School CRUD hooks
    └── useToast.ts            # Toast notification hook
```

---
### Automatic Caching
TanStack Query caches query results using query keys. Data is considered fresh for a configurable `staleTime`, then automatically refetched in the background:

```tsx
// Default staleTime is 2 minutes (configured in query-client.ts)
const { persons, loading, error } = usePersons({ pageNumber: 1, pageSize: 10 });
```

### Cache Invalidation
Mutations automatically invalidate related queries via `onSuccess` callbacks:

```tsx
// After creating a person, the persons list and dashboard stats are invalidated
const { createPerson, loading } = useCreatePerson();
// onSuccess internally calls:
//   queryClient.invalidateQueries({ queryKey: personKeys.lists() });
//   queryClient.invalidateQueries({ queryKey: dashboardKeys.stats() });
```

### Query Key Factory
All cache keys are managed by a centralized query key factory in `src/graphql/query-keys.ts`:

```tsx
// Hierarchical key structure for fine-grained invalidation
personKeys.all           // ['persons'] - invalidate everything
personKeys.lists()       // ['persons', 'list'] - invalidate all lists
personKeys.list(filters) // ['persons', 'list', {...}] - specific list
personKeys.details()     // ['persons', 'detail'] - all details
personKeys.detail(id)    // ['persons', 'detail', 123] - specific detail
```

### Loading and Error States
All hooks return `loading` and `error` states:

```tsx
const { persons, loading, error } = usePersons();

if (loading) return <Spinner />;
if (error) return <Alert>{error.message}</Alert>;
```

## When to Run Codegen

Run `npm run codegen` when:
- Gateway schema changes (new queries/mutations/types)
- GraphQL operations files (`.graphql`) are added/modified
- You see TypeScript errors in generated types

## File Structure

```
src/
├── graphql/
│   ├── graphql-client.ts      # graphql-request client setup
│   ├── query-client.ts        # TanStack QueryClient configuration
│   ├── QueryProvider.tsx      # TanStack Query provider wrapper
│   ├── query-keys.ts         # Query key factory for cache management
│   ├── error-handler.ts      # Global error handling utilities
│   ├── types.ts              # Shared pagination types
│   ├── index.ts              # Barrel exports
│   ├── operations/            # GraphQL queries/mutations (.graphql files)
│   │   ├── auth.graphql       # Authentication mutations
│   │   ├── dashboard.graphql  # Dashboard statistics query
│   │   ├── documents.graphql  # Document queries and mutations
│   │   ├── employments.graphql
│   │   ├── items.graphql
│   │   ├── persons.graphql
│   │   ├── positions.graphql
│   │   ├── salary-grades.graphql
│   │   └── schools.graphql
│   └── generated/             # Auto-generated types (DO NOT EDIT)
│       ├── gql.ts             # Document exports
│       └── graphql.ts         # Types and operations
└── hooks/
    ├── index.ts               # Barrel export file
    ├── useAuth.ts             # Authentication hook
    ├── useAuthMutations.ts    # Auth mutations (login, logout)
    ├── useDashboard.ts        # Dashboard statistics hook
    ├── useDebounce.ts         # Debounce utility hook
    ├── useDocuments.ts        # Document operations and helpers
    ├── useEmployments.ts      # Employment CRUD hooks
    ├── useItems.ts            # Item CRUD hooks
    ├── usePersons.ts          # Person CRUD hooks
    ├── usePositions.ts        # Position CRUD hooks
    ├── useSalaryGrades.ts     # Salary grade CRUD hooks
    └── useSchools.ts          # School CRUD hooks
```

## Best Practices

1. **Always use custom hooks** - They provide better error handling and TypeScript support
2. **Don't edit generated files** - They're overwritten by codegen
6. **Use subscriptions for real-time updates** - Leverage WebSocket for activity feeds
7. **Centralize utilities** - Import utils from `@/utils` for consistency
3. **Use query key factory** - Keep cache keys consistent via `query-keys.ts`
4. **Invalidate on mutation** - Use `onSuccess` to invalidate related query keys
5. **Handle loading/error states** - Always show feedback to users

## Troubleshooting

### Types are out of sync
```bash
npm run codegen
```

### Query not updating after mutation
Ensure the mutation's `onSuccess` invalidates the correct query keys:
```tsx
onSuccess: () => {
  queryClient.invalidateQueries({ queryKey: personKeys.lists() });
}
```

### Network errors
Check that: and `VITE_GRAPHQL_WS_URL`
3. Authentication token is valid

### Subscription not connecting
Check that:
1. WebSocket URL is correct in `.env`: `VITE_GRAPHQL_WS_URL=wss://localhost:5003/graphql`
2. Gateway has WebSocket support enabled (check Program.cs for `app.UseWebSockets()`)
3. Browser console for WebSocket connection errors//localhost:5003`
2. `.env` has correct `VITE_GRAPHQL_URL`
3. Authentication token is valid

## Document Operations (REST via Gateway)

Document uploads and downloads use REST endpoints on the Gateway (not GraphQL) because GraphQL doesn't natively support multipart file uploads.

### Upload Document

```tsx
import { uploadDocument } from '../hooks/useDocuments';

const handleUpload = async (file: File) => {
  const accessToken = localStorage.getItem('accessToken');
  const response = await uploadDocument(
    personDisplayId,
    file,
    'Document description',
    accessToken!
  );
  if (response.ok) {
    // Refresh document list
    refetch();
  }
};
```

### Upload Profile Image

```tsx
import { uploadProfileImage } from '../hooks/useDocuments';

const handleUploadImage = async (file: File) => {
  const accessToken = localStorage.getItem('accessToken');
  const response = await uploadProfileImage(personDisplayId, file, accessToken!);
  if (response.ok) {
    // Image uploaded successfully
  }
};
```

### Download Document

```tsx
import { getDocumentDownloadUrl } from '../hooks/useDocuments';

const downloadUrl = getDocumentDownloadUrl(personDisplayId, documentDisplayId);
// Use this URL in an anchor tag or window.open()
```

### Display Profile Image

```tsx
import { getProfileImageUrl } from '../hooks/useDocuments';

const imageUrl = getProfileImageUrl(personDisplayId, version);
// Use in <img src={imageUrl} /> - version parameter busts cache after updates
```
