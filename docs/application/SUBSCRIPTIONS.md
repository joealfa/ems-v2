# GraphQL Subscriptions (Real-time Updates)

## Overview

The EMS application uses **GraphQL subscriptions** over **WebSocket** for real-time activity updates. This provides instant notifications when data changes occur in the system.

## Architecture

### Flow Diagram

```
Backend → RabbitMQ → Gateway (Consumer) → GraphQL Subscription → Frontend (Subscriber)
   │                      │                       │                      │
   └─ Publish event       └─ Buffer + Broadcast   └─ WebSocket          └─ Update UI
```

### Components

**Backend (Producer)**:
- Publishes domain events to RabbitMQ after data mutations
- Events formatted as CloudEvents: `com.ems.{entity}.{operation}`

**Gateway (Consumer + Publisher)**:
- Consumes RabbitMQ events
- Adds events to `ActivityEventBuffer` (circular buffer, 50 events)
- Broadcasts events to GraphQL subscription clients via WebSocket
- Invalidates Redis cache

**Frontend (Subscriber)**:
- Connects to Gateway via WebSocket using `graphql-ws`
- Receives buffered history (last 50 events) on connect
- Receives live events in real-time
- Automatically reconnects on connection loss

---

## Frontend Usage

### 1. Hook: `useRecentActivities`

The primary way to subscribe to activity events:

```tsx
import { useRecentActivities } from '../hooks/useRecentActivities';
import { formatTimestamp, getActivityIcon } from '@/utils';

function Dashboard() {
  const { activities, isConnected, error } = useRecentActivities();

  if (error) {
    return <Alert status="error">{error.message}</Alert>;
  }

  return (
    <Box>
      {/* Connection Status */}
      {isConnected ? (
        <Badge colorScheme="green">Live</Badge>
      ) : (
        <Badge colorScheme="gray">Connecting...</Badge>
      )}

      {/* Activity List */}
      {activities.length === 0 ? (
        <Text color="gray.500">No recent activities</Text>
      ) : (
        activities.map(activity => (
          <Box key={activity.id} borderBottom="1px" borderColor="gray.200" pb={2} mb={2}>
            <Text fontWeight="medium">
              {getActivityIcon(activity.entityType)} {activity.message}
            </Text>
            <Text fontSize="xs" color="gray.500">
              {formatTimestamp(activity.timestamp)}
            </Text>
          </Box>
        ))
      )}
    </Box>
  );
}
```

### 2. Return Values

```typescript
interface UseRecentActivitiesReturn {
  activities: ActivityEvent[];  // Array of activity events (max 50)
  isConnected: boolean;          // WebSocket connection status
  error: Error | null;           // Connection or subscription error
}
```

### 3. Activity Event Structure

```typescript
interface ActivityEvent {
  id: string;                    // Unique event ID
  eventType: string;             // CloudEvent type (e.g., 'com.ems.person.created')
  entityType: string;            // Entity type (person, school, employment, etc.)
  entityId: string;              // Entity identifier
  operation: string;             // Operation (CREATE, UPDATE, DELETE, etc.)
  timestamp: string;             // ISO timestamp
  userId: string | null;         // User who performed the action
  message: string;               // User-friendly message
  metadata?: Array<{             // Additional metadata
    key: string;
    value: string;
  }> | null;
}
```

---

## WebSocket Configuration

### Connection Details

- **URL**: `wss://localhost:5003/graphql` (from `VITE_GRAPHQL_WS_URL`)
- **Protocol**: `graphql-ws` (not the older `subscriptions-transport-ws`)
- **Keep-alive**: 10 seconds (ping/pong)
- **Auto-retry**: 5 attempts with exponential backoff

### Client Configuration

Location: `src/graphql/subscription-client.ts`

```typescript
import { createClient, type Client } from 'graphql-ws';

const subscriptionClient = createClient({
  url: import.meta.env.VITE_GRAPHQL_WS_URL || 'wss://localhost:5003/graphql',
  connectionParams: () => {
    const token = localStorage.getItem('accessToken');
    return {
      authorization: token ? `Bearer ${token}` : '',
    };
  },
  retryAttempts: 5,
  shouldRetry: () => true,
  keepAlive: 10000,
  on: {
    connected: () => console.log('✅ GraphQL WebSocket connected'),
    closed: () => console.log('🔌 GraphQL WebSocket closed'),
    error: (error) => console.error('❌ GraphQL WebSocket error:', error),
  },
});
```

---

## Environment Variables

Add to `application/.env`:

```env
VITE_GRAPHQL_WS_URL=wss://localhost:5003/graphql
```

For production:
```env
VITE_GRAPHQL_WS_URL=wss://your-domain.com/graphql
```

---

## Event Types

| Entity | Operations | Example Event Type |
|--------|------------|-------------------|
| Person | CREATE, UPDATE, DELETE | `com.ems.person.created` |
| School | CREATE, UPDATE, DELETE | `com.ems.school.updated` |
| Employment | CREATE, UPDATE, DELETE | `com.ems.employee.created` |
| Item | CREATE, UPDATE, DELETE | `com.ems.item.deleted` |
| Position | CREATE, UPDATE, DELETE | `com.ems.position.updated` |
| Salary Grade | CREATE, UPDATE, DELETE | `com.ems.salarygrade.created` |
| Document | UPLOAD, DELETE | `com.ems.blob.uploaded` |

---

## Features

### 1. Buffered History

New subscribers receive the last 50 events immediately upon connection:

```typescript
// Gateway: ActivityEventBuffer.cs
public class ActivityEventBuffer
{
    private readonly ConcurrentQueue<ActivityEventDto> _events = new();
    private readonly int _maxCapacity = 50;

    public void AddEvent(ActivityEventDto activityEvent) { ... }
    public IReadOnlyList<ActivityEventDto> GetRecentEvents() { ... }
}
```

### 2. Automatic Reconnection

The subscription client automatically retries connection on failure:

```typescript
// Frontend handles reconnection transparently
const { activities, isConnected } = useRecentActivities();
// isConnected will be true when reconnected
```

### 3. Local Event Buffer

The frontend maintains a local buffer to prevent event loss during brief disconnections:

```typescript
// In useRecentActivities
const [activities, setActivities] = useState<ActivityEvent[]>([]);

// New events prepended, max 50 kept
setActivities(prev => [newEvent, ...prev].slice(0, 50));
```

---

## Troubleshooting

### Connection Issues

**Symptom**: `isConnected` is `false`, no events received

**Solutions**:
1. Check Gateway is running: `https://localhost:5003/graphql`
2. Verify `.env` has `VITE_GRAPHQL_WS_URL=wss://localhost:5003/graphql`
3. Check browser console for WebSocket errors
4. Ensure Gateway has `app.UseWebSockets()` in Program.cs
5. Verify firewall allows WebSocket connections

### Authentication Errors

**Symptom**: Connection opens but immediately closes

**Solutions**:
1. Check access token is valid: `localStorage.getItem('accessToken')`
2. Re-login to refresh token
3. Verify token is sent in `connectionParams`

### Missing Events

**Symptom**: Events published but not received

**Solutions**:
1. Check RabbitMQ consumer is running (Gateway logs)
2. Verify event type matches subscription filter
3. Check ActivityEventBuffer has events (Gateway endpoint or logs)
4. Confirm subscription is active (check browser DevTools Network tab)

### Performance Issues

**Symptom**: UI lags when many events arrive

**Solutions**:
1. Limit number of displayed events (already capped at 50)
2. Use virtualization for long lists (e.g., `react-window`)
3. Debounce UI updates if receiving rapid events
4. Consider filtering events client-side by entity type

---

## GraphQL Subscription Schema

```graphql
type Subscription {
  subscribeToActivityEvents: ActivityEventDto!
}

type ActivityEventDto {
  id: String!
  eventType: String!
  entityType: String!
  entityId: String!
  operation: String!
  timestamp: DateTime!
  userId: String
  message: String!
  metadata: [KeyValuePairOfStringAndString!]
}

type KeyValuePairOfStringAndString {
  key: String!
  value: String!
}
```

---

## Testing

### Manual Testing

1. Open Dashboard page (with activity feed)
2. Perform an action (e.g., create a person)
3. Observe real-time update in activity feed
4. Check "Live" badge shows connection status

### Browser DevTools

**Network Tab**:
- Filter: WS (WebSocket)
- Look for connection to `/graphql`
- Messages tab shows sent/received frames

**Console**:
```javascript
// Check subscription client
const { getSubscriptionClient } = await import('./graphql/subscription-client');
const client = getSubscriptionClient();
// Should show client status
```

---

## Best Practices

1. **Single Subscription per App**: Don't create multiple subscription clients
2. **Cleanup on Unmount**: Subscription is automatically cleaned up by `useRecentActivities` useEffect
3. **Error Handling**: Always display error state to users
4. **Connection Indicator**: Show connection status (Live/Connecting badges)
5. **Buffer Limits**: Frontend buffer capped at 50 events (matches Gateway buffer)
6. **Timestamp Formatting**: Use `formatTimestamp` utility for relative time display

---

## Related Documentation

- [GraphQL Usage Guide](./GRAPHQL_USAGE.md) - General GraphQL patterns
- [Architecture](./ARCHITECTURE.md) - Overall application architecture
- [Gateway Structure](../server/GATEWAY-STRUCTURE.md) - Gateway subscription implementation
- [RabbitMQ Events](../TESTING_RABBITMQ_EVENTS.md) - Event publishing and testing

---

## Dependencies

**Frontend**:
- `graphql-ws` - WebSocket client for GraphQL subscriptions
- `graphql` - GraphQL peer dependency

**Gateway**:
- `HotChocolate.Subscriptions` - GraphQL subscription support
- `HotChocolate.AspNetCore` - WebSocket handling

**Installation**:
```bash
# Frontend
cd application
npm install graphql-ws

# Gateway (already included in .csproj)
<PackageReference Include="HotChocolate.Subscriptions" Version="15.*" />
```
