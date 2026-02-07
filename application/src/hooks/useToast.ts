import { createToaster } from '@chakra-ui/react';

export const toaster = createToaster({
  placement: 'top-end' as const, // Top-right corner
  duration: 50000, // 50 seconds default - gives plenty of time to read
  pauseOnPageIdle: true,
  gap: 4,
  offsets: {
    top: '5rem', // 96px from top - clears navigation bar
    right: '1rem', // 16px from right
    bottom: '1rem', // 16px from bottom
    left: '1rem', // 16px from left
  },
});

export type ToastType = 'success' | 'error' | 'info' | 'warning' | 'loading';

interface ShowToastOptions {
  title: string;
  description?: string;
  type?: ToastType;
  duration?: number;
}

/**
 * Custom hook for displaying toast notifications.
 * Automatically follows the current theme (light/dark mode).
 */
export const useToast = () => {
  const showToast = ({
    title,
    description,
    type = 'info',
    duration,
  }: ShowToastOptions) => {
    toaster.create({
      title,
      description,
      type,
      duration,
    });
  };

  const showSuccess = (title: string, description?: string) => {
    showToast({ title, description, type: 'success' });
  };

  const showError = (title: string, description?: string) => {
    showToast({ title, description, type: 'error', duration: 60000 }); // 60 seconds for errors - even more time to read
  };

  const showInfo = (title: string, description?: string) => {
    showToast({ title, description, type: 'info' });
  };

  const showWarning = (title: string, description?: string) => {
    showToast({ title, description, type: 'warning' });
  };

  const showLoading = (title: string, description?: string) => {
    return toaster.create({
      title,
      description,
      type: 'loading',
      duration: Infinity, // Loading toasts don't auto-dismiss
    });
  };

  const dismiss = (id: string) => {
    toaster.remove(id);
  };

  const dismissAll = () => {
    toaster.dismiss();
  };

  return {
    showToast,
    showSuccess,
    showError,
    showInfo,
    showWarning,
    showLoading,
    dismiss,
    dismissAll,
  };
};
