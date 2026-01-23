# Theming Documentation

This document describes the theming system used in the EMS frontend application.

---

## Overview

The application uses **Chakra UI v3** for theming and styling. It supports both light and dark modes with automatic system preference detection.

---

## Theme Configuration

### File Structure

```
src/theme/
└── index.ts          # Main theme configuration
```

### Theme Setup

**File:** `src/theme/index.ts`

```typescript
import { createSystem, defaultConfig, defineConfig } from '@chakra-ui/react';

const config = defineConfig({
  theme: {
    tokens: {
      colors: {
        brand: {
          50: { value: '#e6f2ff' },
          100: { value: '#bdd9ff' },
          200: { value: '#94bfff' },
          300: { value: '#6ba6ff' },
          400: { value: '#428cff' },
          500: { value: '#1973e8' },  // Primary brand color
          600: { value: '#145cb8' },
          700: { value: '#0f4589' },
          800: { value: '#0a2e5a' },
          900: { value: '#05172b' },
        },
      },
    },
    semanticTokens: {
      colors: {
        // Sidebar colors
        'sidebar.bg': {
          value: { base: '{colors.gray.100}', _dark: '{colors.gray.800}' },
        },
        'sidebar.border': {
          value: { base: '{colors.gray.200}', _dark: '{colors.gray.700}' },
        },
        // Content area
        'content.bg': {
          value: { base: '{colors.gray.50}', _dark: '{colors.gray.900}' },
        },
        // Card colors
        'card.bg': {
          value: { base: '{colors.white}', _dark: '{colors.gray.800}' },
        },
        // Border colors
        'border.default': {
          value: { base: '{colors.gray.300}', _dark: '{colors.gray.600}' },
        },
        'border.subtle': {
          value: { base: '{colors.gray.200}', _dark: '{colors.gray.700}' },
        },
      },
    },
  },
});

export const system = createSystem(defaultConfig, config);
```

---

## Color Mode System

### How It Works

1. **Initial Load**: Checks `localStorage` for saved preference
2. **First Visit**: Falls back to system preference (`prefers-color-scheme`)
3. **User Toggle**: Saves preference to `localStorage`

### Storage Key

```typescript
const COLOR_MODE_KEY = 'ems-color-mode';
```

### Color Mode Provider

```tsx
// In main.tsx or App.tsx
import { ChakraProvider } from '@chakra-ui/react';
import { ColorModeProvider } from './components/ui/color-mode';
import { system } from './theme';

<ChakraProvider value={system}>
  <ColorModeProvider>
    <App />
  </ColorModeProvider>
</ChakraProvider>
```

### Using Color Mode

```tsx
import { useColorMode, useColorModeValue } from './components/ui/color-mode';

const MyComponent = () => {
  // Get current mode and toggle function
  const { colorMode, toggleColorMode } = useColorMode();
  
  // Get value based on mode
  const bgColor = useColorModeValue('white', 'gray.800');
  const borderColor = useColorModeValue('gray.200', 'gray.600');
  
  return (
    <Box bg={bgColor} borderColor={borderColor}>
      <Button onClick={toggleColorMode}>
        {colorMode === 'light' ? '🌙' : '☀️'}
      </Button>
    </Box>
  );
};
```

---

## Semantic Tokens

Semantic tokens provide consistent styling across light and dark modes.

### Available Tokens

| Token           | Light Mode | Dark Mode  | Usage                   |
|-----------------|------------|------------|-------------------------|
| `sidebar.bg`    | `gray.100` | `gray.800` | Sidebar background      |
| `sidebar.border`| `gray.200` | `gray.700` | Sidebar border          |
| `content.bg`    | `gray.50`  | `gray.900` | Main content background |
| `card.bg`       | `white`    | `gray.800` | Card backgrounds        |
| `border.default`| `gray.300` | `gray.600` | Default borders         |
| `border.subtle` | `gray.200` | `gray.700` | Subtle borders          |

### Using Semantic Tokens

```tsx
// Direct usage with semantic token
<Box bg="sidebar.bg" borderColor="border.default">
  Content
</Box>

// In style props
<Card.Root 
  bg="card.bg" 
  borderWidth="1px" 
  borderColor="border.subtle"
>
  Card Content
</Card.Root>
```

---

## Brand Colors

The brand color palette is based on blue tones:

| Token       | Hex Value | Usage                          |
|-------------|-----------|--------------------------------|
| `brand.50`  | `#e6f2ff` | Lightest backgrounds           |
| `brand.100` | `#bdd9ff` | Light backgrounds              |
| `brand.200` | `#94bfff` | Hover states (light)           |
| `brand.300` | `#6ba6ff` | Active states (light)          |
| `brand.400` | `#428cff` | Borders                        |
| `brand.500` | `#1973e8` | **Primary** (buttons, links)   |
| `brand.600` | `#145cb8` | Hover states (dark)            |
| `brand.700` | `#0f4589` | Active states (dark)           |
| `brand.800` | `#0a2e5a` | Dark text on light bg          |
| `brand.900` | `#05172b` | Darkest text                   |

### Using Brand Colors

```tsx
// Button with brand color
<Button colorPalette="brand">
  Click Me
</Button>

// Text with brand color
<Text color="brand.500">
  Highlighted text
</Text>

// Background with brand color
<Box bg="brand.50">
  Content
</Box>
```

---

## AG Grid Theming

AG Grid has its own theming system that integrates with the app's color mode.

### Theme Hook

**File:** `src/components/ui/use-ag-grid-theme.ts`

```typescript
import { useMemo } from 'react';
import { themeQuartz, colorSchemeDarkBlue } from 'ag-grid-community';
import { useColorMode } from './color-mode';

export const useAgGridTheme = () => {
  const { colorMode } = useColorMode();
  
  return useMemo(() => {
    const customParams = {
      fontFamily: 'inherit',
      fontSize: 14,
      headerFontSize: 14,
      rowHeight: 44,
      headerHeight: 44,
      borderRadius: 8,
    };
    
    if (colorMode === 'dark') {
      return themeQuartz
        .withPart(colorSchemeDarkBlue)
        .withParams(customParams);
    }
    
    return themeQuartz.withParams(customParams);
  }, [colorMode]);
};
```

### Using AG Grid Theme

```tsx
import { AgGridReact } from 'ag-grid-react';
import { useAgGridTheme } from '../../components/ui/use-ag-grid-theme';

const MyGrid = () => {
  const gridTheme = useAgGridTheme();
  
  return (
    <AgGridReact
      theme={gridTheme}
      columnDefs={columnDefs}
      rowData={rowData}
    />
  );
};
```

### AG Grid Theme Properties

| Property         | Value     | Description              |
|------------------|-----------|--------------------------|
| `fontFamily`     | `inherit` | Uses app's font stack    |
| `fontSize`       | `14`      | Cell text size           |
| `headerFontSize` | `14`      | Header text size         |
| `rowHeight`      | `44`      | Row height in pixels     |
| `headerHeight`   | `44`      | Header height in pixels  |
| `borderRadius`   | `8`       | Corner radius            |

---

## Global CSS

**File:** `src/index.css`

Provides base resets and Chakra UI overrides:

```css
:root {
  font-family: Inter, system-ui, Avenir, Helvetica, Arial, sans-serif;
  line-height: 1.5;
  font-weight: 400;
  font-synthesis: none;
  text-rendering: optimizeLegibility;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
}

body {
  margin: 0;
  min-height: 100vh;
}

#root {
  min-height: 100vh;
}
```

---

## Component-Specific Styling

### Card Styling

```tsx
<Card.Root
  bg="card.bg"
  borderWidth="1px"
  borderColor="border.subtle"
  borderRadius="lg"
  shadow="sm"
  _hover={{ shadow: 'md' }}
>
  <Card.Header>
    <Heading size="md">Title</Heading>
  </Card.Header>
  <Card.Body>
    Content
  </Card.Body>
</Card.Root>
```

### Form Styling

```tsx
<Field.Root>
  <Field.Label>Field Name</Field.Label>
  <Input 
    borderColor="border.default"
    _focus={{ 
      borderColor: 'brand.500',
      boxShadow: '0 0 0 1px var(--chakra-colors-brand-500)'
    }}
  />
</Field.Root>
```

### Button Variants

```tsx
// Primary button
<Button colorPalette="blue">Primary</Button>

// Secondary button
<Button variant="outline">Secondary</Button>

// Destructive button
<Button colorPalette="red">Delete</Button>

// Ghost button
<Button variant="ghost">Ghost</Button>
```

---

## Status Badges

Color-coded badges for entity status:

```tsx
// Active status
<Badge colorPalette="green">Active</Badge>

// Inactive status
<Badge colorPalette="gray">Inactive</Badge>

// Pending status
<Badge colorPalette="yellow">Pending</Badge>

// Error status
<Badge colorPalette="red">Error</Badge>
```

### Entity-Specific Badges

```tsx
// Gender
<Badge colorPalette={gender === 'Male' ? 'blue' : 'pink'}>
  {gender}
</Badge>

// Employment Status
<Badge colorPalette={status === 'Permanent' ? 'green' : 'blue'}>
  {status}
</Badge>
```

---

## Best Practices

### 1. Use Semantic Tokens

```tsx
// ✅ Good - Uses semantic token
<Box bg="card.bg">

// ❌ Avoid - Hardcoded colors
<Box bg="white">
```

### 2. Use Color Mode Value

```tsx
// ✅ Good - Responds to color mode
const bg = useColorModeValue('gray.100', 'gray.700');

// ❌ Avoid - Static color
const bg = 'gray.100';
```

### 3. Use Color Palettes

```tsx
// ✅ Good - Uses color palette
<Button colorPalette="blue">

// ❌ Avoid - Direct color reference
<Button bg="blue.500" color="white">
```

### 4. Consistent Spacing

```tsx
// Use Chakra's spacing scale
<Box p={4} mb={6} gap={2}>
// Translates to: padding: 16px, margin-bottom: 24px, gap: 8px
```

### 5. Responsive Design

```tsx
// Use responsive array syntax
<Grid templateColumns={{ base: '1fr', md: 'repeat(2, 1fr)', lg: 'repeat(3, 1fr)' }}>
```
