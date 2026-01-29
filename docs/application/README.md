# Employee Management System - Frontend Application Documentation

## Table of Contents

1. [Overview](#overview)
2. [Quick Start](#quick-start)
3. [Technology Stack](#technology-stack)
4. [Project Structure](#project-structure)
5. [Additional Documentation](#additional-documentation)

---

## Overview

The Employee Management System (EMS) frontend is a modern React-based single-page application (SPA) built with TypeScript and Vite. It provides a comprehensive interface for managing employees, persons, schools, positions, salary grades, items, and employments.

### Key Features

- **Google OAuth2 Authentication** with JWT access and refresh tokens
- **Dashboard** with real-time statistics
- **Person Management** with addresses, contacts, documents, and profile images
- **Employment Management** with school assignments
- **School Management** with addresses and contacts
- **Position, Salary Grade, and Item Management**
- **Dark/Light Mode** theming support
- **AG Grid** integration for powerful data tables with infinite scrolling
- **Apollo Client** with GraphQL Code Generator for type-safe API communication
- **Protected Routes** with automatic token refresh

---

## Quick Start

### Prerequisites

- Node.js 18+
- npm 9+
- GraphQL Gateway running on `https://localhost:5003`
- Google OAuth2 Client ID (for authentication)

### Installation

```bash
# Navigate to application directory
cd application

# Install dependencies
npm install

# Create .env file with required environment variables
echo "VITE_GRAPHQL_URL=https://localhost:5003/graphql" > .env
echo "VITE_GOOGLE_CLIENT_ID=your-google-client-id" >> .env

# Start development server
npm run dev
```

The application will be available at `http://localhost:5173`.

### Environment Variables

| Variable | Description | Default |
|----------|-------------|--------|
| `VITE_GRAPHQL_URL` | GraphQL Gateway URL | `https://localhost:5003/graphql` |
| `VITE_GOOGLE_CLIENT_ID` | Google OAuth2 Client ID | Required |

### Available Scripts

| Script         | Command                               | Description                                  |
|----------------|---------------------------------------|----------------------------------------------|
| `dev`          | `vite`                                | Start development server with hot reload     |
| `build`        | `tsc -b && vite build`                | Type-check and build for production          |
| `lint`         | `eslint .`                            | Run ESLint on all files                      |
| `preview`      | `vite preview`                        | Preview production build locally             |
| `codegen`      | `graphql-codegen --config codegen.ts` | Generate TypeScript types from GraphQL schema|
| `format`       | `prettier --write src/`               | Format all source files                      |
| `format:check` | `prettier --check src/`               | Check formatting without modifying files     |

### Regenerating GraphQL Types

When the GraphQL schema changes on the gateway, regenerate the TypeScript types:

```bash
npm run codegen
```

This fetches the schema from `https://localhost:5003/graphql` and generates typed queries, mutations, and TypeScript interfaces.

---

## Technology Stack

### Production Dependencies

| Package              | Version  | Purpose                                                          |
|----------------------|----------|------------------------------------------------------------------|
| `@apollo/client`     | ^3.x     | GraphQL client for data fetching and caching                     |
| `@chakra-ui/react`   | ^3.31.0  | Component library for UI elements                                |
| `@emotion/react`     | ^11.14.0 | CSS-in-JS styling (Chakra UI dependency)                         |
| `@react-oauth/google`| ^0.13.4  | Google OAuth2 authentication for React                           |
| `ag-grid-react`      | ^35.0.1  | Enterprise data grid with sorting, filtering, infinite scrolling |
| `graphql`            | ^16.x    | GraphQL language support                                         |
| `react`              | ^19.2.0  | Core UI framework                                                |
| `react-dom`          | ^19.2.0  | React DOM rendering                                              |
| `react-router-dom`   | ^7.12.0  | Client-side routing                                              |

### Development Dependencies

| Package                                           | Purpose                                       |
|---------------------------------------------------|-----------------------------------------------|
| `@graphql-codegen/cli`                            | GraphQL Code Generator CLI                    |
| `@graphql-codegen/typescript`                     | TypeScript types generation                   |
| `@graphql-codegen/typescript-operations`          | TypeScript operation types                    |
| `@graphql-codegen/typescript-react-apollo`        | React Apollo hooks generation                 |
| `@types/node`, `@types/react`, `@types/react-dom` | TypeScript definitions                        |
| `@vitejs/plugin-react`                            | Vite React plugin for Fast Refresh            |
| `eslint`, `typescript-eslint`                     | Code linting                                  |
| `prettier`                                        | Code formatting                               |
| `typescript`                                      | TypeScript compiler                           |
| `vite`                                            | Build tool and dev server                     |

---

## Project Structure

```
src/
├── graphql/                      # GraphQL layer
│   ├── client.ts                 # Apollo Client configuration
│   ├── ApolloProvider.tsx        # React provider wrapper
│   ├── operations/               # GraphQL queries and mutations
│   │   ├── persons.graphql
│   │   ├── employments.graphql
│   │   ├── schools.graphql
│   │   └── ...
│   └── generated/                # Auto-generated types (DO NOT EDIT)
│       └── graphql.ts
├── assets/                       # Static assets
├── components/                   # Reusable components
│   ├── documents/                # Document-related components
│   ├── layout/                   # Layout components (MainLayout, Sidebar, Header)
│   └── ui/                       # UI utilities (color mode, AG Grid theme)
├── hooks/                        # Custom React hooks
│   ├── usePersons.ts
│   ├── useEmployments.ts
│   ├── useSchools.ts
│   └── ...
├── pages/                        # Page components by feature
│   ├── Dashboard.tsx
│   ├── persons/
│   ├── schools/
│   ├── positions/
│   ├── salary-grades/
│   ├── items/
│   └── employments/
├── theme/                        # Chakra UI theme configuration
├── App.tsx                       # Root component with routing
├── App.css                       # Component styles
├── index.css                     # Global CSS
└── main.tsx                      # Application entry point
```

---

## Additional Documentation

| Document                                 | Description                             |
|------------------------------------------|-----------------------------------------|
| [Architecture](./ARCHITECTURE.md)        | Application architecture and patterns   |
| [Components](./COMPONENTS.md)            | Component library and usage             |
| [Pages](./PAGES.md)                      | Page structure and functionality        |
| [API Integration](./API-INTEGRATION.md)  | GraphQL client and data fetching        |
| [GraphQL Usage](./GRAPHQL_USAGE.md)      | GraphQL hooks and operations guide      |
| [Theming](./THEMING.md)                  | Theme configuration and styling         |
| [Development Guide](./DEVELOPMENT.md)    | Development workflow and best practices |
