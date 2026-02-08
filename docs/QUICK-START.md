# Quick Start Guide

Get the Employee Management System running locally in minutes.

---

## Prerequisites Checklist

- [ ] .NET 10 SDK installed
- [ ] Node.js 18+ and npm installed
- [ ] Docker installed (for Redis, Seq, and RabbitMQ)
- [ ] SQL Server (LocalDB or SQL Server Express)
- [ ] Azure Storage Account (or Azurite emulator)
- [ ] Google OAuth2 Client ID

---

## Step 1: Clone and Setup

```bash
git clone <repository-url>
cd ems-v2
```

---

## Step 2: Start Infrastructure Services

### Start Redis (Caching)
```bash
docker run -d --name redis -p 6379:6379 redis
```

### Start Seq (Logging)
```bash
docker run -d --name seq -e ACCEPT_EULA=Y -p 5341:80 datalust/seq:latest
```

### Start RabbitMQ (Event Messaging)
```bash
docker run -d --name rabbitmq \
  -p 5672:5672 \
  -p 15672:15672 \
  -e RABBITMQ_DEFAULT_USER=admin \
  -e RABBITMQ_DEFAULT_PASS=your-password \
  rabbitmq:management
```

### Setup RabbitMQ Infrastructure
```powershell
cd server/scripts
.\setup-rabbitmq-queues.ps1
```

**Verify Services**:
- Redis: Running on port 6379
- Seq UI: http://localhost:5341
- RabbitMQ Management: http://localhost:15672

---

## Step 3: Configure Backend API

### Set User Secrets

```bash
cd server/EmployeeManagementSystem.Api

# Database connection
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=(localdb)\\mssqllocaldb;Database=EmsDb;Trusted_Connection=True;MultipleActiveResultSets=true"

# Azure Blob Storage (or use Azurite)
dotnet user-secrets set "ConnectionStrings:BlobStorage" "DefaultEndpointsProtocol=https;AccountName=your_account;AccountKey=your_key"

# JWT Secret
dotnet user-secrets set "Jwt:Secret" "your-super-secret-key-min-32-chars-long"

# Google OAuth
dotnet user-secrets set "Google:ClientId" "your-google-client-id.apps.googleusercontent.com"

# Seq API Key
dotnet user-secrets set "Serilog:WriteTo:1:Args:configure:0:Args:apiKey" "your-seq-api-key"

# RabbitMQ Password
dotnet user-secrets set "RabbitMQ:Password" "your-rabbitmq-password"
```

### Run Database Migrations

```bash
dotnet ef database update
```

### Seed Database (Optional)

Run the seed script in SQL Server Management Studio or via sqlcmd:

```bash
sqlcmd -S "(localdb)\mssqllocaldb" -d EmsDb -i "..\scripts\seed-data.sql"
```

### Start Backend API

```bash
dotnet run
```

**Verify**: API running at https://localhost:7166
- Swagger UI: https://localhost:7166/swagger

---

## Step 4: Configure Gateway

### Set User Secrets

```bash
cd ../../gateway/EmployeeManagementSystem.Gateway

# Seq API Key
dotnet user-secrets set "Serilog:WriteTo:1:Args:configure:0:Args:apiKey" "your-seq-api-key"

# RabbitMQ Password
dotnet user-secrets set "RabbitMQ:Password" "your-rabbitmq-password"
```

### Start Gateway

```bash
dotnet run
```

**Verify**: Gateway running at https://localhost:5003
- GraphQL Playground: https://localhost:5003/graphql

---

## Step 5: Configure Frontend

### Create Environment File

```bash
cd ../../application
cp .env.example .env
```

### Edit .env File

```env
VITE_GRAPHQL_URL=https://localhost:5003/graphql
VITE_GOOGLE_CLIENT_ID=your-google-client-id.apps.googleusercontent.com
```

### Install Dependencies

```bash
npm install
```

### Generate GraphQL Types

```bash
npm run codegen
```

### Start Frontend

```bash
npm run dev
```

**Verify**: Frontend running at http://localhost:5173

---

## Step 6: Verify Everything Works

### Check Services

| Service | URL | Expected |
|---------|-----|----------|
| Frontend | http://localhost:5173 | Login page loads |
| Gateway GraphQL | https://localhost:5003/graphql | GraphQL playground opens |
| Backend Swagger | https://localhost:7166/swagger | Swagger UI loads |
| Seq Logs | http://localhost:5341 | Seq UI shows recent logs |
| RabbitMQ | http://localhost:15672 | Management UI loads |
| Redis | localhost:6379 | Connection accepted |

### Test Login

1. Open http://localhost:5173
2. Click "Sign in with Google"
3. Complete OAuth flow
4. You should see the dashboard

### Check Logs in Seq

1. Open http://localhost:5341
2. You should see logs from:
   - `EmployeeManagementSystem.Api` (Backend)
   - `EmployeeManagementSystem.Gateway` (Gateway)
3. Try filtering: `Application = 'EmployeeManagementSystem.Gateway'`

---

## Common Issues and Fixes

### Backend won't start

**Issue**: Database connection failed
```bash
# Check LocalDB is running
sqllocaldb start mssqllocaldb

# Or update connection string to use SQL Server Express
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost\\SQLEXPRESS;Database=EmsDb;Trusted_Connection=True;"
```

### Gateway can't connect to Backend

**Issue**: SSL certificate validation failed
```bash
# Trust development certificate
dotnet dev-certs https --trust
```

### Frontend can't connect to Gateway

**Issue**: CORS error
- Check `appsettings.Development.json` in Gateway
- Ensure `http://localhost:5173` is in `Cors:AllowedOrigins`

### No logs in Seq

**Issue**: Seq not receiving logs
```bash
# Check Seq is running
docker ps | grep seq

# Restart Seq if needed
docker restart seq

# Check Seq API keys are set in user secrets
dotnet user-secrets list
```

### Redis connection failed

**Issue**: Redis not running
```bash
# Check Redis is running
docker ps | grep redis

# Start Redis if needed
docker start redis
```

### RabbitMQ connection failed

**Issue**: RabbitMQ not running or not configured
```bash
# Check RabbitMQ is running
docker ps | grep rabbitmq

# Start RabbitMQ if needed
docker start rabbitmq

# Re-run setup script if needed
cd server/scripts
.\setup-rabbitmq-queues.ps1
```

---

## Next Steps

Once everything is running:

1. **Explore the Application**
   - Create a person
   - Upload documents
   - Create employments
   - View dashboard statistics

2. **Check Logs in Seq**
   - Open http://localhost:5341
   - Watch logs in real-time
   - Try queries: `Level = 'Error'` or `@Message like '%person%'`

3. **Explore GraphQL**
   - Open https://localhost:5003/graphql
   - Try queries and mutations
   - Review auto-generated documentation

4. **Review Documentation**
   - [Architecture](./application/ARCHITECTURE.md)
   - [API Reference](./server/API-REFERENCE.md)
   - [Logging Guide](./server/LOGGING.md)

---

## Development Workflow

### Making Changes to GraphQL Schema

1. Update schema in Gateway (Query.cs or Mutation.cs)
2. Restart Gateway
3. Regenerate frontend types: `cd application && npm run codegen`

### Adding a New Service

1. Create interface in `Application/Interfaces/`
2. Implement in `Application/Services/`
3. Add logging via constructor injection: `ILogger<YourService> logger`
4. Register in `Program.cs`
5. Use message templates for all logs

### Database Migrations

```bash
cd server/EmployeeManagementSystem.Api

# Create migration
dotnet ef migrations add YourMigrationName

# Apply to database
dotnet ef database update

# Rollback if needed
dotnet ef database update PreviousMigrationName
```

---

## Stopping Everything

```bash
# Stop Docker containers
docker stop redis seq rabbitmq

# Stop running processes
# Ctrl+C in each terminal running dotnet/npm
```

---

## Getting Help

- [Documentation](../README.md)
- [Changelog](../CHANGELOG.md)
- [GitHub Issues](https://github.com/your-repo/issues)
