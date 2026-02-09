# RabbitMQ Event-Driven Architecture

This document describes the RabbitMQ implementation for event-driven cache invalidation in the Employee Management System.

## Overview

The EMS uses RabbitMQ to implement event-driven cache invalidation and activity persistence:
- **Backend API (Producer)**: Publishes domain events when entities are created, updated, or deleted
- **Activity Persistence**: `ActivityPersistingEventPublisher` decorator saves activities to the `RecentActivities` database table before publishing to RabbitMQ
- **Gateway (Consumer)**: Listens for events and invalidates the corresponding Redis cache entries

This decoupled architecture ensures that:
1. Cache is automatically invalidated when data changes
2. Activity history persists across server restarts (stored in database)
3. Backend doesn't need to know about Gateway's caching strategy
4. Multiple consumers can listen to the same events if needed

## Architecture

```
┌─────────────────────────────────────────────────────────────────────┐
│                      Backend API (Producer)                         │
│  PersonService → ActivityPersistingEventPublisher (Decorator)       │
│                    ├─ Save to RecentActivities table (DB)           │
│                    └─ RabbitMQEventPublisher → CloudEvent → Exchange│
└─────────────────────────────────────────────────────────────────────┘
                                    │
                                    ▼
┌─────────────────────────────────────────────────────────────────────┐
│                      RabbitMQ Broker                                │
│  Exchange: ems.events (topic) → Queue: ems.gateway.cache-invalidate │
└─────────────────────────────────────────────────────────────────────┘
                                    │
                                    ▼
┌─────────────────────────────────────────────────────────────────────┐
│                      Gateway (Consumer)                             │
│  RabbitMQEventConsumer → Parse CloudEvent → Invalidate Redis Cache  │
└─────────────────────────────────────────────────────────────────────┘
```

---

## Setup

### Prerequisites

- Docker (recommended) or RabbitMQ installed locally
- RabbitMQ Management Plugin enabled

### Option 1: Docker (Recommended)

```bash
# Start RabbitMQ with management UI
docker run -d --name rabbitmq \
  -p 5672:5672 \
  -p 15672:15672 \
  -e RABBITMQ_DEFAULT_USER=admin \
  -e RABBITMQ_DEFAULT_PASS=your-password \
  rabbitmq:management
```

### Option 2: Automated Setup Script

Run the provided PowerShell script to set up RabbitMQ infrastructure:

```powershell
cd server/scripts
.\setup-rabbitmq-queues.ps1
```

This script creates:
- Virtual host: `ems`
- Exchange: `ems.events` (topic, durable)
- Queue: `ems.gateway.cache-invalidation` (durable, TTL 24h, max 10,000 messages)
- Binding: `com.ems.#` routing key

### Option 3: Manual Setup via Management UI

1. Open RabbitMQ Management UI: http://localhost:15672
2. Login with your credentials

**Create Virtual Host:**
- Go to Admin → Virtual Hosts
- Add new vhost: `ems`

**Create Exchange:**
- Go to Exchanges (select vhost: `ems`)
- Add new exchange:
  - Name: `ems.events`
  - Type: `topic`
  - Durable: Yes

**Create Queue:**
- Go to Queues (select vhost: `ems`)
- Add new queue:
  - Name: `ems.gateway.cache-invalidation`
  - Durable: Yes
  - Arguments:
    - `x-message-ttl`: 86400000 (24 hours)
    - `x-max-length`: 10000

**Create Binding:**
- Click on the queue name
- Add binding:
  - From exchange: `ems.events`
  - Routing key: `com.ems.#`

---

## Configuration

### Backend API (`appsettings.json`)

```json
{
  "RabbitMQ": {
    "HostName": "localhost",
    "Port": 5672,
    "VirtualHost": "ems",
    "UserName": "emsadmin",
    "Password": "${RABBITMQ_PASSWORD}",
    "ExchangeName": "ems.events",
    "UseSsl": false,
    "RetryCount": 3,
    "RetryDelayMilliseconds": 1000,
    "Enabled": true
  }
}
```

### Gateway (`appsettings.json`)

```json
{
  "RabbitMQ": {
    "HostName": "localhost",
    "Port": 5672,
    "VirtualHost": "ems",
    "UserName": "emsadmin",
    "Password": "${RABBITMQ_PASSWORD}",
    "ExchangeName": "ems.events",
    "QueueName": "ems.gateway.cache-invalidation",
    "UseSsl": false,
    "RetryCount": 3,
    "RetryDelayMilliseconds": 1000,
    "Enabled": true
  }
}
```

### User Secrets (Development)

Store the RabbitMQ password securely:

```bash
# Backend API
cd server/EmployeeManagementSystem.Api
dotnet user-secrets set "RabbitMQ:Password" "your-secure-password"

# Gateway
cd gateway/EmployeeManagementSystem.Gateway
dotnet user-secrets set "RabbitMQ:Password" "your-secure-password"
```

---

## Event Types

### Routing Key Format

Events use the format: `com.ems.{entity}.{operation}`

| Entity | Created | Updated | Deleted |
|--------|---------|---------|---------|
| Person | `com.ems.person.created` | `com.ems.person.updated` | `com.ems.person.deleted` |
| School | `com.ems.school.created` | `com.ems.school.updated` | `com.ems.school.deleted` |
| Item | `com.ems.item.created` | `com.ems.item.updated` | `com.ems.item.deleted` |
| Position | `com.ems.position.created` | `com.ems.position.updated` | `com.ems.position.deleted` |
| SalaryGrade | `com.ems.salarygrade.created` | `com.ems.salarygrade.updated` | `com.ems.salarygrade.deleted` |
| Employee | `com.ems.employee.created` | `com.ems.employee.updated` | `com.ems.employee.deleted` |
| Blob | `com.ems.blob.uploaded` | - | `com.ems.blob.deleted` |

### CloudEvents Message Format

All events follow the [CloudEvents specification](https://cloudevents.io/):

```json
{
  "specversion": "1.0",
  "type": "com.ems.person.created",
  "source": "ems-backend-api",
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "time": "2026-02-08T10:30:00Z",
  "datacontenttype": "application/json",
  "data": {
    "entityType": "person",
    "entityId": "123456789012",
    "operation": "CREATE",
    "timestamp": "2026-02-08T10:30:00Z",
    "userId": "user@example.com",
    "correlationId": "abc-123-def",
    "payload": {
      "PersonId": 123456789012,
      "FirstName": "Juan",
      "LastName": "Santos",
      "MiddleName": "Cruz",
      "DateOfBirth": "1990-01-01",
      "Gender": "Male",
      "CivilStatus": "Single"
    },
    "metadata": {
      "ipAddress": "192.168.1.100",
      "userAgent": "Mozilla/5.0...",
      "source": "PersonService"
    }
  }
}
```

---

## Code Implementation

### Backend: Event Publishing with Activity Persistence

Events are published from services via the `IEventPublisher` interface. The `ActivityPersistingEventPublisher` decorator wraps the `RabbitMQEventPublisher` to persist activities to the database before publishing to RabbitMQ:

```csharp
// Registration in Program.cs (Decorator pattern)
services.AddSingleton<RabbitMQEventPublisher>();
services.AddScoped<IEventPublisher>(sp => new ActivityPersistingEventPublisher(
    sp.GetRequiredService<RabbitMQEventPublisher>(),    // Inner publisher
    sp.GetRequiredService<IRecentActivityRepository>(), // DB persistence
    sp.GetRequiredService<ILogger<ActivityPersistingEventPublisher>>()));
```

Events are published from services after successful operations. Example from `PersonService.cs`:

```csharp
public async Task<PersonResponseDto> CreatePersonAsync(CreatePersonDto dto)
{
    // ... create person logic ...
    
    await _repository.AddAsync(person);
    await _repository.SaveChangesAsync();
    
    // Publish domain event
    await _eventPublisher.PublishAsync(
        new PersonCreatedEvent(person.DisplayId, person.FirstName, person.LastName),
        userId: currentUserId,
        metadata: new EventMetadata
        {
            IpAddress = httpContext.GetClientIpAddress(),
            UserAgent = httpContext.Request.Headers.UserAgent,
            Source = nameof(PersonService)
        });
    
    return person.ToResponseDto();
}
```

### Gateway: Consuming Events

The `RabbitMQEventConsumer` processes events and invalidates cache:

```csharp
// RabbitMQEventConsumer.cs (simplified)
private async Task InvalidateCacheForEventAsync(string eventType, EventMessage eventData)
{
    string entityType = ExtractEntityType(eventType); // e.g., "person"
    
    switch (entityType.ToLowerInvariant())
    {
        case "person":
            await cacheService.RemoveByPrefixAsync(CacheKeys.PersonsListPrefix);
            await cacheService.RemoveByPrefixAsync(CacheKeys.EmploymentsListPrefix);
            break;
            
        case "school":
            await cacheService.RemoveByPrefixAsync(CacheKeys.SchoolsListPrefix);
            await cacheService.RemoveByPrefixAsync(CacheKeys.EmploymentsListPrefix);
            break;
            
        // ... other entities ...
    }
    
    // Always invalidate dashboard stats
    await cacheService.RemoveAsync(CacheKeys.DashboardStats);
}
```

---

## Cache Invalidation Strategy

| Event Type | Caches Invalidated |
|------------|-------------------|
| Person events | `persons:list:*`, `employments:list:*`, `dashboard:stats` |
| School events | `schools:list:*`, `employments:list:*`, `dashboard:stats` |
| Item events | `items:list:*`, `dashboard:stats` |
| Position events | `positions:list:*`, `employments:list:*`, `dashboard:stats` |
| SalaryGrade events | `salarygrades:list:*`, `employments:list:*`, `dashboard:stats` |
| Employment events | `employments:list:*`, `dashboard:stats` |
| Blob events | Depends on `relatedEntityType` in metadata |

---

## Monitoring

### RabbitMQ Management UI

Access at: http://localhost:15672

Key metrics to monitor:
- **Queue depth**: Number of pending messages
- **Message rate**: Publish/consume rates
- **Consumer count**: Active consumers

### Application Logs

When events are published, you'll see:
```
[Information] Published event com.ems.person.created with ID {guid}
```

When events are consumed:
```
╔═══════════════════════════════════════════════════════════════════════════════
║ 📨 RABBITMQ MESSAGE RECEIVED
╠═══════════════════════════════════════════════════════════════════════════════
║ Routing Key: com.ems.person.created
║ Timestamp:   2026-02-08 10:30:00.000 UTC
╠═══════════════════════════════════════════════════════════════════════════════
║ MESSAGE BODY:
╠═══════════════════════════════════════════════════════════════════════════════
║ { ... CloudEvent JSON ... }
╚═══════════════════════════════════════════════════════════════════════════════

✅ Cache invalidated for event: com.ems.person.created
```

### Seq Logs

Filter in Seq: `@Message like '%RabbitMQ%'`

---

## Troubleshooting

### Events Not Published

1. **Check if enabled**: Verify `RabbitMQ.Enabled: true` in Backend config
2. **Check connection**: Look for `RabbitMQ connection established successfully` in logs
3. **Check credentials**: Ensure user has permissions on the `ems` vhost

### Events Published but Not Consumed

1. **Check Gateway consumer**: Look for `RabbitMQ event consumer started successfully`
2. **Check queue binding**: Verify queue is bound to exchange with `com.ems.#` routing key
3. **Check vhost**: Both Backend and Gateway must use the same vhost (`ems`)

### Cache Not Invalidated

1. **Check Redis connection**: Gateway must be connected to Redis
2. **Check event processing logs**: Look for `✅ Processed event` messages
3. **Check entity type mapping**: Event type must match expected format

### Connection Issues

```bash
# Verify RabbitMQ is running
docker ps | grep rabbitmq

# Check connection via Management API
curl -u admin:password http://localhost:15672/api/overview

# List queues in ems vhost
docker exec rabbitmq rabbitmqctl list_queues -p ems name messages consumers
```

---

## Production Considerations

### High Availability

For production, consider:
- RabbitMQ cluster with mirrored queues
- Multiple Gateway instances (each gets messages)
- Dead letter queues for failed messages

### Security

- Use TLS/SSL for RabbitMQ connections (`UseSsl: true`)
- Use strong passwords stored in Azure Key Vault
- Limit vhost permissions to required operations

### Performance

- Monitor queue depth and consumer lag
- Adjust prefetch count if needed
- Consider message batching for high-volume scenarios

---

## Related Documentation

- [Quick Start Guide](./QUICK-START.md) - Getting started with the project
- [Gateway Architecture](./server/GATEWAY-STRUCTURE.md) - Gateway structure and components
- [Logging Guide](./server/LOGGING.md) - Serilog and Seq configuration
- [Deployment Guide](./DEPLOYMENT.md) - Production deployment instructions
