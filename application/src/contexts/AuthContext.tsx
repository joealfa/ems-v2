import { useState, useEffect, useCallback, type ReactNode } from 'react';
import { graphqlRequest } from '../graphql/graphql-client';
import { queryClient } from '../graphql/query-client';
import {
  GoogleLoginDocument,
  RefreshTokenDocument,
  LogoutDocument,
  type GoogleLoginMutation,
  type GoogleLoginMutationVariables,
  type RefreshTokenMutation,
  type RefreshTokenMutationVariables,
  type LogoutMutation,
  type LogoutMutationVariables,
} from '../graphql/generated/graphql';
import {
  AuthContext,
  type AuthContextType,
  type UserDto,
} from './AuthContextType';

export {
  AuthContext,
  type AuthContextType,
  type UserDto,
} from './AuthContextType';

const ACCESS_TOKEN_KEY = 'accessToken';
const REFRESH_TOKEN_KEY = 'refreshToken';
const USER_KEY = 'user';
const TOKEN_EXPIRY_KEY = 'tokenExpiry';

export const AuthProvider = ({ children }: { children: ReactNode }) => {
  const [user, setUser] = useState<UserDto | null>(null);
  const [accessToken, setAccessToken] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  const clearAuthData = useCallback(() => {
    localStorage.removeItem(ACCESS_TOKEN_KEY);
    localStorage.removeItem(REFRESH_TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
    localStorage.removeItem(TOKEN_EXPIRY_KEY);
    setAccessToken(null);
    setUser(null);
  }, []);

  const saveAuthData = useCallback(
    (response: {
      accessToken?: string | null;
      refreshToken?: string | null;
      expiresOn?: string | null;
      user?: UserDto | null;
    }) => {
      if (response.accessToken) {
        localStorage.setItem(ACCESS_TOKEN_KEY, response.accessToken);
        setAccessToken(response.accessToken);
      }
      if (response.refreshToken) {
        localStorage.setItem(REFRESH_TOKEN_KEY, response.refreshToken);
      }
      if (response.expiresOn) {
        localStorage.setItem(TOKEN_EXPIRY_KEY, response.expiresOn);
      }
      if (response.user) {
        localStorage.setItem(USER_KEY, JSON.stringify(response.user));
        setUser(response.user);
      }
    },
    []
  );

  const refreshToken = useCallback(async (): Promise<string | null> => {
    try {
      const storedRefreshToken = localStorage.getItem(REFRESH_TOKEN_KEY);

      if (!storedRefreshToken) {
        clearAuthData();
        return null;
      }

      const data = await graphqlRequest<
        RefreshTokenMutation,
        RefreshTokenMutationVariables
      >(RefreshTokenDocument, { refreshToken: storedRefreshToken });

      if (data?.refreshToken) {
        saveAuthData(data.refreshToken);
        return data.refreshToken.accessToken || null;
      }

      clearAuthData();
      return null;
    } catch {
      clearAuthData();
      return null;
    }
  }, [clearAuthData, saveAuthData]);

  const login = useCallback(
    async (googleIdToken: string) => {
      setIsLoading(true);
      try {
        const data = await graphqlRequest<
          GoogleLoginMutation,
          GoogleLoginMutationVariables
        >(GoogleLoginDocument, { idToken: googleIdToken });

        if (data?.googleLogin) {
          saveAuthData(data.googleLogin);
        }
      } finally {
        setIsLoading(false);
      }
    },
    [saveAuthData]
  );

  const logout = useCallback(async () => {
    const storedRefreshToken = localStorage.getItem(REFRESH_TOKEN_KEY);

    if (storedRefreshToken) {
      try {
        await graphqlRequest<LogoutMutation, LogoutMutationVariables>(
          LogoutDocument,
          { refreshToken: storedRefreshToken }
        );
      } catch {
        // Ignore errors during logout
      }
    }

    clearAuthData();
    queryClient.clear();
  }, [clearAuthData]);

  // Initialize auth state from localStorage
  useEffect(() => {
    const initializeAuth = async () => {
      const storedToken = localStorage.getItem(ACCESS_TOKEN_KEY);
      const storedUser = localStorage.getItem(USER_KEY);
      const tokenExpiry = localStorage.getItem(TOKEN_EXPIRY_KEY);

      if (storedToken && storedUser) {
        // Check if token is expired
        if (tokenExpiry && new Date(tokenExpiry) <= new Date()) {
          // Token expired, try to refresh
          const newToken = await refreshToken();
          if (!newToken) {
            setIsLoading(false);
            return;
          }
        } else {
          setAccessToken(storedToken);
          try {
            setUser(JSON.parse(storedUser));
          } catch {
            clearAuthData();
          }
        }
      }
      setIsLoading(false);
    };

    initializeAuth();
  }, [clearAuthData, refreshToken]);

  // If storage is cleared while the app is running (e.g. DevTools), keep in-memory auth state in sync.
  useEffect(() => {
    const syncAuthStateFromStorage = () => {
      const storedToken = localStorage.getItem(ACCESS_TOKEN_KEY);
      const storedUser = localStorage.getItem(USER_KEY);

      // Only clear when we think we're authenticated but storage says otherwise.
      if ((accessToken && !storedToken) || (user && !storedUser)) {
        clearAuthData();
      }
    };

    window.addEventListener('storage', syncAuthStateFromStorage);
    window.addEventListener('focus', syncAuthStateFromStorage);
    const onVisibilityChange = () => {
      if (!document.hidden) {
        syncAuthStateFromStorage();
      }
    };
    document.addEventListener('visibilitychange', onVisibilityChange);

    const intervalId = window.setInterval(syncAuthStateFromStorage, 2000);

    return () => {
      window.removeEventListener('storage', syncAuthStateFromStorage);
      window.removeEventListener('focus', syncAuthStateFromStorage);
      document.removeEventListener('visibilitychange', onVisibilityChange);
      window.clearInterval(intervalId);
    };
  }, [accessToken, user, clearAuthData]);

  const value: AuthContextType = {
    user,
    accessToken,
    isAuthenticated: !!accessToken && !!user,
    isLoading,
    login,
    logout,
    refreshToken,
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};
