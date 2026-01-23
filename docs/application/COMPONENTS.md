# Component Library

This document describes all reusable components in the EMS frontend application.

---

## Layout Components

Located in `src/components/layout/`

### MainLayout

The root layout component that provides the application shell.

**File:** `MainLayout.tsx`

**Structure:**
```
┌────────────────────────────────────────────────┐
│                   Header                       │
├─────────────┬──────────────────────────────────┤
│             │                                  │
│   Sidebar   │          Content Area            │
│             │         (Outlet)                 │
│             │                                  │
└─────────────┴──────────────────────────────────┘
```

**Usage:**
```tsx
// In App.tsx routing
<Route path="/" element={<MainLayout />}>
  <Route index element={<Dashboard />} />
  {/* Child routes render in Outlet */}
</Route>
```

**Features:**
- Fixed sidebar (250px width)
- Flexible content area
- Responsive header
- Uses Chakra UI Flex layout

---

### Sidebar

Navigation menu with links to all major sections.

**File:** `Sidebar.tsx`

**Props:** None (standalone component)

**Navigation Items:**

| Label         | Path             | Icon |
|---------------|------------------|------|
| Dashboard     | `/`              | 📊   |
| Persons       | `/persons`       | 👥   |
| Schools       | `/schools`       | 🏫   |
| Positions     | `/positions`     | 💼   |
| Salary Grades | `/salary-grades` | 💰   |
| Items         | `/items`         | 📦   |
| Employments   | `/employments`   | 📋   |

**Features:**
- Active link highlighting using `useLocation()`
- Hover effects
- Dark/light mode compatible

**Styling:**
```tsx
// Active item styling
bg={isActive ? 'blue.500' : 'transparent'}
color={isActive ? 'white' : 'inherit'}
```

---

### Header

Top navigation bar with title and theme toggle.

**File:** `Header.tsx`

**Props:** None (standalone component)

**Features:**
- Application title display
- Dark/light mode toggle button
- Fixed height (60px)
- Responsive styling

**Theme Toggle:**
```tsx
<Button onClick={toggleColorMode} size="sm">
  {colorMode === 'light' ? '🌙' : '☀️'}
</Button>
```

---

## UI Components

Located in `src/components/ui/`

### Color Mode Utilities

**File:** `color-mode.tsx`

#### ColorModeProvider

Context provider for managing color mode state.

```tsx
<ColorModeProvider>
  {children}
</ColorModeProvider>
```

#### useColorMode Hook

Access and toggle the current color mode.

```tsx
const { colorMode, toggleColorMode } = useColorMode();

// colorMode: 'light' | 'dark'
// toggleColorMode: () => void
```

#### useColorModeValue Hook

Get a value based on current color mode.

```tsx
const bgColor = useColorModeValue('gray.100', 'gray.800');
const textColor = useColorModeValue('gray.900', 'white');
```

**Color Mode Persistence:**
- Stored in `localStorage`
- Key: `ems-color-mode`
- Respects system preference on first visit

---

### AG Grid Theme Hook

**File:** `use-ag-grid-theme.ts`

#### useAgGridTheme Hook

Returns the appropriate AG Grid theme based on current color mode.

```tsx
import { useAgGridTheme } from '../../components/ui/use-ag-grid-theme';

const MyGrid = () => {
  const gridTheme = useAgGridTheme();
  
  return (
    <AgGridReact
      theme={gridTheme}
      // ...other props
    />
  );
};
```

**Theme Configuration:**

| Mode  | Theme                                        |
|-------|----------------------------------------------|
| Light | `themeQuartz` with default colors            |
| Dark  | `themeQuartz.withPart(colorSchemeDarkBlue)`  |

**Custom Parameters:**
```typescript
const customParams = {
  fontFamily: 'inherit',
  fontSize: 14,
  headerFontSize: 14,
  rowHeight: 44,
  headerHeight: 44,
  borderRadius: 8,
};
```

---

## Document Components

Located in `src/components/documents/`

### PersonDocuments

Manages document list, upload, download, and delete for a person.

**File:** `PersonDocuments.tsx`

**Props:**
```typescript
interface PersonDocumentsProps {
  personDisplayId: number;    // The person's display ID
  isEditable?: boolean;       // Enable upload/delete actions
}
```

**Features:**
- Document list with pagination
- File upload with type/size validation
- Download document
- Delete document with confirmation
- Document type selection (PDF, Word, Excel, etc.)
- File description input

**Allowed File Types:**
- PDF (`.pdf`)
- Word (`.doc`, `.docx`)
- Excel (`.xls`, `.xlsx`)
- PowerPoint (`.ppt`, `.pptx`)
- Images (`.jpg`, `.jpeg`, `.png`)

**Max File Size:** 50 MB

**Usage:**
```tsx
<PersonDocuments 
  personDisplayId={person.displayId} 
  isEditable={true} 
/>
```

---

### ProfileImageUpload

Handles profile image upload and deletion with preview.

**File:** `ProfileImageUpload.tsx`

**Props:**
```typescript
interface ProfileImageUploadProps {
  personDisplayId: number;           // The person's display ID
  currentImageUrl?: string | null;   // Current profile image URL
  onImageChange?: (url: string | null) => void;  // Callback on change
  isEditable?: boolean;              // Enable upload/delete actions
}
```

**Features:**
- Image preview with placeholder
- Upload new image
- Delete current image
- File type validation (JPG, PNG only)
- Size validation (max 5 MB)
- Loading states

**Usage:**
```tsx
<ProfileImageUpload
  personDisplayId={person.displayId}
  currentImageUrl={person.profileImageUrl}
  onImageChange={(url) => setProfileImage(url)}
  isEditable={true}
/>
```

---

## Common UI Patterns

### Card Layout

Used for dashboard stats and detail sections:

```tsx
<Card.Root>
  <Card.Header>
    <Heading size="md">Title</Heading>
  </Card.Header>
  <Card.Body>
    {/* Content */}
  </Card.Body>
</Card.Root>
```

### Form Layout

Standard form structure:

```tsx
<VStack gap={4} align="stretch">
  <Field.Root required>
    <Field.Label>Field Name</Field.Label>
    <Input 
      name="fieldName"
      value={formData.fieldName}
      onChange={handleChange}
    />
    <Field.ErrorMessage>Error message</Field.ErrorMessage>
  </Field.Root>
</VStack>
```

### Loading Spinner

Consistent loading indicator:

```tsx
<Box p={8}>
  <VStack gap={4}>
    <Spinner size="xl" color="blue.500" />
    <Text>Loading...</Text>
  </VStack>
</Box>
```

### Action Buttons

Standard button group for forms:

```tsx
<HStack gap={4}>
  <Button 
    colorPalette="blue" 
    type="submit"
    loading={isSaving}
  >
    Save
  </Button>
  <Button 
    variant="outline" 
    onClick={() => navigate(-1)}
  >
    Cancel
  </Button>
</HStack>
```

### Delete Confirmation Dialog

Pattern for destructive actions:

```tsx
<DialogRoot open={isDeleteOpen} onOpenChange={(e) => setIsDeleteOpen(e.open)}>
  <DialogContent>
    <DialogHeader>
      <DialogTitle>Confirm Delete</DialogTitle>
    </DialogHeader>
    <DialogBody>
      Are you sure you want to delete this item?
    </DialogBody>
    <DialogFooter>
      <Button variant="outline" onClick={() => setIsDeleteOpen(false)}>
        Cancel
      </Button>
      <Button colorPalette="red" onClick={handleDelete}>
        Delete
      </Button>
    </DialogFooter>
  </DialogContent>
</DialogRoot>
```

---

## Component Best Practices

### 1. Props Interface Definition

Always define props interfaces for reusable components:

```tsx
interface MyComponentProps {
  title: string;
  description?: string;
  onAction: () => void;
  children?: React.ReactNode;
}

const MyComponent: React.FC<MyComponentProps> = ({ 
  title, 
  description, 
  onAction,
  children 
}) => {
  // ...
};
```

### 2. Default Props

Use destructuring defaults for optional props:

```tsx
const MyComponent = ({ 
  isEditable = false,
  maxItems = 10 
}: MyComponentProps) => {
  // ...
};
```

### 3. Composition Pattern

Prefer composition over configuration:

```tsx
// Good - Composition
<Card>
  <CardHeader>
    <Title />
  </CardHeader>
  <CardBody>
    <Content />
  </CardBody>
</Card>

// Avoid - Over-configuration
<Card 
  title="..." 
  subtitle="..." 
  content={<Content />} 
  footer={<Footer />}
/>
```

### 4. Controlled vs Uncontrolled

Use controlled components for forms:

```tsx
// Controlled
<Input 
  value={formData.name}
  onChange={(e) => setFormData({...formData, name: e.target.value})}
/>
```

### 5. Error Boundaries

Consider wrapping complex components:

```tsx
<ErrorBoundary fallback={<ErrorDisplay />}>
  <ComplexComponent />
</ErrorBoundary>
```
