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

- **Dashboard** with real-time statistics
- **Person Management** with addresses, contacts, documents, and profile images
- **Employment Management** with school assignments
- **School Management** with addresses and contacts
- **Position, Salary Grade, and Item Management**
- **Dark/Light Mode** theming support
- **AG Grid** integration for powerful data tables with infinite scrolling
- **Auto-generated API Client** from OpenAPI specification

---

## Quick Start

### Prerequisites

- Node.js 18+ 
- npm 9+
- Backend API running on `http://localhost:5062`

### Installation

```bash
# Navigate to application directory
cd application

# Install dependencies
npm install

# Start development server
npm run dev
```

The application will be available at `http://localhost:5173`.

### Available Scripts

| Script         | Command                             | Description                                  |
|----------------|-------------------------------------|----------------------------------------------|
| `dev`          | `vite`                              | Start development server with hot reload     |
| `build`        | `tsc -b && vite build`              | Type-check and build for production          |
| `lint`         | `eslint .`                          | Run ESLint on all files                      |
| `preview`      | `vite preview`                      | Preview production build locally             |
| `generate-api` | `openapi-generator-cli generate...` | Generate TypeScript API client from Swagger  |
| `format`       | `prettier --write src/`             | Format all source files                      |
| `format:check` | `prettier --check src/`             | Check formatting without modifying files     |

### Regenerating API Client

When the backend API changes, regenerate the TypeScript client:

```bash
npm run generate-api
```

This fetches the OpenAPI specification from `http://localhost:5062/swagger/v1/swagger.json` and generates typed API clients and models.

---

## Technology Stack

### Production Dependencies

| Package            | Version  | Purpose                                                          |
|--------------------|----------|------------------------------------------------------------------|
| `@chakra-ui/react` | ^3.31.0  | Component library for UI elements                                |
| `@emotion/react`   | ^11.14.0 | CSS-in-JS styling (Chakra UI dependency)                         |
| `ag-grid-react`    | ^35.0.1  | Enterprise data grid with sorting, filtering, infinite scrolling |
| `axios`            | ^1.13.2  | HTTP client for API communication                                |
| `react`            | ^19.2.0  | Core UI framework                                                |
| `react-dom`        | ^19.2.0  | React DOM rendering                                              |
| `react-router-dom` | ^7.12.0  | Client-side routing                                              |

### Development Dependencies

| Package                                           | Purpose                                                |
|---------------------------------------------------|--------------------------------------------------------|
| `@openapitools/openapi-generator-cli`             | Auto-generates TypeScript API client from OpenAPI spec |
| `@types/node`, `@types/react`, `@types/react-dom` | TypeScript definitions                                 |
| `@vitejs/plugin-react`                            | Vite React plugin for Fast Refresh                     |
| `eslint`, `typescript-eslint`                     | Code linting                                           |
| `prettier`                                        | Code formatting                                        |
| `typescript`                                      | TypeScript compiler                                    |
| `vite`                                            | Build tool and dev server                              |

---

## Project Structure

```
src/
├── api/                          # API layer
│   ├── config.ts                 # Axios configuration
│   ├── index.ts                  # API instances & exports
│   └── generated/                # Auto-generated OpenAPI client
│       ├── api/                  # API classes
│       └── models/               # TypeScript interfaces
├── assets/                       # Static assets
├── components/                   # Reusable components
│   ├── documents/                # Document-related components
│   ├── layout/                   # Layout components (MainLayout, Sidebar, Header)
│   └── ui/                       # UI utilities (color mode, AG Grid theme)
├── hooks/                        # Custom React hooks
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
| [API Integration](./API-INTEGRATION.md)  | API client and data fetching            |
| [Theming](./THEMING.md)                  | Theme configuration and styling         |
| [Development Guide](./DEVELOPMENT.md)    | Development workflow and best practices |
