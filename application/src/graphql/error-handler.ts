import { ClientError } from 'graphql-request';
import { queryClient } from './query-client';
import { toaster } from '../hooks/useToast';

const isAuthError = (error: unknown): boolean => {
  if (error instanceof ClientError) {
    return (
      error.response.errors?.some(
        (err) =>
          err.extensions?.code === 'UNAUTHENTICATED' ||
          err.extensions?.code === 'FORBIDDEN' ||
          err.message.includes('Unauthorized') ||
          err.message.includes('Forbidden') ||
          err.message.includes('Access denied')
      ) ?? false
    );
  }
  return false;
};

export const handleGlobalError = (error: unknown): void => {
  if (isAuthError(error)) {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('tokenExpiry');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('user');
    queryClient.clear();

    toaster.create({
      title: 'Session Expired',
      description: 'Your session has expired. Please login again.',
      type: 'warning',
      duration: 5000,
    });

    if (!window.location.pathname.includes('/login')) {
      window.location.href = '/login';
    }
  } else {
    // Show generic error toast for non-auth errors
    const errorMessage = getErrorMessage(error);
    toaster.create({
      title: 'Error',
      description: errorMessage,
      type: 'error',
      duration: 7000,
    });
  }
};

export const getErrorMessage = (error: unknown): string => {
  if (error instanceof ClientError) {
    const firstError = error.response.errors?.[0];
    if (firstError) {
      return firstError.message;
    }
  }
  if (error instanceof Error) {
    return error.message;
  }
  return 'An unexpected error occurred';
};
