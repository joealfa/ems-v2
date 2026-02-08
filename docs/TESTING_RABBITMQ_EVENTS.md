# Testing RabbitMQ Event Publishing

## Quick Start - Manual Setup via RabbitMQ Management UI

### Option 1: Automated Setup (PowerShell Script)

Run the provided script:
```powershell
.\setup-rabbitmq-queues.ps1
```

Enter your RabbitMQ password when prompted. This will create:
- `ems.test.person-events` queue (captures all person events)
- `ems.test.all-events` queue (captures all EMS events)

### Option 2: Manual Setup via Management UI

1. **Open RabbitMQ Management UI**
   - URL: http://localhost:15672
   - Login with your credentials (user: `emsadmin`)

2. **Navigate to Queues Tab**
   - Click **"Queues"** in the top menu
   - Select virtual host: **"ems"** from dropdown

3. **Create a Test Queue**
   - Click **"Add a new queue"** section
   - Enter queue name: `ems.test.person-events`
   - Set **Durability**: Durable
   - **Arguments** (optional):
     - Add: `x-message-ttl` = `86400000` (24 hours)
     - Add: `x-max-length` = `10000`
   - Click **"Add queue"**

4. **Bind Queue to Exchange**
   - Click on the newly created queue name
   - Scroll down to **"Bindings"** section
   - Under **"Add binding to this queue"**:
     - **From exchange**: `ems.events`
     - **Routing key**: `com.ems.person.#` (captures all person events)
     - Click **"Bind"**

5. **Create Another Queue for All Events (Optional)**
   - Repeat steps 3-4 with:
     - Queue name: `ems.test.all-events`
     - Routing key: `com.ems.#` (captures ALL events)

---

## Testing Event Publishing

### Step 1: Verify API Configuration

Check `appsettings.Development.json`:
```json
{
  "RabbitMQ": {
    "HostName": "localhost",
    "Port": 5672,
    "VirtualHost": "ems",
    "UserName": "emsadmin",
    "Password": "your-password",
    "ExchangeName": "ems.events",
    "Enabled": true  // ← Make sure this is true!
  }
}
```

### Step 2: Start the API

```bash
cd server/EmployeeManagementSystem.Api
dotnet run
```

Look for this log entry:
```
[Information] RabbitMQ connection established successfully
```

### Step 3: Test with Swagger

1. Open Swagger UI: https://localhost:5001/swagger
2. Authorize with Google OAuth (click **Authorize** button)
3. Find **POST /api/v1/persons** endpoint
4. Click **"Try it out"**
5. Enter test data:

```json
{
  "firstName": "Test",
  "lastName": "User",
  "dateOfBirth": "1990-01-01",
  "gender": "Male",
  "civilStatus": "Single"
}
```

6. Click **Execute**
7. Note the `displayId` from the response

### Step 4: Update the Person

1. Find **PUT /api/v1/persons/{displayId}** endpoint
2. Use the displayId from step 3
3. Click **"Try it out"**
4. Update some fields:

```json
{
  "firstName": "Test Updated",
  "lastName": "User Updated",
  "dateOfBirth": "1990-01-01",
  "gender": "Male",
  "civilStatus": "Married"
}
```

5. Click **Execute**

### Step 5: Check RabbitMQ Management UI

1. Go back to RabbitMQ Management UI
2. Click **"Queues"** tab
3. You should see:
   - **ems.test.person-events** with message count (should show 2 messages: created + updated)
   - Message rate graph showing activity

4. Click on the queue name
5. Click **"Get messages"** section
6. Set **Messages**: 10
7. Click **"Get Message(s)"**

You should see CloudEvents formatted messages like:

```json
{
  "specversion": "1.0",
  "type": "com.ems.person.created",
  "source": "ems-backend-api",
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "time": "2026-02-07T10:30:00Z",
  "datacontenttype": "application/json",
  "data": {
    "entityType": "person",
    "entityId": "123456789012",
    "operation": "CREATE",
    "timestamp": "2026-02-07T10:30:00Z",
    "userId": "your-email@example.com",
    "correlationId": "...",
    "payload": {
      "PersonId": 123456789012,
      "FirstName": "Test",
      "LastName": "User",
      "MiddleName": null,
      "DateOfBirth": "1990-01-01",
      "Gender": "Male",
      "CivilStatus": "Single"
    },
    "metadata": {
      "ipAddress": "::1",
      "userAgent": "Mozilla/5.0...",
      "source": "PersonService"
    }
  }
}
```

---

## Troubleshooting

### No Messages in Queue

**Check 1: Is event publishing enabled?**
```json
"RabbitMQ": { "Enabled": true }
```

**Check 2: Check API logs**
```bash
# Look for these entries:
[Information] RabbitMQ connection established successfully
[Information] Published event com.ems.person.created with ID {guid}
[Debug] Published PersonCreatedEvent for DisplayId {id}
```

**Check 3: Verify queue binding**
- Go to Queues → Click your queue → Check "Bindings" section
- Should show: `ems.events` → `com.ems.person.#`

**Check 4: Check exchange**
- Go to Exchanges → Click `ems.events`
- Check "Message rates" - should show "publish" activity

**Check 5: Manual test - Publish from Exchange**
- Go to Exchanges → `ems.events`
- Scroll to "Publish message"
- Routing key: `com.ems.person.created`
- Payload: `{"test": "message"}`
- Click "Publish message"
- Check if message appears in queue

### Connection Refused

**Check RabbitMQ is running:**
```bash
docker ps | grep rabbitmq
```

**Check connection settings:**
- Hostname: `localhost`
- Port: `5672`
- VHost: `ems` (must exist)
- User has permissions on vhost

**Check logs for connection errors:**
```
[Error] Failed to initialize RabbitMQ connection
```

### Events Published but Not in Queue

**Possible causes:**
1. **Wrong routing key** - Check binding matches event type
   - Event: `com.ems.person.created`
   - Binding should be: `com.ems.person.#` or `com.ems.#`

2. **Wrong virtual host** - Ensure queue is in `ems` vhost

3. **Exchange doesn't exist** - Check Exchanges tab for `ems.events`

---

## Understanding Routing Keys

RabbitMQ uses routing keys to route messages to queues. The `#` wildcard matches zero or more words.

### Event Types Published

**Person events:**
- `com.ems.person.created`
- `com.ems.person.updated`
- `com.ems.person.deleted`

### Routing Key Patterns

| Pattern | Matches |
|---------|---------|
| `com.ems.person.#` | All person events |
| `com.ems.person.created` | Only person created events |
| `com.ems.person.*` | Person events (one word after person) |
| `com.ems.#` | All EMS events |
| `#` | Everything |

---

## Next Steps

Once you see events in the queue:

1. **Add more services** - Apply the same pattern to SchoolService, EmploymentService, etc.
2. **Phase 3: Blob Storage** - Add event publishing for file uploads
3. **Phase 4: Gateway Consumer** - Create consumer to invalidate cache when events arrive

---

## Useful RabbitMQ CLI Commands

**List queues:**
```bash
docker exec ems-rabbitmq rabbitmqctl list_queues -p ems name messages consumers
```

**List bindings:**
```bash
docker exec ems-rabbitmq rabbitmqctl list_bindings -p ems
```

**Purge queue:**
```bash
docker exec ems-rabbitmq rabbitmqctl purge_queue ems.test.person-events -p ems
```

**Delete queue:**
```bash
docker exec ems-rabbitmq rabbitmqctl delete_queue ems.test.person-events -p ems
```

---

## Success Criteria

✅ You should see:
1. API logs showing "Published event com.ems.person.updated"
2. Message count increases in RabbitMQ UI
3. Messages visible when clicking "Get messages"
4. CloudEvents-formatted JSON in message payload

🎉 If you see all of these, your RabbitMQ event publishing is working perfectly!
