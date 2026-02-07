# Toast Notification System

A centralized, theme-aware toast notification system using Chakra UI that automatically handles network errors and provides a clean API for success, error, info, and warning messages.

## Features

- ✅ **Automatic Error Handling**: Network errors from GraphQL mutations and queries automatically show toast notifications
- ✅ **Theme Aware**: Automatically adapts to light/dark mode
- ✅ **Centralized**: Single source of truth for all notifications
- ✅ **Type Safe**: Full TypeScript support
- ✅ **Easy to Use**: Simple, intuitive API

## Setup

The toast system is already integrated into the app:

1. **Toaster Component**: Added to `App.tsx` - renders all toast notifications
2. **Error Handler**: Automatically catches GraphQL errors and displays toasts
3. **useToast Hook**: Available throughout the app for manual toast notifications

## Usage

### Basic Usage

```tsx
import { useToast } from '../hooks';

const MyComponent = () => {
  const { showSuccess, showError, showInfo, showWarning } = useToast();

  const handleSuccess = () => {
    showSuccess('Success!', 'Your action completed successfully.');
  };

  const handleError = () => {
    showError('Error!', 'Something went wrong.');
  };

  return (
    <div>
      <button onClick={handleSuccess}>Show Success</button>
      <button onClick={handleError}>Show Error</button>
    </div>
  );
};
```

### Available Methods

```typescript
const {
  showSuccess,  // Green toast for successful operations
  showError,    // Red toast for errors (7s duration)
  showInfo,     // Blue toast for informational messages
  showWarning,  // Orange toast for warnings
  showLoading,  // Loading toast (doesn't auto-dismiss)
  showToast,    // Generic toast with custom type
  dismiss,      // Dismiss a specific toast by ID
  dismissAll,   // Dismiss all toasts
} = useToast();
```

### Usage Examples

#### 1. Success Message

```tsx
const handleSave = async () => {
  try {
    await savePerson(data);
    showSuccess('Person Saved', 'The person record was saved successfully.');
  } catch (error) {
    // Error toast automatically shown by error handler
  }
};
```

#### 2. Error Message

```tsx
const handleDelete = async () => {
  try {
    await deletePerson(id);
    showSuccess('Deleted', 'Person deleted successfully.');
  } catch (error) {
    showError('Delete Failed', 'Unable to delete the person record.');
  }
};
```

#### 3. Info Message

```tsx
const handleRefresh = () => {
  refetch();
  showInfo('Refreshing', 'Fetching latest data...');
};
```

#### 4. Warning Message

```tsx
const handleWarning = () => {
  showWarning(
    'Unsaved Changes',
    'You have unsaved changes that will be lost.'
  );
};
```

#### 5. Loading Toast with Dismiss

```tsx
const handleUpload = async () => {
  const loadingToast = showLoading(
    'Uploading',
    'Please wait while we upload your file...'
  );

  try {
    await uploadFile(file);
    dismiss(loadingToast.id);
    showSuccess('Upload Complete', 'File uploaded successfully.');
  } catch (error) {
    dismiss(loadingToast.id);
    showError('Upload Failed', 'Unable to upload file.');
  }
};
```

#### 6. Custom Duration

```tsx
const { showToast } = useToast();

showToast({
  title: 'Custom Toast',
  description: 'This will display for 10 seconds',
  type: 'info',
  duration: 10000,
});
```

## Automatic Error Handling

The toast system automatically handles errors from GraphQL operations:

### Authentication Errors
When a request fails due to authentication (401/403), the system:
1. Shows a warning toast: "Session Expired"
2. Clears local storage
3. Redirects to login page

### Other Errors
All other GraphQL errors automatically show error toasts with the error message.

### Disabling Automatic Toasts

If you need to suppress the automatic error toast for a specific operation:

```tsx
import { queryClient } from '../graphql/query-client';

const { mutate } = useMutation({
  mutationFn: myMutation,
  onError: (error) => {
    // Handle error silently or with custom logic
    console.error('Silent error:', error);
  },
  meta: {
    // This prevents the global error handler from showing a toast
    suppressErrorToast: true,
  },
});
```

## Styling

The toast automatically uses Chakra UI's default color palette based on toast type:

- **Success**: Green color palette (check icon)
- **Error**: Red color palette (X icon)
- **Info**: Blue color palette (info icon)
- **Warning**: Orange color palette (warning icon)
- **Loading**: Gray color palette (spinner icon)
- **Shadow**: Subtle elevation with `shadow="lg"`
- **Positioning**: Top-right corner (`top-end`) with 5rem top offset to clear navigation bar
- **Theme Aware**: Automatically adapts to light/dark mode with appropriate contrast

## Configuration

Default settings in `useToast.ts`:

```typescript
{
  placement: 'top-end',        // Toast position: top-right corner
  duration: 40000,             // Default 40 seconds
  pauseOnPageIdle: true,       // Pause timer when tab inactive
  gap: 4,                      // Spacing between toasts
  offsets: {
    top: '5rem',               // 80px from top - clears navigation bar
    right: '1rem',             // 16px from right
    bottom: '1rem',            // 16px from bottom
    left: '1rem'               // 16px from left
  }
}
```

Error toasts have a longer duration (50000ms / 50 seconds) by default.

## API Reference

### showSuccess(title, description?)
Shows a success toast with green color palette and checkmark icon. Duration: 40 seconds.

### showError(title, description?)
Shows an error toast with red color palette and X icon. Duration: 50 seconds.

### showInfo(title, description?)
Shows an info toast with blue color palette and info icon. Duration: 40 seconds.

### showWarning(title, description?)
Shows an warning toast with orange color palette and warning icon. Duration: 40 seconds.

### showLoading(title, description?)
Shows a loading toast with gray color palette and spinner. Does not auto-dismiss.

### showToast(options)
Generic method for custom toast configurations.

Options:
- `title` (string, required)
- `description` (string, optional)
- `type` ('success' | 'error' | 'info' | 'warning' | 'loading')
- `duration` (number, optional) - in milliseconds

### dismiss(id)
Dismisses a specific toast by its ID.

### dismissAll()
Dismisses all active toasts.

## Best Practices

1. **Keep titles short**: 1-3 words
2. **Use descriptions for details**: Provide context in the description
3. **Don't overuse toasts**: Only for important user feedback
4. **Success messages**: Confirm important actions
5. **Error messages**: Explain what went wrong and possible solutions
6. **Loading states**: Use for operations taking > 2 seconds
