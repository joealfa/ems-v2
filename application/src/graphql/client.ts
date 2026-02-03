import {
  ApolloClient,
  InMemoryCache,
  from,
  type NormalizedCacheObject,
  type ApolloLink,
} from '@apollo/client';
import { setContext } from '@apollo/client/link/context';
import { onError } from '@apollo/client/link/error';
// @ts-expect-error - apollo-upload-client v18 doesn't have proper type exports for this path
import createUploadLink from 'apollo-upload-client/createUploadLink.mjs';

// GraphQL Gateway URL
const GRAPHQL_URL =
  import.meta.env.VITE_GRAPHQL_URL || 'https://localhost:5003/graphql';

// Create Upload link (supports both regular requests and file uploads)
const uploadLink = createUploadLink({
  uri: GRAPHQL_URL,
  credentials: 'include', // Include cookies for refresh token
}) as unknown as ApolloLink;

// Auth link to add authorization header
const authLink = setContext((_, { headers }) => {
  // Get the authentication token from local storage
  const token = localStorage.getItem('accessToken');

  return {
    headers: {
      ...headers,
      ...(token ? { authorization: `Bearer ${token}` } : {}),
      // HotChocolate CSRF protection: required for multipart (file upload) requests.
      'GraphQL-Preflight': '1',
    },
  };
});

// Error handling link
const errorLink = onError(({ graphQLErrors, networkError }) => {
  if (graphQLErrors) {
    for (const err of graphQLErrors) {
      console.error(
        `[GraphQL error]: Message: ${err.message}, Location: ${err.locations}, Path: ${err.path}`
      );

      const missingToken = !localStorage.getItem('accessToken');
      const isAuthError =
        err.extensions?.code === 'UNAUTHENTICATED' ||
        err.extensions?.code === 'FORBIDDEN' ||
        err.message.includes('Unauthorized') ||
        err.message.includes('Forbidden');

      // Fallback: if local auth was cleared mid-session (devtools) but React state is still "logged in",
      // protected queries may fail. Redirect to login when we see a protected operation failing without a token.
      const pathRoot = Array.isArray(err.path) ? String(err.path[0]) : '';
      const looksProtected =
        missingToken &&
        (pathRoot === 'employments' ||
          pathRoot === 'persons' ||
          pathRoot === 'schools' ||
          pathRoot === 'positions' ||
          pathRoot === 'salaryGrades' ||
          pathRoot === 'items' ||
          pathRoot === 'person' ||
          pathRoot === 'employment' ||
          pathRoot === 'school');

      // Handle authentication errors
      if (isAuthError || looksProtected) {
        // Clear tokens and redirect to login
        localStorage.removeItem('accessToken');
        localStorage.removeItem('tokenExpiry');
        localStorage.removeItem('refreshToken');
        localStorage.removeItem('user');

        // Only redirect if not already on login page
        if (!window.location.pathname.includes('/login')) {
          window.location.href = '/login';
        }
      }
    }
  }

  if (networkError) {
    console.error(`[Network error]: ${networkError}`);
  }
});

// Create Apollo Client instance
export const apolloClient: ApolloClient<NormalizedCacheObject> =
  new ApolloClient({
    link: from([errorLink, authLink, uploadLink]),
    cache: new InMemoryCache({
      typePolicies: {
        Query: {
          fields: {
            // Configure pagination for list queries
            persons: {
              keyArgs: [
                'searchTerm',
                'fullNameFilter',
                'displayIdFilter',
                'gender',
                'civilStatus',
                'sortBy',
                'sortDescending',
              ],
            },
            employments: {
              keyArgs: [
                'searchTerm',
                'displayIdFilter',
                'employeeNameFilter',
                'positionFilter',
                'depEdIdFilter',
                'employmentStatus',
                'isActive',
                'sortBy',
                'sortDescending',
              ],
            },
            schools: {
              keyArgs: ['searchTerm', 'sortBy', 'sortDescending'],
            },
            positions: {
              keyArgs: ['searchTerm', 'sortBy', 'sortDescending'],
            },
            salaryGrades: {
              keyArgs: ['searchTerm', 'sortBy', 'sortDescending'],
            },
            items: {
              keyArgs: ['searchTerm', 'sortBy', 'sortDescending'],
            },
          },
        },
        // Use displayId as the cache key for entities
        PersonResponseDto: {
          keyFields: ['displayId'],
        },
        PersonListDto: {
          keyFields: ['displayId'],
        },
        EmploymentResponseDto: {
          keyFields: ['displayId'],
        },
        EmploymentListDto: {
          keyFields: ['displayId'],
        },
        SchoolResponseDto: {
          keyFields: ['displayId'],
        },
        SchoolListDto: {
          keyFields: ['displayId'],
        },
        PositionResponseDto: {
          keyFields: ['displayId'],
        },
        SalaryGradeResponseDto: {
          keyFields: ['displayId'],
        },
        ItemResponseDto: {
          keyFields: ['displayId'],
        },
      },
    }),
    defaultOptions: {
      watchQuery: {
        fetchPolicy: 'cache-and-network',
        errorPolicy: 'all',
      },
      query: {
        fetchPolicy: 'cache-first',
        errorPolicy: 'all',
      },
      mutate: {
        errorPolicy: 'all',
      },
    },
  });

// Helper to reset the store (useful for logout)
export const resetApolloStore = async (): Promise<void> => {
  await apolloClient.resetStore();
};

export default apolloClient;
