import { useEffect, useState } from 'react';
import { getSubscriptionClient } from '../graphql/subscription-client';

export interface ActivityEvent {
  id: string;
  eventType: string;
  entityType: string;
  entityId: string;
  operation: string;
  timestamp: string;
  userId: string | null;
  message: string;
  metadata?: Array<{ key: string; value: string }> | null;
}

interface SubscriptionData {
  data?: {
    subscribeToActivityEvents?: ActivityEvent;
  };
}

interface UseRecentActivitiesReturn {
  activities: ActivityEvent[];
  isConnected: boolean;
  error: Error | null;
}

/**
 * Hook to subscribe to real-time activity events via GraphQL subscriptions.
 * Maintains a local buffer of the last 50 events.
 */
export const useRecentActivities = (): UseRecentActivitiesReturn => {
  const [activities, setActivities] = useState<ActivityEvent[]>([]);
  const [isConnected, setIsConnected] = useState(false);
  const [error, setError] = useState<Error | null>(null);

  useEffect(() => {
    const client = getSubscriptionClient();

    // Subscribe to activity events
    const unsubscribe = client.subscribe(
      {
        query: `
          subscription OnActivityEvent {
            subscribeToActivityEvents {
              id
              eventType
              entityType
              entityId
              operation
              timestamp
              userId
              message
              metadata {
                key
                value
              }
            }
          }
        `,
      },
      {
        next: (data: SubscriptionData) => {
          // Mark as connected on first successful data receive
          setIsConnected(true);

          if (data?.data?.subscribeToActivityEvents) {
            const event = data.data.subscribeToActivityEvents;

            // Add new event to the beginning, keep max 50
            setActivities((prev) => {
              const updated = [event, ...prev];
              return updated.slice(0, 50);
            });

            console.log('📨 Received activity event:', event.message);
          }
        },
        error: (err: Error | unknown) => {
          console.error('❌ Subscription error:', err);
          setError(err instanceof Error ? err : new Error(String(err)));
          setIsConnected(false);
        },
        complete: () => {
          console.log('✅ Subscription completed');
          setIsConnected(false);
        },
      }
    );

    // Cleanup on unmount
    return () => {
      unsubscribe();
    };
  }, []);

  return {
    activities,
    isConnected,
    error,
  };
};
