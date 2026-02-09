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

const USER_KEY = 'user';

export const AuthProvider = ({ children }: { children: ReactNode }) => {
  const [user, setUser] = useState<UserDto | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  const clearAuthData = useCallback(() => {
    localStorage.removeItem(USER_KEY);
    setUser(null);
  }, []);

  const saveUser = useCallback((userData: UserDto | null) => {
    if (userData) {
      localStorage.setItem(USER_KEY, JSON.stringify(userData));
      setUser(userData);
    }
  }, []);

  const refreshToken = useCallback(async (): Promise<boolean> => {
    try {
      // No refresh token variable needed — Gateway reads from HttpOnly cookie
      const data = await graphqlRequest<
        RefreshTokenMutation,
        RefreshTokenMutationVariables
      >(RefreshTokenDocument, {});

      if (data?.refreshToken?.user) {
        saveUser(data.refreshToken.user as UserDto);
        return true;
      }

      clearAuthData();
      return false;
    } catch {
      clearAuthData();
      return false;
    }
  }, [clearAuthData, saveUser]);

  const login = useCallback(
    async (googleIdToken: string) => {
      setIsLoading(true);
      try {
        const data = await graphqlRequest<
          GoogleLoginMutation,
          GoogleLoginMutationVariables
        >(GoogleLoginDocument, { idToken: googleIdToken });

        if (data?.googleLogin?.user) {
          saveUser(data.googleLogin.user as UserDto);
          setIsLoading(false);
          return; // Exit early - don't run validation logic
        }
      } catch (error) {
        setIsLoading(false);
        throw error; // Re-throw so LoginPage can handle it
      }
      setIsLoading(false);
    },
    [saveUser]
  );

  const logout = useCallback(async () => {
    try {
      // No refresh token variable needed — Gateway reads from HttpOnly cookie
      await graphqlRequest<LogoutMutation, LogoutMutationVariables>(
        LogoutDocument,
        {}
      );
    } catch {
      // Ignore errors during logout
    }

    clearAuthData();
    queryClient.clear();
  }, [clearAuthData]);

  // Initialize auth state: trust localStorage, don't aggressively validate
  useEffect(() => {
    const initializeAuth = async () => {
      const storedUser = localStorage.getItem(USER_KEY);

      if (storedUser) {
        try {
          const parsedUser = JSON.parse(storedUser);
          setUser(parsedUser);

          // Don't validate immediately - let natural API errors handle auth issues
          // This prevents race conditions after login where cookies aren't ready yet
        } catch {
          clearAuthData();
        }
      }
      setIsLoading(false);
    };

    initializeAuth();
  }, [clearAuthData]);

  // Keep in-memory auth state in sync if user key is cleared from storage
  useEffect(() => {
    const syncAuthStateFromStorage = () => {
      const storedUser = localStorage.getItem(USER_KEY);

      if (user && !storedUser) {
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
  }, [user, clearAuthData]);

  const value: AuthContextType = {
    user,
    isAuthenticated: !!user,
    isLoading,
    login,
    logout,
    refreshToken,
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};
