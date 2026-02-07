# Toast Notification System - Quick Reference

## ✅ What's Implemented

### 1. **Core Components**
- ✅ `useToast` hook - Custom hook for showing toasts
- ✅ `Toaster` component - Global toast renderer
- ✅ Theme-aware styling (auto-adapts to light/dark mode)
- ✅ Integrated with App.tsx

### 2. **Automatic Error Handling**
- ✅ Network errors from GraphQL automatically show error toasts
- ✅ Authentication errors show warning toast and redirect to login
- ✅ All unhandled errors display user-friendly messages

### 3. **Example Implementation**
- ✅ PersonFormPage now shows success toasts on create/update
- ✅ Example component with all toast types (ToastExample.tsx)

## 🚀 Quick Start

### Basic Usage

```typescript
import { useToast } from '../hooks';

const MyComponent = () => {
  const { showSuccess, showError, showInfo, showWarning } = useToast();

  // Success
  showSuccess('Saved!', 'Your changes have been saved.');

  // Error (errors are also auto-shown from GraphQL)
  showError('Failed', 'Unable to complete the operation.');

  // Info
  showInfo('Processing', 'Your request is being processed.');

  // Warning
  showWarning('Unsaved Changes', 'You have unsaved changes.');
};
```

### With Loading State

```typescript
const { showLoading, dismiss, showSuccess } = useToast();

const handleSave = async () => {
  const loadingToast = showLoading('Saving', 'Please wait...');

  try {
    await saveData();
    dismiss(loadingToast.id);
    showSuccess('Saved!', 'Data saved successfully.');
  } catch (error) {
    dismiss(loadingToast.id);
    // Error toast automatically shown by error handler
  }
};
```

## 📋 Available Methods

| Method | Description | Duration |
|--------|-------------|----------|
| `showSuccess(title, desc?)` | Green success toast | 40s |
| `showError(title, desc?)` | Red error toast | 50s |
| `showInfo(title, desc?)` | Blue info toast | 40s |
| `showWarning(title, desc?)` | Orange warning toast | 40s |
| `showLoading(title, desc?)` | Loading toast w/ spinner | ∞ (manual dismiss) |
| `dismiss(id)` | Dismiss specific toast | - |
| `dismissAll()` | Dismiss all toasts | - |

## 🎨 Theme Support

Toasts use Chakra UI's default color palette system:
- **Success**: Green color palette with checkmark icon
- **Error**: Red color palette with X icon
- **Info**: Blue color palette with info icon
- **Warning**: Orange color palette with warning icon
- **Loading**: Gray color palette with spinner
- **Position**: Top-right corner with proper spacing
- Automatically adapts to light/dark mode with appropriate contrast

## 📁 Files Created/Modified

### Created:
1. `src/hooks/useToast.ts` - Custom toast hook
2. `src/components/ui/Toaster.tsx` - Toast renderer component
3. `docs/application/TOAST-NOTIFICATIONS.md` - Full documentation
4. `src/components/examples/ToastExample.tsx` - Example usage

### Modified:
1. `src/graphql/error-handler.ts` - Added toast notifications
2. `src/components/ui/index.ts` - Export Toaster
3. `src/hooks/index.ts` - Export useToast
4. `src/App.tsx` - Added Toaster component
5. `src/pages/persons/PersonFormPage.tsx` - Example implementation

## 🔧 Configuration

Default settings (in `useToast.ts`):
- **Placement**: top-end (top-right corner)
- **Duration**: 40 seconds (50 seconds for errors)
- **Pause on Idle**: Yes
- **Gap**: 4 (spacing between multiple toasts)
- **Offsets**: 5rem from top (clears nav bar), 1rem from other edges

## 💡 Best Practices

1. **Short titles**: Keep titles to 1-3 words
2. **Descriptive messages**: Use descriptions for details
3. **Success feedback**: Show success on important actions
4. **Error clarity**: Explain what went wrong
5. **Loading states**: Use for operations > 2 seconds

## 📚 Full Documentation

See `docs/application/TOAST-NOTIFICATIONS.md` for:
- Detailed API reference
- Advanced usage patterns
- Integration examples
- Troubleshooting guide

## 🧪 Test It

1. Navigate to any person form
2. Create or update a person
3. See success toast appear in top-right corner with proper theme styling
4. Try triggering an error to see error toast
5. Test in both light and dark modes to verify theme adaptation

## Example: Current Implementation

```typescript
// In PersonFormPage.tsx
const { showSuccess } = useToast();

const handleSubmit = async () => {
  try {
    await updatePerson(data);
    showSuccess(
      'Person Updated',
      `${firstName} ${lastName} has been updated successfully.`
    );
    navigate('/persons');
  } catch (error) {
    // Error toast automatically shown by global error handler
  }
};
```

---

**Status**: ✅ Fully Implemented & Production Ready
