# Application Architecture

## Overview

The EMS frontend follows a feature-based architecture with clear separation of concerns. This document outlines the architectural decisions, patterns, and conventions used throughout the application.

---

## Core Architecture Principles

### 1. Feature-Based Organization

Each major feature (persons, schools, positions, etc.) has its own folder under `pages/` containing:
- **List Page** - Displays paginated data with search and sorting
- **Form Page** - Handles both create and edit operations
- **Detail Page** - Shows read-only view with actions

### 2. Component Composition

- **Layout Components** - Structural components (MainLayout, Sidebar, Header)
- **UI Components** - Reusable utilities (color mode, AG Grid theme)
- **Feature Components** - Feature-specific components (documents, profile image)

### 3. Centralized API Layer

All API communication goes through TanStack Query with graphql-request for GraphQL operations and direct fetch calls for file operations via the Gateway REST endpoints.

---

## Provider Hierarchy

```tsx
<GoogleOAuthProvider>   // Google OAuth2 client context
  <ChakraProvider>      // Chakra UI theming and component context
    <ColorModeProvider> // Light/dark mode state management
      <BrowserRouter>   // React Router navigation
        <AuthProvider>  // Authentication state and token management
          <Routes>      // Route definitions
            <ProtectedRoute>  // Route guard for authenticated routes
              <MainLayout />  // Shared layout wrapper
              {/* Page Content */}
```

### Provider Responsibilities

| Provider              | Responsibility                                      |
|-----------------------|-----------------------------------------------------|
| `GoogleOAuthProvider` | Google OAuth2 client configuration                  |
| `ChakraProvider`      | Theme tokens, component styles, CSS reset           |
| `ColorModeProvider`   | Dark/light mode state, persistence                  |
| `BrowserRouter`       | URL-based navigation, history API                   |
| `AuthProvider`        | JWT tokens, user state, token refresh, logout       |
| `ProtectedRoute`      | Route guard, redirects unauthenticated users        |
| `MainLayout`          | Sidebar, header, content area structure             |

---

## Routing Architecture

### Route Structure

```
/login                      → Login Page (public)
/                           → Dashboard (protected)
/persons                    → Person List
/persons/new                → Create Person
/persons/:displayId         → Person Detail
/persons/:displayId/edit    → Edit Person
```

This pattern is repeated for all entities (schools, positions, salary-grades, items, employments).

### Authentication Flow

```
┌─────────────────┐     ┌──────────────┐     ┌─────────────┐
│   Login Page    │────▶│  Google      │────▶│  Backend    │
│   (Google Btn)  │     │  OAuth2      │     │  /auth/google
└─────────────────┘     └──────────────┘     └─────────────┘
                               │                    │
                               ▼                    ▼
                        ┌──────────────┐     ┌─────────────┐
                        │  ID Token    │────▶│  JWT Tokens │
                        │  from Google │     │  & User Info│
                        └──────────────┘     └─────────────┘
                                                    │
                                                    ▼
                        ┌──────────────────────────────────┐
                        │  Access Token → localStorage     │
                        │  Refresh Token → HttpOnly Cookie │
                        │  User Info → localStorage        │
                        └──────────────────────────────────┘
```

### Token Management

**Access Tokens**:
- Stored in `localStorage` with key `accessToken`
- Short-lived (15 minutes)
- Sent in `Authorization: Bearer <token>` header
- Used for all API requests

**Refresh Tokens**:
- Stored as HttpOnly cookies (name: `refreshToken`)
- Long-lived (7 days)
- Automatically sent by browser
- Cannot be accessed by JavaScript (XSS protection)
- Cookie attributes: `HttpOnly`, `Secure`, `SameSite=Strict`

**Token Refresh Flow**:
```
API Request → 401 Unauthorized → Interceptor
    │                              │
    ▼                              ▼
Wait for Refresh ◀── Refresh Token API Call
    │                  (Cookie auto-sent)
    ▼                              │
New Access Token ◀─────────────────┘
    │
    ▼
Retry Original Request
```

### Route Parameters

- **`:displayId`** - The 12-digit public identifier (not the internal GUID)
- Used for URL-friendly references that are safe to share

### Navigation Patterns

```tsx
// Programmatic navigation
const navigate = useNavigate();
navigate('/persons');
navigate(`/persons/${displayId}`);
navigate(`/persons/${displayId}/edit`);

// Link component
<Link to="/persons/new">Add New Person</Link>

// Row click in AG Grid
onRowClicked: (event) => navigate(`/persons/${event.data.displayId}`)
```

---

## State Management

### Local Component State

The application primarily uses local component state with React hooks:

```tsx
// Form state
const [formData, setFormData] = useState<CreatePersonDto>({...});

// Loading state
const [isLoading, setIsLoading] = useState(false);

// Error state
const [error, setError] = useState<string | null>(null);
```

### Server State

Data from the API is fetched using custom GraphQL hooks:

```tsx
import { usePerson } from '../hooks';

const PersonDetail = ({ displayId }: { displayId: number }) => {
  const { person, loading, error } = usePerson(displayId);

  if (loading) return <Spinner />;
  if (error) return <Alert>{error.message}</Alert>;
  if (!person) return <NotFound />;

  return <PersonCard person={person} />;
};
```

### Global State

- **Color Mode** - Managed by Chakra UI's ColorModeProvider
- **No Redux/Zustand** - Application complexity doesn't require global state management

---

## Data Flow Patterns

### List Page Data Flow

```
┌─────────────┐     ┌──────────────┐     ┌─────────────┐
│   User      │────▶│  AG Grid     │────▶│  API Call   │
│   Action    │     │  Datasource  │     │  (Server)   │
└─────────────┘     └──────────────┘     └─────────────┘
                           │                    │
                           ▼                    ▼
                    ┌──────────────┐     ┌─────────────┐
                    │  Update      │◀────│  Response   │
                    │  Grid Rows   │     │  Data       │
                    └──────────────┘     └─────────────┘
```

### Form Page Data Flow

```
┌─────────────┐     ┌──────────────┐     ┌─────────────┐
│  Form       │────▶│  Validate    │────▶│  API Call   │
│  Submit     │     │  Data        │     │  (Create/   │
└─────────────┘     └──────────────┘     │   Update)   │
                                         └─────────────┘
                                               │
                           ┌───────────────────┘
                           ▼
                    ┌──────────────┐
                    │  Navigate    │
                    │  to Detail   │
                    └──────────────┘
```

---

## Error Handling Strategy

### GraphQL Errors

```tsx
const { persons, loading, error } = usePersons();

if (error) {
  return <Alert>{error.message}</Alert>;
}
```

Global error handling is configured in `query-client.ts` via `QueryCache` and `MutationCache` `onError` callbacks, which handle auth errors and redirects automatically.

### Error Display

- **Inline Errors** - Form validation messages
- **Toast Notifications** - Success/error feedback (via Chakra UI)
- **Error States** - Full-page error displays for critical failures

### Loading States

```tsx
if (isLoading) {
  return (
    <Box p={8}>
      <VStack gap={4}>
        <Spinner size="xl" />
        <Text>Loading...</Text>
      </VStack>
    </Box>
  );
}
```

---

## Code Organization Patterns

### Feature Folder Structure

```
pages/persons/
├── index.ts              # Barrel exports
├── PersonsPage.tsx       # List page
├── PersonFormPage.tsx    # Create/Edit page
└── PersonDetailPage.tsx  # Detail page
```

### Barrel Exports

Each feature folder has an `index.ts` for clean imports:

```tsx
// pages/persons/index.ts
export { default as PersonsPage } from './PersonsPage';
export { default as PersonFormPage } from './PersonFormPage';
export { default as PersonDetailPage } from './PersonDetailPage';

// Usage
import { PersonsPage, PersonFormPage, PersonDetailPage } from './pages/persons';
```

### Component File Structure

```tsx
// 1. Imports
import { useParams, useNavigate } from 'react-router-dom';
import { Box, Heading, Spinner, Alert } from '@chakra-ui/react';
import { usePerson, type PersonResponseDto } from '../hooks';

// 2. Type definitions (if needed)
interface PersonDetailProps {
  // ...
}

// 3. Component definition
const PersonDetailPage = () => {
  // 3a. Router hooks
  const { displayId } = useParams();
  const navigate = useNavigate();

  // 3b. Data fetching with GraphQL hooks
  const { person, loading, error } = usePerson(Number(displayId));

  // 3c. Handlers
  const handleEdit = () => {
    navigate(`/persons/${displayId}/edit`);
  };

  // 3d. Render with loading/error states
  if (loading) return <Spinner />;
  if (error) return <Alert>{error.message}</Alert>;
  if (!person) return <Box>Person not found</Box>;

  return (
    <Box>
      <Heading>{person.fullName}</Heading>
      {/* Component JSX */}
    </Box>
  );
};

// 4. Export
export default PersonDetailPage;
```

---

## Performance Considerations

### AG Grid Optimization

- **Infinite Row Model** - Only loads visible data
- **Server-Side Pagination** - API handles data chunking
- **Debounced Search** - Reduces API calls during typing

### React Optimization

- **Memoization** - Use `useMemo` and `useCallback` for expensive operations
- **Lazy Loading** - Consider for route-based code splitting
- **Key Props** - Proper keys for list rendering

### Bundle Optimization

- **Tree Shaking** - Vite automatically removes unused code
- **Code Splitting** - Automatic per-route chunking
- **Dependency Optimization** - Vite pre-bundles dependencies

---

## TypeScript Patterns

### Strict Typing

```typescript
// GraphQL-generated types from codegen
import { type PersonResponseDto, type CreatePersonInput } from '../graphql/generated/graphql';

// Component with typed props
interface PersonFormProps {
  initialData?: PersonResponseDto;
  onSubmit: (data: CreatePersonInput) => void;
}

// Event handler typing
const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
  const { name, value } = e.target;
  setFormData(prev => ({ ...prev, [name]: value }));
};
```

### Type Guards

```typescript
// Check if in edit mode
const isEditMode = displayId !== undefined;

// Type narrowing
if (person) {
  // person is PersonResponseDto here
}
```

### Enum Usage

```tsx
import { Gender, CivilStatus } from '../graphql/generated/graphql';

// Enum in select options
<option value={Gender.Male}>Male</option>
<option value={Gender.Female}>Female</option>
```

---

## Testing Strategy

### Recommended Testing Approach

| Layer      | Testing Tool               | Focus                          |
|------------|----------------------------|--------------------------------|
| Components | Vitest + Testing Library   | UI behavior, user interactions |
| Hooks      | Vitest                     | Custom hook logic              |
| API Layer  | MSW (Mock Service Worker)  | API mocking, integration       |
| E2E        | Playwright/Cypress         | Full user flows                |

### Test File Organization

```
src/
├── components/
│   ├── layout/
│   │   ├── Sidebar.tsx
│   │   └── Sidebar.test.tsx     # Co-located tests
├── hooks/
│   ├── useDebounce.ts
│   └── useDebounce.test.ts
```
