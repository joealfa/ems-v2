# Development Guide

This document provides guidelines for developing and contributing to the EMS frontend application.

---

## Prerequisites

- **Node.js** 18.x or higher
- **npm** 9.x or higher
- **VS Code** (recommended IDE)
- **Backend API** running on `http://localhost:5031`
- **Google Cloud Console** project with OAuth2 credentials

---

## Getting Started

### Initial Setup

```bash
# Clone the repository (if not already done)
git clone <repository-url>
cd ems-v2/application

# Install dependencies
npm install

# Create environment file
cp .env.example .env
# Edit .env with your configuration
```

### Environment Configuration

Create a `.env` file in the `application` directory:

```env
VITE_API_BASE_URL=http://localhost:5031
VITE_GOOGLE_CLIENT_ID=your-google-client-id.apps.googleusercontent.com
```

### Google OAuth2 Setup

1. Go to [Google Cloud Console](https://console.cloud.google.com/)
2. Create a new project or select existing
3. Navigate to **APIs & Services** > **Credentials**
4. Create **OAuth 2.0 Client ID** (Web application)
5. Add authorized JavaScript origins:
   - `http://localhost:5173` (development)
6. Copy the Client ID to your `.env` file

### VS Code Extensions (Recommended)

- **ESLint** - Code linting
- **Prettier** - Code formatting
- **ES7+ React/Redux/React-Native snippets** - React snippets
- **TypeScript Importer** - Auto-import suggestions
- **GitLens** - Git history and blame

---

## Development Workflow

### 1. Start Development Server

```bash
npm run dev
```

The app will be available at `http://localhost:5173` with hot module replacement.

### 2. Ensure Backend is Running

The backend API must be running at `http://localhost:5031`. If not:

```bash
cd ../server
dotnet run --project EmployeeManagementSystem.Api
```

### 3. Code Changes

- Make changes to source files
- Vite will automatically reload affected modules
- Check browser console for errors

### 4. Run Linting

```bash
npm run lint
```

Fix any linting errors before committing.

### 5. Format Code

```bash
npm run format
```

Or check formatting without changes:

```bash
npm run format:check
```

---

## Project Scripts

| Script                 | Description                |
|------------------------|----------------------------|
| `npm run dev`          | Start development server   |
| `npm run build`        | Build for production       |
| `npm run preview`      | Preview production build   |
| `npm run lint`         | Run ESLint                 |
| `npm run format`       | Format code with Prettier  |
| `npm run format:check` | Check code formatting      |
| `npm run codegen`      | Regenerate GraphQL types   |

---

## Code Style Guidelines

### TypeScript

- Use strict typing - avoid `any` when possible
- Define interfaces for props and state
- Use `type` for unions, `interface` for objects

```typescript
// ✅ Good
interface PersonFormProps {
  displayId?: number;
  onSubmit: (data: CreatePersonDto) => void;
}

// ❌ Avoid
const MyComponent = (props: any) => { ... }
```

### React Components

- Use functional components with hooks
- Use arrow function syntax
- Keep components focused and small

```tsx
// ✅ Good
const PersonCard: React.FC<PersonCardProps> = ({ person, onEdit }) => {
  const [isHovered, setIsHovered] = useState(false);
  
  return (
    <Card onMouseEnter={() => setIsHovered(true)}>
      {/* ... */}
    </Card>
  );
};

// ❌ Avoid class components
class PersonCard extends React.Component { ... }
```

### File Naming

| Type       | Pattern                     | Example           |
|------------|-----------------------------|-------------------|
| Components | PascalCase                  | `PersonCard.tsx`  |
| Hooks      | camelCase with `use` prefix | `useDebounce.ts`  |
| Utilities  | camelCase                   | `formatDate.ts`   |
| Folders    | kebab-case                  | `salary-grades/`  |

### Import Order

```typescript
// 1. React and core libraries
import { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';

// 2. Third-party components/libraries
import { Box, Heading, Button } from '@chakra-ui/react';
import { AgGridReact } from 'ag-grid-react';

// 3. Internal components
import { MainLayout } from '../../components/layout';
import { PersonDocuments } from '../../components/documents';

// 4. API and types
import { personsApi, type PersonResponseDto } from '../../api';

// 5. Hooks and utilities
import { useDebounce } from '../../hooks';

// 6. Styles (if any)
import './PersonCard.css';
```

---

## Creating New Features

### Adding a New Entity (Full CRUD)

1. **Create page folder:**
   ```
   src/pages/new-entity/
   ├── index.ts
   ├── NewEntityPage.tsx      (List)
   ├── NewEntityFormPage.tsx  (Create/Edit)
   └── NewEntityDetailPage.tsx (View)
   ```

2. **Add routes in App.tsx:**
   ```tsx
   <Route path="new-entities" element={<NewEntityPage />} />
   <Route path="new-entities/new" element={<NewEntityFormPage />} />
   <Route path="new-entities/:displayId" element={<NewEntityDetailPage />} />
   <Route path="new-entities/:displayId/edit" element={<NewEntityFormPage />} />
   ```

3. **Add to sidebar:**
   ```tsx
   // In Sidebar.tsx
   { label: 'New Entities', path: '/new-entities', icon: '📦' }
   ```

4. **Regenerate API client** (if backend has new endpoints):
   ```bash
   npm run generate-api
   ```

### Adding a New Component

1. **Choose location:**
   - `components/ui/` - Reusable UI utilities
   - `components/layout/` - Layout components
   - `components/{feature}/` - Feature-specific components

2. **Create component file:**
   ```tsx
   // components/ui/MyComponent.tsx
   interface MyComponentProps {
     title: string;
     children: React.ReactNode;
   }
   
   export const MyComponent: React.FC<MyComponentProps> = ({ title, children }) => {
     return (
       <Box>
         <Heading>{title}</Heading>
         {children}
       </Box>
     );
   };
   ```

3. **Export from barrel file:**
   ```typescript
   // components/ui/index.ts
   export { MyComponent } from './MyComponent';
   ```

### Adding a Custom Hook

1. **Create in hooks folder:**
   ```typescript
   // hooks/useMyHook.ts
   import { useState, useEffect } from 'react';
   
   export const useMyHook = (initialValue: string) => {
     const [value, setValue] = useState(initialValue);
     
     useEffect(() => {
       // Effect logic
     }, [value]);
     
     return { value, setValue };
   };
   ```

2. **Export from barrel file:**
   ```typescript
   // hooks/index.ts
   export { useDebounce } from './useDebounce';
   export { useMyHook } from './useMyHook';
   ```

---

## Working with GraphQL

### After Schema Changes

When the GraphQL Gateway schema changes:

1. **Ensure Gateway is running** on `https://localhost:5003`
2. **Regenerate types:**
   ```bash
   npm run codegen
   ```
3. **Check for TypeScript errors**
4. **Update components using changed queries/mutations**

### Adding New GraphQL Operations

1. **Create `.graphql` file** in `src/graphql/operations/`:
   ```graphql
   # src/graphql/operations/new-entity.graphql
   query GetNewEntities($pageNumber: Int, $pageSize: Int) {
     newEntities(pageNumber: $pageNumber, pageSize: $pageSize) {
       items {
         displayId
         name
       }
       totalCount
     }
   }
   ```

2. **Run codegen:**
   ```bash
   npm run codegen
   ```

3. **Create custom hook** in `src/hooks/`:
   ```typescript
   import { useQuery } from '@apollo/client';
   import { GetNewEntitiesDocument } from '../graphql/generated/graphql';

   export const useNewEntities = (variables?: { pageNumber?: number; pageSize?: number }) => {
     const { data, loading, error, refetch } = useQuery(GetNewEntitiesDocument, { variables });
     return {
       newEntities: data?.newEntities?.items ?? [],
       totalCount: data?.newEntities?.totalCount ?? 0,
       loading,
       error,
       refetch,
     };
   };
   ```

4. **Export from barrel file** `src/hooks/index.ts`

---

## Authentication & Security

### Token Storage

The application uses a dual-token system:

**Access Tokens (localStorage)**:
- Short-lived (15 minutes)
- Used for API authorization
- Stored in `localStorage` with key `accessToken`

**Refresh Tokens (HttpOnly Cookies)**:
- Long-lived (7 days)
- Automatically sent by browser
- Cannot be accessed by JavaScript (XSS protection)
- Set by backend with `HttpOnly`, `Secure`, and `SameSite=Strict` flags

### Authentication Flow

```typescript
// 1. Login - Google OAuth
const { accessToken, user } = await authApi.apiV1AuthGooglePost({ idToken });
localStorage.setItem('accessToken', accessToken);
localStorage.setItem('user', JSON.stringify(user));
// Refresh token automatically stored as HttpOnly cookie by backend

// 2. API Requests - Automatic via interceptor
axiosInstance.interceptors.request.use((config) => {
  const token = localStorage.getItem('accessToken');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// 3. Token Refresh - Automatic on 401
// Refresh token sent automatically via cookie
const { accessToken } = await axiosInstance.post('/api/v1/auth/refresh');
localStorage.setItem('accessToken', accessToken);

// 4. Logout
await axiosInstance.post('/api/v1/auth/revoke');
localStorage.clear();
// Backend clears refresh token cookie
```

### Security Best Practices

- ✅ Never store refresh tokens in `localStorage`
- ✅ Use `withCredentials: true` for cookie-based auth
- ✅ Clear all auth data on logout
- ✅ Validate token expiration before API calls
- ❌ Don't log or expose tokens in console
- ❌ Don't store sensitive data in state or localStorage

---

## Debugging

### Browser DevTools

- **React DevTools** - Component tree, props, state
- **Network Tab** - API requests/responses
- **Console** - Errors and logs

### Common Issues

#### API Connection Failed

```
Error: Network Error
```

**Solution:** Ensure backend is running at `http://localhost:5062`

#### CORS Error

```
Access-Control-Allow-Origin
```

**Solution:** Backend should have CORS configured for `http://localhost:5173`

#### Type Errors After API Change

```
Property 'newField' does not exist
```

**Solution:** Regenerate API client: `npm run generate-api`

---

## Testing (Recommended Setup)

### Unit Tests with Vitest

```bash
npm install -D vitest @testing-library/react @testing-library/jest-dom jsdom
```

**vitest.config.ts:**
```typescript
import { defineConfig } from 'vitest/config';
import react from '@vitejs/plugin-react';

export default defineConfig({
  plugins: [react()],
  test: {
    environment: 'jsdom',
    globals: true,
    setupFiles: './src/test/setup.ts',
  },
});
```

### Example Test

```typescript
// PersonCard.test.tsx
import { render, screen } from '@testing-library/react';
import { PersonCard } from './PersonCard';

describe('PersonCard', () => {
  it('renders person name', () => {
    render(<PersonCard person={{ fullName: 'John Doe' }} />);
    expect(screen.getByText('John Doe')).toBeInTheDocument();
  });
});
```

---

## Building for Production

### Build Command

```bash
npm run build
```

This will:
1. Type-check with TypeScript
2. Bundle with Vite
3. Output to `dist/` folder

### Preview Build

```bash
npm run preview
```

Opens a local server to preview the production build.

### Build Artifacts

```
dist/
├── index.html
├── assets/
│   ├── index-[hash].js
│   ├── index-[hash].css
│   └── [other assets]
```

---

## Environment Configuration

### Development vs Production

| Setting     | Development              | Production              |
|-------------|--------------------------|-------------------------|
| API URL     | `http://localhost:5062`  | Configure in deployment |
| Source Maps | Enabled                  | Disabled                |
| HMR         | Enabled                  | N/A                     |

### Configuring API URL

For different environments, update `src/api/config.ts`:

```typescript
export const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5062';
```

Then set environment variable:

```bash
VITE_API_URL=https://api.production.com npm run build
```

---

## Troubleshooting

### Clear Cache and Reinstall

```bash
rm -rf node_modules
rm package-lock.json
npm install
```

### Clear Vite Cache

```bash
rm -rf node_modules/.vite
npm run dev
```

### TypeScript Errors Not Updating

Restart TypeScript server in VS Code:
- `Cmd/Ctrl + Shift + P` → "TypeScript: Restart TS Server"
