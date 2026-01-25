/**
 * Development-only authentication utilities.
 * These functions are only available when running in development mode.
 */

import axios from 'axios';

interface DevTokenRequest {
  userId?: string;
  email?: string;
  name?: string;
}

interface DevTokenResponse {
  token: string;
  expiresAt: string;
  userId: string;
  email: string;
  name: string;
}

/**
 * Generates a development JWT token without requiring OAuth authentication.
 * This function only works in development mode and when the backend is running in DEBUG mode.
 * 
 * @param request Optional user information to include in the token
 * @returns The generated JWT token
 * @throws Error if not in development mode or if the request fails
 */
export const getDevToken = async (request?: DevTokenRequest): Promise<string> => {
  if (import.meta.env.PROD) {
    throw new Error('Dev token generation is only available in development mode');
  }

  const apiUrl = import.meta.env.VITE_API_URL || 'https://localhost:7001';
  
  try {
    const response = await axios.post<DevTokenResponse>(
      `${apiUrl}/api/v1/dev/devauth/token`,
      request || {}
    );
    
    return response.data.token;
  } catch (error) {
    if (axios.isAxiosError(error)) {
      throw new Error(
        `Failed to generate dev token: ${error.response?.data?.message || error.message}`
      );
    }
    throw error;
  }
};

/**
 * Generates a development token and stores it in localStorage.
 * This is useful for quick testing without having to login via OAuth.
 * 
 * @param request Optional user information
 * @returns The full token response with expiry information
 */
export const generateAndStoreDevToken = async (
  request?: DevTokenRequest
): Promise<DevTokenResponse> => {
  if (import.meta.env.PROD) {
    throw new Error('Dev token generation is only available in development mode');
  }

  const apiUrl = import.meta.env.VITE_API_URL || 'https://localhost:7001';
  
  try {
    const response = await axios.post<DevTokenResponse>(
      `${apiUrl}/api/v1/dev/devauth/token`,
      request || {}
    );
    
    // Store the token in localStorage (same key as used by your auth system)
    localStorage.setItem('token', response.data.token);
    
    console.log('Development token generated and stored:', {
      email: response.data.email,
      expiresAt: response.data.expiresAt,
    });
    
    return response.data;
  } catch (error) {
    if (axios.isAxiosError(error)) {
      throw new Error(
        `Failed to generate dev token: ${error.response?.data?.message || error.message}`
      );
    }
    throw error;
  }
};

/**
 * Quick helper to generate a default dev token and store it.
 * Useful for browser console testing: `await window.devLogin()`
 */
export const quickDevLogin = async (): Promise<void> => {
  await generateAndStoreDevToken({
    userId: 'dev-user-123',
    email: 'dev@example.com',
    name: 'Dev User'
  });
  
  console.log('✅ Dev token generated! Refresh the page to use it.');
};

// Expose devLogin function to window in development mode for easy console access
if (import.meta.env.DEV) {
  (window as any).devLogin = quickDevLogin;
  (window as any).getDevToken = getDevToken;
  console.log('🔧 Dev mode: Use window.devLogin() in console for quick authentication');
}
