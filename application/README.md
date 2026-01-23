# Employee Management System - Frontend

Modern desktop and web application built with React, TypeScript, and Vite.

## Technology Stack

- **React** - UI framework
- **TypeScript** - Static typing
- **Vite** - Build tool and dev server
- **Radix-UI** - Component library
- **Axios** - HTTP client for API communication
- **CSS** - Styling

## Project Structure

```
application/
├── src/
│   ├── components/       # Reusable UI components
│   ├── App.tsx          # Main application component
│   ├── main.tsx         # Application entry point
│   └── ...
├── public/              # Static assets
└── package.json         # Dependencies and scripts
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

## API Integration

Coming soon...
import reactDom from 'eslint-plugin-react-dom'

export default defineConfig([
  globalIgnores(['dist']),
  {
    files: ['**/*.{ts,tsx}'],
    extends: [
      // Other configs...
      // Enable lint rules for React
      reactX.configs['recommended-typescript'],
      // Enable lint rules for React DOM
      reactDom.configs.recommended,
    ],
    languageOptions: {
      parserOptions: {
        project: ['./tsconfig.node.json', './tsconfig.app.json'],
        tsconfigRootDir: import.meta.dirname,
      },
      // other options...
    },
  },
])
```
