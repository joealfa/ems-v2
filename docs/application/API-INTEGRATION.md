# API Integration

This document describes how the frontend application integrates with the backend API through the GraphQL Gateway.

---

## Overview

The EMS frontend uses **TanStack Query** with **graphql-request** and **GraphQL Code Generator** to communicate with the backend via a GraphQL Gateway (HotChocolate). This ensures type safety and provides a great developer experience with auto-generated types and custom hooks.

---

## Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    Frontend Application                     │
├─────────────────────────────────────────────────────────────┤
│  Components/Pages                                           │
│       │                                                     │
│       ▼                                                     │
│  ┌─────────────────┐                                        │
│  │  Custom Hooks   │  ← src/hooks/                          │
│  │  (usePersons,   │                                        │
│  │   useSchools)   │                                        │
│  └────────┬────────┘                                        │
│           │                                                 │
│           ▼                                                 │
│  ┌─────────────────┐    ┌─────────────────┐                 │
│  │  TanStack Query │    │  Generated Types│                 │
│  │  + graphql-req  │◄───│  & Documents    │                 │
│  └────────┬────────┘    └─────────────────┘                 │
│           │                    src/graphql/generated/       │
│           ▼                                                 │
│  ┌─────────────────┐                                        │
│  │  GraphQL        │                                        │
│  │  Operations     │  ← src/graphql/operations/*.graphql    │
│  └─────────────────┘                                        │
└─────────────────────────────────────────────────────────────┘
                          │
                          ▼
┌─────────────────────────────────────────────────────────────┐
│                    GraphQL Gateway                          │
│                 https://localhost:5003/graphql              │
│                    (HotChocolate)                           │
└─────────────────────────────────────────────────────────────┘
                          │
                          ▼
┌─────────────────────────────────────────────────────────────┐
│                    Backend API                              │
│                 https://localhost:5001                      │
└─────────────────────────────────────────────────────────────┘
```

---

## Authentication Security

The application uses a dual-token authentication system:

### Access Tokens
- **Storage**: `localStorage`
- **Lifetime**: Short-lived (15 minutes)
- **Purpose**: API authorization via `Authorization: Bearer <token>` header
- **Risk**: Exposed to XSS, but limited damage due to short lifetime

### Refresh Tokens
- **Storage**: HttpOnly cookies
- **Lifetime**: Long-lived (7 days)
- **Purpose**: Obtain new access tokens without re-authentication
- **Security Features**:
  - `HttpOnly`: JavaScript cannot access (XSS protection)
  - `Secure`: HTTPS only
  - `SameSite=Strict`: CSRF protection
  - Automatically sent by browser with requests

### Why This Approach?

1. **Access tokens in localStorage**: Required for making API calls with custom headers
2. **Refresh tokens in cookies**: Maximum security for long-lived credentials
3. **Automatic refresh**: Seamless token renewal via AuthContext
4. **No token exposure**: Refresh tokens never touch JavaScript code

---

## Configuration

### GraphQL Client Configuration

**File:** `src/graphql/graphql-client.ts`

The GraphQL client is configured with:
- `graphql-request` `GraphQLClient` pointing to the GraphQL Gateway
- Automatic Bearer token injection from `localStorage`
- Credentials included for cookie-based refresh tokens
- `GraphQL-Preflight` header for HotChocolate CSRF protection

### TanStack QueryClient Configuration

**File:** `src/graphql/query-client.ts`

The QueryClient is configured with:
- Global error handling via `QueryCache` and `MutationCache` `onError` callbacks
- Default `staleTime: 2 minutes`, `gcTime: 5 minutes`
- `refetchOnWindowFocus: true` for automatic background refetching
- `retry: 1` for queries, `retry: false` for mutations

### GraphQL Code Generator

**File:** `codegen.ts`

```typescript
import type { CodegenConfig } from '@graphql-codegen/cli';

const config: CodegenConfig = {
  schema: 'https://localhost:5003/graphql',
  documents: ['src/**/*.graphql', 'src/**/operations.ts'],
  generates: {
    './src/graphql/generated/': {
      preset: 'client',
      config: {
        enumsAsTypes: true,
        skipTypename: false,
        useTypeImports: true,
      }
    }
  }
};

export default config;
```

---

## GraphQL Operations

### Query Files

GraphQL operations are defined in `.graphql` files in `src/graphql/operations/`:

| File | Description |
|------|-------------|
| `auth.graphql` | Authentication mutations |
| `dashboard.graphql` | Dashboard statistics query |
| `documents.graphql` | Document queries and mutations |
| `employments.graphql` | Employment queries and mutations |
| `items.graphql` | Item queries and mutations |
| `persons.graphql` | Person queries and mutations |
| `positions.graphql` | Position queries and mutations |
| `salary-grades.graphql` | Salary grade queries and mutations |
| `schools.graphql` | School queries and mutations |

### Example Operations

**Query:**
```graphql
query GetPersons($pageNumber: Int, $pageSize: Int, $searchTerm: String) {
  persons(pageNumber: $pageNumber, pageSize: $pageSize, searchTerm: $searchTerm) {
    items {
      displayId
      fullName
      gender
      civilStatus
    }
    totalCount
    pageNumber
    pageSize
  }
}
```

**Mutation:**
```graphql
mutation CreatePerson($input: CreatePersonInput!) {
  createPerson(input: $input) {
    displayId
    firstName
    lastName
    fullName
  }
}
```

---

## Custom Hooks

### Available Hooks

| Hook | Purpose |
|------|---------|
| `usePersons()` | Get paginated list of persons |
| `usePerson()` | Get single person by ID |
| `useCreatePerson()` | Create a new person |
| `useUpdatePerson()` | Update an existing person |
| `useDeletePerson()` | Delete a person |
| `useEmployments()` | Get paginated list of employments |
| `useSchools()` | Get paginated list of schools |
| `usePositions()` | Get paginated list of positions |
| `useSalaryGrades()` | Get paginated list of salary grades |
| `useItems()` | Get paginated list of items |
| `useDashboard()` | Get dashboard statistics |
| `usePersonDocuments()` | Get paginated list of documents for a person |
| `useDocument()` | Get single document by display ID |
| `useUpdateDocument()` | Update document metadata |
| `useDeleteDocument()` | Delete a document |
| `useDeleteProfileImage()` | Delete a person's profile image |

### Usage Examples

#### Get List (Paginated)

```typescript
import { usePersons } from '../hooks';

const PersonsPage = () => {
  const { persons, loading, error, totalCount, refetch } = usePersons({
    pageNumber: 1,
    pageSize: 10,
    searchTerm: 'John'
  });

  if (loading) return <Spinner />;
  if (error) return <Alert>{error.message}</Alert>;

  return <PersonsTable data={persons} total={totalCount} />;
};
```

#### Get Single Record

```typescript
import { usePerson } from '../hooks';

const PersonDetail = ({ displayId }: { displayId: number }) => {
  const { person, loading, error } = usePerson(displayId);

  if (loading) return <Spinner />;
  if (error) return <Alert>{error.message}</Alert>;
  if (!person) return <NotFound />;

  return <PersonCard person={person} />;
};
```

#### Create Record

```typescript
import { useCreatePerson } from '../hooks';

const PersonForm = () => {
  const { createPerson, loading, error } = useCreatePerson();

  const handleSubmit = async (data: CreatePersonInput) => {
    const result = await createPerson(data);
    if (result) {
      navigate(`/persons/${result.displayId}`);
    }
  };

  return <form onSubmit={handleSubmit}>...</form>;
};
```

#### Update Record

```typescript
import { useUpdatePerson } from '../hooks';

const EditPersonForm = ({ displayId }: { displayId: number }) => {
  const { updatePerson, loading, error } = useUpdatePerson();

  const handleSubmit = async (data: UpdatePersonInput) => {
    const result = await updatePerson(displayId, data);
    if (result) {
      navigate(`/persons/${displayId}`);
    }
  };

  return <form onSubmit={handleSubmit}>...</form>;
};
```

#### Delete Record

```typescript
import { useDeletePerson } from '../hooks';

const DeleteButton = ({ displayId }: { displayId: number }) => {
  const { deletePerson, loading } = useDeletePerson();

  const handleDelete = async () => {
    const success = await deletePerson(displayId);
    if (success) {
      navigate('/persons');
    }
  };

  return <Button onClick={handleDelete} loading={loading}>Delete</Button>;
};
```

---

## Authentication Integration

### AuthContext

The authentication context manages user authentication state and token lifecycle.

**File:** `src/contexts/AuthContext.tsx`

**Key Features:**
- Google OAuth2 authentication
- JWT access token management
- Refresh token rotation
- Automatic token refresh on API calls
- User session persistence

**Context Interface:**
```typescript
interface AuthContextType {
  user: UserDto | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  login: (googleIdToken: string) => Promise<void>;
  logout: () => Promise<void>;
}
```

### Authentication Flow

1. User clicks Google Sign-In button on Login page
2. Google returns an ID token via `@react-oauth/google`
3. Frontend sends ID token to GraphQL Gateway via `googleLogin` mutation
4. Gateway forwards to backend which validates token with Google
5. Backend returns JWT access token, refresh token, and user info
6. Frontend stores access token in localStorage and user in context
7. Subsequent GraphQL requests include Bearer token in Authorization header
8. When access token expires, AuthContext automatically refreshes using refresh token

---

## Error Handling

### Global Error Handling

Errors are handled at the TanStack QueryClient level via `QueryCache` and `MutationCache` `onError` callbacks that:
- Log errors for debugging
- Detect auth errors (401/403) and redirect to login
- Clear stored tokens on authentication failure

### Component-Level Handling

```typescript
const { persons, loading, error } = usePersons();

if (loading) return <Spinner />;
if (error) return <Alert>{error.message}</Alert>;
```

---

## Gateway REST Endpoints (File Operations)

The Gateway provides REST endpoints for file operations because GraphQL doesn't natively support multipart file uploads. These endpoints proxy requests to the Backend API with proper authentication.

### Base URL
`https://localhost:5003/api/persons/{personDisplayId}/documents`

### Available Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/api/persons/{personDisplayId}/documents` | Upload a document |
| `GET` | `/api/persons/{personDisplayId}/documents/{documentDisplayId}/download` | Download a document |
| `DELETE` | `/api/persons/{personDisplayId}/documents/{documentDisplayId}` | Delete a document |
| `POST` | `/api/persons/{personDisplayId}/documents/profile-image` | Upload profile image |
| `GET` | `/api/persons/{personDisplayId}/documents/profile-image` | Get profile image |
| `DELETE` | `/api/persons/{personDisplayId}/documents/profile-image` | Delete profile image |

### Usage Example

```typescript
import {
  uploadDocument,
  uploadProfileImage,
  getDocumentDownloadUrl,
  getProfileImageUrl,
} from '../hooks/useDocuments';

// Upload document
const accessToken = localStorage.getItem('accessToken');
await uploadDocument(personDisplayId, file, 'Description', accessToken!);

// Upload profile image
await uploadProfileImage(personDisplayId, imageFile, accessToken!);

// Get download URL
const downloadUrl = getDocumentDownloadUrl(personDisplayId, documentDisplayId);

// Get profile image URL (with cache busting version)
const imageUrl = getProfileImageUrl(personDisplayId, Date.now());
```

---

## Regenerating Types

When the GraphQL schema changes on the gateway:

1. **Ensure gateway is running** on `https://localhost:5003`
2. **Run codegen command:**
   ```bash
   npm run codegen
   ```
3. **Check for breaking changes** in generated code
4. **Update components** if needed
5. **Test affected functionality**

### Post-Generation Checklist

- [ ] New types generated correctly
- [ ] Existing imports still work
- [ ] New queries/mutations accessible
- [ ] No TypeScript compilation errors
