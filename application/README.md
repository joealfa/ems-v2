# Employee Management System - Frontend

Modern desktop and web application built with React, TypeScript, and Vite.

## Technology Stack

- **React 19** - UI framework
- **TypeScript** - Static typing
- **Vite** - Build tool and dev server
- **Chakra-UI** - Component library
- **AG Grid** - Data grid component
- **TanStack Query** - Server state management and data fetching
- **graphql-request** - Lightweight GraphQL client
- **GraphQL Code Generator** - Auto-generated types and documents
- **React Router** - Client-side routing

## Project Structure

```
application/
├── src/
│   ├── graphql/          # GraphQL configuration
│   │   ├── graphql-client.ts  # graphql-request client setup
│   │   ├── query-client.ts    # TanStack QueryClient configuration
│   │   ├── QueryProvider.tsx  # TanStack Query provider wrapper
│   │   ├── query-keys.ts     # Query key factory for cache management
│   │   ├── operations/   # GraphQL queries and mutations (.graphql files)
│   │   └── generated/    # Auto-generated types (DO NOT EDIT)
│   ├── assets/           # Static assets (images, fonts, etc.)
│   ├── components/       # Reusable UI components
│   │   ├── auth/         # Authentication components
│   │   ├── documents/    # Document-related components
│   │   ├── layout/       # Layout components
│   │   └── ui/           # Generic UI components
│   ├── contexts/         # React context providers
│   │   └── AuthContext.tsx
│   ├── hooks/            # Custom React hooks (TanStack Query)
│   │   ├── index.ts        # Barrel exports
│   │   ├── useAuth.ts
│   │   ├── useAuthMutations.ts
│   │   ├── useDashboard.ts
│   │   ├── useDebounce.ts
│   │   ├── useDocuments.ts
│   │   ├── useEmployments.ts
│   │   ├── useItems.ts
│   │   ├── usePersons.ts
│   │   ├── usePositions.ts
│   │   ├── useSalaryGrades.ts
│   │   └── useSchools.ts
│   ├── pages/            # Page components
│   │   ├── Dashboard.tsx
│   │   ├── LoginPage.tsx
│   │   ├── employments/
│   │   ├── items/
│   │   ├── persons/
│   │   ├── positions/
│   │   ├── salary-grades/
│   │   └── schools/
│   ├── theme/            # Chakra-UI theme configuration
│   ├── App.tsx           # Main application component
│   ├── main.tsx          # Application entry point
│   └── index.css         # Global styles
├── public/               # Static assets served at root
└── package.json          # Dependencies and scripts
```

## Getting Started

### 1. Environment Setup

Copy the example environment file and configure it:

```bash
cp .env.example .env
```

Edit `.env` and add your Google OAuth Client ID:

```env
DEV=true
VITE_API_BASE_URL=https://localhost:7166
VITE_GRAPHQL_URL=https://localhost:5003/graphql
VITE_GOOGLE_CLIENT_ID=your-actual-google-client-id
```

**Important:** Never commit the `.env` file to git. It's already in `.gitignore`.

### 2. Install Dependencies

```bash
npm install
```

### 3. Development

```bash
# Start development server
npm run dev
```

The application will be available at `http://localhost:5173/`

### Build for Production

```bash
npm run build
```

### Preview Production Build

```bash
npm run preview
```

## Available Scripts


| Script                 | Description                              |
|------------------------|------------------------------------------|
| `npm run dev`          | Start development server                 |
| `npm run build`        | Build for production                     |
| `npm run preview`      | Preview production build                 |
| `npm run lint`         | Run ESLint                               |
| `npm run format`       | Format code with Prettier                |
| `npm run format:check` | Check code formatting                    |
| `npm run codegen`      | Generate TypeScript types from GraphQL   |

## Coding Standards

- Use semicolons at the end of each statement
- Use single quotes for strings
- Use function-based components
- Use arrow functions for callbacks
- Follow TypeScript best practices

## UI Guidelines

- Modern and clean design
- Light and dark mode support via toggle
- Responsive layout
- Accessible components using Chakra-UI
- Use AG Grid for displaying tabular data with sorting, filtering, and pagination

## Security

- **Access Tokens** - Stored in localStorage for API authorization (short-lived, 15 minutes)
- **Refresh Tokens** - Stored as HttpOnly cookies for enhanced security (long-lived, 7 days)
- **Automatic Token Refresh** - AuthContext handles token expiration seamlessly
- **CSRF Protection** - SameSite cookie policy prevents cross-site attacks

## API Integration

The application uses **TanStack Query** with **graphql-request** and **GraphQL Code Generator** to communicate with the backend via a GraphQL Gateway. To regenerate types after schema changes:

```bash
npm run codegen
```

The generated types are located in `src/graphql/generated/`.

For detailed usage, see [GraphQL Usage Guide](../docs/application/GRAPHQL_USAGE.md).
