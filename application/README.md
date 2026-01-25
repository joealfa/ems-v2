# Employee Management System - Frontend

Modern desktop and web application built with React, TypeScript, and Vite.

## Technology Stack

- **React 19** - UI framework
- **TypeScript** - Static typing
- **Vite** - Build tool and dev server
- **Chakra-UI** - Component library
- **AG Grid** - Data grid component
- **Axios** - HTTP client for API communication
- **React Router** - Client-side routing
- **OpenAPI Generator** - Auto-generated API client

## Project Structure

```
application/
├── src/
│   ├── api/              # API client and configuration
│   │   ├── config.ts     # Axios configuration
│   │   ├── generated/    # Auto-generated API client from OpenAPI
│   │   └── index.ts      # API exports
│   ├── assets/           # Static assets (images, fonts, etc.)
│   ├── components/       # Reusable UI components
│   │   ├── auth/         # Authentication components
│   │   ├── documents/    # Document-related components
│   │   ├── layout/       # Layout components
│   │   └── ui/           # Generic UI components
│   ├── contexts/         # React context providers
│   │   └── AuthContext.tsx
│   ├── hooks/            # Custom React hooks
│   │   ├── useAuth.ts
│   │   └── useDebounce.ts
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

### Install Dependencies

```bash
npm install
```

### Development

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


| Script                 | Description                           |
|------------------------|---------------------------------------|
| `npm run dev`          | Start development server              |
| `npm run build`        | Build for production                  |
| `npm run preview`      | Preview production build              |
| `npm run lint`         | Run ESLint                            |
| `npm run format`       | Format code with Prettier             |
| `npm run format:check` | Check code formatting                 |
| `npm run generate-api` | Generate API client from OpenAPI spec |

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

## API Integration

The application uses an auto-generated API client from the backend's OpenAPI specification. To regenerate the client after API changes:

```bash
npm run generate-api
```

The generated client is located in `src/api/generated/`.
