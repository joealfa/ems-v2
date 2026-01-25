import {
  useState,
  useEffect,
  useCallback,
  useMemo,
  type ReactNode,
} from 'react';
import type { AuthResponseDto, UserDto } from '../api/generated/models';
import { AuthApi } from '../api/generated/api';
import { axiosInstance, API_BASE_URL } from '../api/config';
import { AuthContext, type AuthContextType } from './AuthContextType';

export { AuthContext, type AuthContextType } from './AuthContextType';

const ACCESS_TOKEN_KEY = 'accessToken';
const USER_KEY = 'user';
const TOKEN_EXPIRY_KEY = 'tokenExpiry';

export const AuthProvider = ({ children }: { children: ReactNode }) => {
  const [user, setUser] = useState<UserDto | null>(null);
  const [accessToken, setAccessToken] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  const authApi = useMemo(
    () => new AuthApi(undefined, API_BASE_URL, axiosInstance),
    []
  );

  const clearAuthData = useCallback(() => {
    localStorage.removeItem(ACCESS_TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
    localStorage.removeItem(TOKEN_EXPIRY_KEY);
    setAccessToken(null);
    setUser(null);
    // Refresh token is in HttpOnly cookie, will be cleared by server on logout
  }, []);

  const saveAuthData = useCallback((response: AuthResponseDto) => {
    if (response.accessToken) {
      localStorage.setItem(ACCESS_TOKEN_KEY, response.accessToken);
      setAccessToken(response.accessToken);
    }
    // refreshToken is set as HttpOnly cookie by the server
    if (response.expiresOn) {
      localStorage.setItem(TOKEN_EXPIRY_KEY, response.expiresOn);
    }
    if (response.user) {
      localStorage.setItem(USER_KEY, JSON.stringify(response.user));
      setUser(response.user);
    }
  }, []);

  const refreshToken = useCallback(async (): Promise<string | null> => {
    try {
      // Refresh token is sent automatically via HttpOnly cookie
      const response = await authApi.apiV1AuthRefreshPost({});
      saveAuthData(response.data);
      return response.data.accessToken || null;
    } catch {
      clearAuthData();
      return null;
    }
  }, [authApi, clearAuthData, saveAuthData]);

  const login = useCallback(
    async (googleIdToken: string) => {
      setIsLoading(true);
      try {
        const response = await authApi.apiV1AuthGooglePost({
          idToken: googleIdToken,
        });
        saveAuthData(response.data);
      } finally {
        setIsLoading(false);
      }
    },
    [authApi, saveAuthData]
  );

  const logout = useCallback(async () => {
    const storedAccessToken = localStorage.getItem(ACCESS_TOKEN_KEY);

    if (storedAccessToken) {
      try {
        // Refresh token is sent automatically via HttpOnly cookie
        await axiosInstance.post(
          '/api/v1/Auth/revoke',
          {}, // Empty body - refresh token is in cookie
          {
            headers: {
              Authorization: `Bearer ${storedAccessToken}`,
            },
          }
        );
      } catch {
        // Ignore errors during logout
      }
    }

    clearAuthData();
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
