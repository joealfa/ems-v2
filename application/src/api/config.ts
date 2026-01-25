import axios, { type AxiosError, type InternalAxiosRequestConfig } from 'axios';
import { Configuration } from './generated';

// Base URL for the API
export const API_BASE_URL =
  import.meta.env.VITE_API_BASE_URL || 'http://localhost:5031';

// Create axios instance with default configuration
export const axiosInstance = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
  withCredentials: true, // Enable sending cookies with requests
});

// API configuration for generated clients
export const apiConfiguration = new Configuration({
  basePath: API_BASE_URL,
});

const ACCESS_TOKEN_KEY = 'accessToken';
const TOKEN_EXPIRY_KEY = 'tokenExpiry';

let isRefreshing = false;
let failedQueue: Array<{
  resolve: (token: string | null) => void;
  reject: (error: unknown) => void;
}> = [];

const processQueue = (error: unknown, token: string | null = null) => {
  failedQueue.forEach(prom => {
    if (error) {
      prom.reject(error);
    } else {
      prom.resolve(token);
    }
  });
  failedQueue = [];
};

// Request interceptor for adding auth tokens
axiosInstance.interceptors.request.use(
  config => {
    const token = localStorage.getItem(ACCESS_TOKEN_KEY);
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  error => {
    return Promise.reject(error);
  }
);

// Response interceptor for handling errors and token refresh
axiosInstance.interceptors.response.use(
  response => response,
  async (error: AxiosError) => {
    const originalRequest = error.config as InternalAxiosRequestConfig & {
      _retry?: boolean;
    };

    // Handle 401 Unauthorized
    if (error.response?.status === 401 && !originalRequest._retry) {
      // Skip refresh for auth endpoints
      if (
        originalRequest.url?.includes('/Auth/google') ||
        originalRequest.url?.includes('/Auth/refresh')
      ) {
        return Promise.reject(error);
      }

      if (isRefreshing) {
        // Queue requests while refreshing
        return new Promise((resolve, reject) => {
          failedQueue.push({ resolve, reject });
        })
          .then(token => {
            if (token) {
              originalRequest.headers.Authorization = `Bearer ${token}`;
            }
            return axiosInstance(originalRequest);
          })
          .catch(err => Promise.reject(err));
      }

      originalRequest._retry = true;
      isRefreshing = true;

      try {
        // Refresh token is sent automatically via HttpOnly cookie
        const response = await axios.post(
          `${API_BASE_URL}/api/v1/Auth/refresh`,
          {}, // Empty body - refresh token is in cookie
          { withCredentials: true } // Ensure cookies are sent
        );

        const { accessToken, expiresOn } = response.data;

        if (accessToken) {
          localStorage.setItem(ACCESS_TOKEN_KEY, accessToken);
        }
        if (expiresOn) {
          localStorage.setItem(TOKEN_EXPIRY_KEY, expiresOn);
        }

        processQueue(null, accessToken);
        originalRequest.headers.Authorization = `Bearer ${accessToken}`;
        return axiosInstance(originalRequest);
      } catch (refreshError) {
        processQueue(refreshError, null);
        clearAuthStorage();
        window.location.href = '/login';
        return Promise.reject(refreshError);
      } finally {
        isRefreshing = false;
      }
    }

    return Promise.reject(error);
  }
);

function clearAuthStorage() {
  localStorage.removeItem(ACCESS_TOKEN_KEY);
  localStorage.removeItem(TOKEN_EXPIRY_KEY);
  localStorage.removeItem('user');
  // Refresh token is in HttpOnly cookie, managed by the server
}
