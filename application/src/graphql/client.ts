import {
  ApolloClient,
  InMemoryCache,
  createHttpLink,
  from,
  type NormalizedCacheObject,
} from '@apollo/client';
import { setContext } from '@apollo/client/link/context';
import { onError } from '@apollo/client/link/error';

// GraphQL Gateway URL
const GRAPHQL_URL =
  import.meta.env.VITE_GRAPHQL_URL || 'https://localhost:5003/graphql';

// Create HTTP link
const httpLink = createHttpLink({
  uri: GRAPHQL_URL,
  credentials: 'include', // Include cookies for refresh token
});

// Auth link to add authorization header
const authLink = setContext((_, { headers }) => {
  // Get the authentication token from local storage
  const token = localStorage.getItem('accessToken');

  return {
    headers: {
      ...headers,
      authorization: token ? `Bearer ${token}` : '',
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

      // Handle authentication errors
      if (
        err.extensions?.code === 'UNAUTHENTICATED' ||
        err.message.includes('Unauthorized')
      ) {
        // Clear tokens and redirect to login
        localStorage.removeItem('accessToken');
        localStorage.removeItem('tokenExpiry');

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
    link: from([errorLink, authLink, httpLink]),
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
