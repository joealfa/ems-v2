# GraphQL Usage Guide

This application uses **Apollo Client** with **GraphQL Code Generator** for type-safe API communication.

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

We've created custom hooks that wrap Apollo Client for easier usage:

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

### 3. Using Apollo Hooks Directly (Advanced)

You can also use Apollo's hooks directly:

```tsx
import { useQuery, useMutation } from '@apollo/client';
import { GetPersonsDocument, CreatePersonDocument } from '../graphql/generated/graphql';

function MyComponent() {
  const { data, loading, error } = useQuery(GetPersonsDocument, {
    variables: { pageNumber: 1, pageSize: 10 }
  });

  const [createPerson] = useMutation(CreatePersonDocument);

  // ...
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

## Apollo Client Features

### Automatic Caching
Apollo automatically caches query results. When you refetch data, it uses the cache first, then updates with fresh data:

```tsx
const { data } = useQuery(GetPersonsDocument, {
  fetchPolicy: 'cache-and-network' // Use cache, then fetch fresh data
});
```

### Refetch Queries
Mutations automatically refetch related queries:

```tsx
const [createPerson] = useMutation(CreatePersonDocument, {
  refetchQueries: [GetPersonsDocument] // Auto-refresh persons list
});
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
│   ├── client.ts              # Apollo Client setup
│   ├── ApolloProvider.tsx     # React provider
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
    ├── useDebounce.ts         # Debounce utility hook
    ├── useDocuments.ts        # Document operations and helpers
    ├── useEmployments.ts      # Employment CRUD hooks
    └── usePersons.ts          # Person CRUD hooks
```

## Best Practices

1. **Always use custom hooks** - They provide better error handling and TypeScript support
2. **Don't edit generated files** - They're overwritten by codegen
3. **Use refetchQueries** - Keep UI in sync after mutations
4. **Leverage caching** - Apollo's cache reduces network requests
5. **Handle loading/error states** - Always show feedback to users

## Troubleshooting

### Types are out of sync
```bash
npm run codegen
```

### Query not updating after mutation
Add `refetchQueries` to your mutation:
```tsx
const [createPerson] = useMutation(CreatePersonDocument, {
  refetchQueries: [GetPersonsDocument]
});
```

### Network errors
Check that:
1. Gateway is running on `https://localhost:5003`
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
