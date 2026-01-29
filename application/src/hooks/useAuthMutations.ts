import { useMutation, useQuery } from '@apollo/client';
import {
  GoogleLoginDocument,
  GoogleTokenLoginDocument,
  RefreshTokenDocument,
  LogoutDocument,
  GetCurrentUserDocument,
} from '../graphql/generated/graphql';
import { resetApolloStore } from '../graphql/client';

/**
 * Hook for Google ID token login
 */
export function useGoogleLogin() {
  const [login, { data, loading, error }] = useMutation(GoogleLoginDocument);

  const handleLogin = async (idToken: string) => {
    const result = await login({ variables: { idToken } });
    return result.data?.googleLogin;
  };

  return {
    login: handleLogin,
    authResponse: data?.googleLogin,
    loading,
    error,
  };
}

/**
 * Hook for Google access token login
 */
export function useGoogleTokenLogin() {
  const [login, { data, loading, error }] = useMutation(
    GoogleTokenLoginDocument
  );

  const handleLogin = async (accessToken: string) => {
    const result = await login({ variables: { accessToken } });
    return result.data?.googleTokenLogin;
  };

  return {
    login: handleLogin,
    authResponse: data?.googleTokenLogin,
    loading,
    error,
  };
}

/**
 * Hook for refreshing authentication token
 */
export function useRefreshToken() {
  const [refresh, { data, loading, error }] = useMutation(RefreshTokenDocument);

  const handleRefresh = async (refreshToken?: string) => {
    const result = await refresh({ variables: { refreshToken } });
    return result.data?.refreshToken;
  };

  return {
    refreshToken: handleRefresh,
    authResponse: data?.refreshToken,
    loading,
    error,
  };
}

/**
 * Hook for logging out
 */
export function useLogout() {
  const [logout, { loading, error }] = useMutation(LogoutDocument);

  const handleLogout = async (refreshToken?: string) => {
    const result = await logout({ variables: { refreshToken } });

    // Clear Apollo cache after logout
    if (result.data?.logout) {
      await resetApolloStore();
    }

    return result.data?.logout ?? false;
  };

  return {
    logout: handleLogout,
    loading,
    error,
  };
}

/**
 * Hook for getting current authenticated user
 */
export function useCurrentUser() {
  const { data, loading, error, refetch } = useQuery(GetCurrentUserDocument, {
    fetchPolicy: 'cache-first',
  });

  return {
    user: data?.currentUser,
    loading,
    error,
    refetch,
  };
}
