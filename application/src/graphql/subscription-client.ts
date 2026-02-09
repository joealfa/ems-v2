import { createClient, type Client } from 'graphql-ws';

const GRAPHQL_WS_URL =
  import.meta.env.VITE_GRAPHQL_WS_URL || 'wss://localhost:5003/graphql';

let subscriptionClient: Client | null = null;

/**
 * Gets or creates the WebSocket subscription client.
 * The client is lazily initialized on first use.
 */
export const getSubscriptionClient = (): Client => {
  if (!subscriptionClient) {
    subscriptionClient = createClient({
      url: GRAPHQL_WS_URL,
      connectionParams: () => {
        // Auth is handled via HttpOnly cookies on the WebSocket upgrade request
        return {};
      },
      retryAttempts: 5,
      shouldRetry: () => true,
      keepAlive: 10000, // Send ping every 10 seconds
      on: {
        connected: () => {
          console.log('✅ GraphQL WebSocket connected');
        },
        closed: () => {
          console.log('🔌 GraphQL WebSocket closed');
        },
        error: (error) => {
          console.error('❌ GraphQL WebSocket error:', error);
        },
      },
    });
  }

  return subscriptionClient;
};

/**
 * Closes the WebSocket subscription client.
 * Should be called when the application is shutting down.
 */
export const closeSubscriptionClient = async (): Promise<void> => {
  if (subscriptionClient) {
    await subscriptionClient.dispose();
    subscriptionClient = null;
    console.log('🔌 GraphQL WebSocket client disposed');
  }
};
