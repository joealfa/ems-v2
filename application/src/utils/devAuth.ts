/**
 * Development-only authentication utilities.
 * These functions are only available when running in development mode.
 *
 * Calls the Gateway dev-auth endpoint which sets HttpOnly cookies.
 * No tokens are stored in localStorage.
 */

interface DevTokenRequest {
  userId?: string;
  email?: string;
  name?: string;
}

interface DevTokenResponse {
  userId: string;
  email: string;
  name: string;
  expiresAt: string;
}

const GATEWAY_BASE_URL =
  import.meta.env.VITE_GRAPHQL_URL?.replace('/graphql', '') ||
  'https://localhost:5003';

/**
 * Generates a development JWT token via the Gateway.
 * The token is set as an HttpOnly cookie by the Gateway — it is never
 * exposed to JavaScript.
 *
 * @param request Optional user information to include in the token
 * @returns User info from the dev token (token itself is in HttpOnly cookie)
 * @throws Error if not in development mode or if the request fails
 */
export const generateAndStoreDevToken = async (
  request?: DevTokenRequest
): Promise<DevTokenResponse> => {
  if (import.meta.env.PROD) {
    throw new Error(
      'Dev token generation is only available in development mode'
    );
  }

  try {
    const response = await fetch(`${GATEWAY_BASE_URL}/api/dev/auth/token`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      credentials: 'include',
      body: JSON.stringify(request || {}),
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}));
      throw new Error(
        `Failed to generate dev token: ${errorData.message || response.statusText}`
      );
    }

    const data: DevTokenResponse = await response.json();

    // Store a mock user object for display purposes (not a secret)
    const mockUser = {
      id: data.userId,
      email: data.email,
      firstName: data.name.split(' ')[0] || data.name,
      lastName: data.name.split(' ').slice(1).join(' ') || '',
      profilePictureUrl: null,
      role: 'Admin',
    };
    localStorage.setItem('user', JSON.stringify(mockUser));

    console.log('Development token set as HttpOnly cookie:', {
      email: data.email,
      expiresAt: data.expiresAt,
    });

    return data;
  } catch (error) {
    if (error instanceof Error) {
      throw new Error(`Failed to generate dev token: ${error.message}`);
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
    name: 'Dev User',
  });

  console.log('Dev token set as HttpOnly cookie! Refresh the page to use it.');
};

// Expose devLogin function to window in development mode for easy console access
if (import.meta.env.DEV) {
  interface WindowWithDevTools extends Window {
    devLogin: typeof quickDevLogin;
  }

  (window as unknown as WindowWithDevTools).devLogin = quickDevLogin;
  console.log(
    'Dev mode: Use window.devLogin() in console for quick authentication'
  );
}
