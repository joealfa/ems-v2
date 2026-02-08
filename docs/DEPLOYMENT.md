# Deployment Guide

## Table of Contents
1. [Prerequisites](#prerequisites)
2. [Environment Configuration](#environment-configuration)
3. [Database Setup](#database-setup)
4. [Backend API Deployment](#backend-api-deployment)
5. [Gateway Deployment](#gateway-deployment)
6. [Frontend Deployment](#frontend-deployment)
7. [Redis Setup](#redis-setup)
8. [Azure Blob Storage](#azure-blob-storage)
9. [RabbitMQ Setup](#rabbitmq-setup)
10. [Post-Deployment](#post-deployment)
11. [Monitoring & Logging](#monitoring--logging)
12. [Troubleshooting](#troubleshooting)

---

## Prerequisites

### Development Tools
- **.NET 10 SDK** - Required for building backend and gateway
- **Node.js 18+** - Required for frontend build
- **Redis** - For caching layer
- **RabbitMQ** - For event-driven cache invalidation
- **SQL Server** - For database (or Azure SQL Database)

### Azure Resources (Production)
- Azure App Service (for Backend API)
- Azure App Service (for Gateway)
- Azure Static Web Apps or App Service (for Frontend)
- Azure SQL Database
- Azure Cache for Redis
- Azure Blob Storage
- Azure Key Vault (recommended for secrets)

---

## Environment Configuration

### Backend API (`server/EmployeeManagementSystem.Api`)

**Development (`appsettings.Development.json`):**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=EmployeeManagementSystem;Trusted_Connection=true;TrustServerCertificate=true"
  },
  "Jwt": {
    "Key": "[FROM USER SECRETS]",
    "Issuer": "EmployeeManagementSystem",
    "Audience": "EmployeeManagementSystem",
    "ExpiryInMinutes": 15
  },
  "GoogleAuth": {
    "ClientId": "[FROM USER SECRETS]",
    "ClientSecret": "[FROM USER SECRETS]"
  },
  "AzureBlobStorage": {
    "ConnectionString": "[FROM USER SECRETS]",
    "ContainerName": "documents"
  },
  "RabbitMQ": {
    "HostName": "localhost",
    "Port": 5672,
    "UserName": "guest",
    "Password": "guest",
    "VirtualHost": "ems",
    "ExchangeName": "ems.events",
    "ExchangeType": "topic"
  },
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:5173",
      "https://localhost:5003"
    ]
  }
}
```

**Production (`appsettings.json`):**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "[FROM ENVIRONMENT VARIABLES]"
  },
  "Jwt": {
    "Key": "[FROM ENVIRONMENT VARIABLES]",
    "Issuer": "EmployeeManagementSystem",
    "Audience": "EmployeeManagementSystem",
    "ExpiryInMinutes": 15
  },
  "GoogleAuth": {
    "ClientId": "[FROM ENVIRONMENT VARIABLES]",
    "ClientSecret": "[FROM ENVIRONMENT VARIABLES]"
  },
  "AzureBlobStorage": {
    "ConnectionString": "[FROM ENVIRONMENT VARIABLES]",
    "ContainerName": "documents-prod"
  },
  "RabbitMQ": {
    "HostName": "[FROM ENVIRONMENT VARIABLES]",
    "Port": 5672,
    "UserName": "[FROM ENVIRONMENT VARIABLES]",
    "Password": "[FROM ENVIRONMENT VARIABLES]",
    "VirtualHost": "ems",
    "ExchangeName": "ems.events",
    "ExchangeType": "topic"
  },
  "Cors": {
    "AllowedOrigins": [
      "https://yourdomain.com"
    ]
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Warning"
    }
  }
}
```

### Gateway (`gateway/EmployeeManagementSystem.Gateway`)

**Development (`appsettings.Development.json`):**
```json
{
  "ApiClient": {
    "BaseUrl": "https://localhost:7166"
  },
  "Redis": {
    "ConnectionString": "localhost:6379",
    "InstanceName": "EmsGateway_Dev:"
  },
  "RabbitMQ": {
    "HostName": "localhost",
    "Port": 5672,
    "UserName": "guest",
    "Password": "guest",
    "VirtualHost": "ems",
    "ExchangeName": "ems.events",
    "QueueName": "ems.gateway.cache-invalidation"
  },
  "Caching": {
    "DefaultTtlMinutes": 1,
    "EntityTtlMinutes": 2,
    "ListTtlMinutes": 1
  },
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:5173"
    ]
  }
}
```

**Production (`appsettings.json`):**
```json
{
  "ApiClient": {
    "BaseUrl": "https://your-backend-api.azurewebsites.net"
  },
  "Redis": {
    "ConnectionString": "your-redis.redis.cache.windows.net:6380,password=your-password,ssl=True,abortConnect=False",
    "InstanceName": "EmsGateway_Prod:"
  },
  "RabbitMQ": {
    "HostName": "[FROM ENVIRONMENT VARIABLES]",
    "Port": 5672,
    "UserName": "[FROM ENVIRONMENT VARIABLES]",
    "Password": "[FROM ENVIRONMENT VARIABLES]",
    "VirtualHost": "ems",
    "ExchangeName": "ems.events",
    "QueueName": "ems.gateway.cache-invalidation"
  },
  "Caching": {
    "DefaultTtlMinutes": 5,
    "EntityTtlMinutes": 10,
    "ListTtlMinutes": 2
  },
  "Gateway": {
    "BaseUrl": "https://your-gateway.azurewebsites.net"
  },
  "Cors": {
    "AllowedOrigins": [
      "https://yourdomain.com"
    ]
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "HotChocolate": "Warning"
    }
  }
}
```

### Frontend (`application`)

**Development (`.env`):**
```env
DEV=true
VITE_API_BASE_URL=https://localhost:7166
VITE_GRAPHQL_URL=https://localhost:5003/graphql
VITE_GOOGLE_CLIENT_ID=your-dev-google-client-id
```

**Production (`.env.production`):**
```env
DEV=false
VITE_API_BASE_URL=https://your-backend-api.azurewebsites.net
VITE_GRAPHQL_URL=https://your-gateway.azurewebsites.net/graphql
VITE_GOOGLE_CLIENT_ID=your-prod-google-client-id
```

---

## Database Setup

### Local Development

1. **Install SQL Server** (Express or Developer edition)

2. **Create Database:**
   ```sql
   CREATE DATABASE EmployeeManagementSystem;
   ```

3. **Run Migrations:**
   ```bash
   cd server/EmployeeManagementSystem.Api
   dotnet ef database update
   ```

4. **Seed Initial Data** (automatic on first run in Development mode)

### Azure SQL Database

1. **Create Azure SQL Database:**
   ```bash
   az sql server create \
     --name ems-sql-server \
     --resource-group ems-rg \
     --location eastus \
     --admin-user sqladmin \
     --admin-password 'YourStrongPassword123!'

   az sql db create \
     --resource-group ems-rg \
     --server ems-sql-server \
     --name EmployeeManagementSystem \
     --service-objective S0
   ```

2. **Configure Firewall:**
   ```bash
   # Allow Azure services
   az sql server firewall-rule create \
     --resource-group ems-rg \
     --server ems-sql-server \
     --name AllowAzureServices \
     --start-ip-address 0.0.0.0 \
     --end-ip-address 0.0.0.0
   ```

3. **Get Connection String:**
   ```bash
   az sql db show-connection-string \
     --client ado.net \
     --name EmployeeManagementSystem \
     --server ems-sql-server
   ```

4. **Run Migrations from Local:**
   ```bash
   # Set connection string in environment
   export ConnectionStrings__DefaultConnection="Server=tcp:ems-sql-server.database.windows.net,1433;Database=EmployeeManagementSystem;User ID=sqladmin;Password=YourStrongPassword123!;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"

   cd server/EmployeeManagementSystem.Api
   dotnet ef database update
   ```

---

## Backend API Deployment

### Build for Production

```bash
cd server/EmployeeManagementSystem.Api
dotnet publish -c Release -o ./publish
```

### Azure App Service Deployment

1. **Create App Service:**
   ```bash
   az appservice plan create \
     --name ems-api-plan \
     --resource-group ems-rg \
     --sku B1 \
     --is-linux

   az webapp create \
     --resource-group ems-rg \
     --plan ems-api-plan \
     --name ems-backend-api \
     --runtime "DOTNETCORE:10.0"
   ```

2. **Configure Environment Variables:**
   ```bash
   az webapp config appsettings set \
     --resource-group ems-rg \
     --name ems-backend-api \
     --settings \
       ConnectionStrings__DefaultConnection="[Azure SQL Connection String]" \
       Jwt__Key="[Strong Random Key - min 256 bits]" \
       Jwt__Issuer="EmployeeManagementSystem" \
       Jwt__Audience="EmployeeManagementSystem" \
       GoogleAuth__ClientId="[Production Google Client ID]" \
       GoogleAuth__ClientSecret="[Production Google Client Secret]" \
       AzureBlobStorage__ConnectionString="[Azure Storage Connection String]" \
       AzureBlobStorage__ContainerName="documents-prod"
   ```

3. **Deploy from Local:**
   ```bash
   cd server/EmployeeManagementSystem.Api
   az webapp deployment source config-zip \
     --resource-group ems-rg \
     --name ems-backend-api \
     --src ./publish.zip
   ```

4. **Or Deploy with GitHub Actions:**
   ```yaml
   # .github/workflows/deploy-backend.yml
   name: Deploy Backend API

   on:
     push:
       branches: [main]
       paths:
         - 'server/**'

   jobs:
     build-and-deploy:
       runs-on: ubuntu-latest
       steps:
         - uses: actions/checkout@v3

         - name: Setup .NET
           uses: actions/setup-dotnet@v3
           with:
             dotnet-version: '10.0.x'

         - name: Build
           run: |
             cd server/EmployeeManagementSystem.Api
             dotnet publish -c Release -o ./publish

         - name: Deploy to Azure
           uses: azure/webapps-deploy@v2
           with:
             app-name: ems-backend-api
             publish-profile: ${{ secrets.AZURE_BACKEND_PUBLISH_PROFILE }}
             package: ./server/EmployeeManagementSystem.Api/publish
   ```

---

## Gateway Deployment

### Build for Production

```bash
cd gateway/EmployeeManagementSystem.Gateway
dotnet publish -c Release -o ./publish
```

### Azure App Service Deployment

1. **Create App Service:**
   ```bash
   az webapp create \
     --resource-group ems-rg \
     --plan ems-api-plan \
     --name ems-gateway \
     --runtime "DOTNETCORE:10.0"
   ```

2. **Configure Environment Variables:**
   ```bash
   az webapp config appsettings set \
     --resource-group ems-rg \
     --name ems-gateway \
     --settings \
       ApiClient__BaseUrl="https://ems-backend-api.azurewebsites.net" \
       Redis__ConnectionString="[Azure Redis Connection String]" \
       Redis__InstanceName="EmsGateway_Prod:" \
       Gateway__BaseUrl="https://ems-gateway.azurewebsites.net"
   ```

3. **Deploy:**
   ```bash
   cd gateway/EmployeeManagementSystem.Gateway
   az webapp deployment source config-zip \
     --resource-group ems-rg \
     --name ems-gateway \
     --src ./publish.zip
   ```

---

## Frontend Deployment

### Build for Production

```bash
cd application
npm install
npm run build
```

This creates a `dist/` folder with static files.

### Azure Static Web Apps

1. **Create Static Web App:**
   ```bash
   az staticwebapp create \
     --name ems-frontend \
     --resource-group ems-rg \
     --location eastus \
     --source ./ \
     --branch main \
     --app-location "/application" \
     --output-location "dist"
   ```

2. **Configure Environment Variables:**
   - In Azure Portal → Static Web App → Configuration
   - Add environment variables:
     - `VITE_API_BASE_URL`: `https://ems-backend-api.azurewebsites.net`
     - `VITE_GRAPHQL_URL`: `https://ems-gateway.azurewebsites.net/graphql`
     - `VITE_GOOGLE_CLIENT_ID`: `[Production Google Client ID]`

3. **GitHub Actions (Automatic):**
   Azure Static Web Apps automatically creates a GitHub Actions workflow.

### Alternative: Azure App Service (Node.js)

1. **Create App Service:**
   ```bash
   az webapp create \
     --resource-group ems-rg \
     --plan ems-api-plan \
     --name ems-frontend \
     --runtime "NODE:18-lts"
   ```

2. **Add `server.js` for serving static files:**
   ```javascript
   // application/server.js
   const express = require('express');
   const path = require('path');
   const app = express();

   app.use(express.static(path.join(__dirname, 'dist')));

   app.get('*', (req, res) => {
     res.sendFile(path.join(__dirname, 'dist', 'index.html'));
   });

   const port = process.env.PORT || 8080;
   app.listen(port, () => {
     console.log(`Server running on port ${port}`);
   });
   ```

3. **Update `package.json`:**
   ```json
   {
     "scripts": {
       "start": "node server.js",
       "build": "tsc && vite build"
     }
   }
   ```

4. **Deploy:**
   ```bash
   az webapp deployment source config-zip \
     --resource-group ems-rg \
     --name ems-frontend \
     --src ./dist.zip
   ```

---

## Redis Setup

### Local Development

**Using Docker:**
```bash
docker run -d --name redis -p 6379:6379 redis:latest
```

**Using Redis CLI:**
```bash
redis-cli
```

### Azure Cache for Redis

1. **Create Redis Cache:**
   ```bash
   az redis create \
     --resource-group ems-rg \
     --name ems-redis \
     --location eastus \
     --sku Basic \
     --vm-size c0 \
     --enable-non-ssl-port false
   ```

2. **Get Connection String:**
   ```bash
   az redis list-keys \
     --resource-group ems-rg \
     --name ems-redis \
     --query primaryKey \
     --output tsv
   ```

3. **Connection String Format:**
   ```
   ems-redis.redis.cache.windows.net:6380,password=YOUR_PRIMARY_KEY,ssl=True,abortConnect=False
   ```

4. **Configure in Gateway:**
   ```json
   {
     "Redis": {
       "ConnectionString": "ems-redis.redis.cache.windows.net:6380,password=YOUR_PRIMARY_KEY,ssl=True,abortConnect=False",
       "InstanceName": "EmsGateway_Prod:"
     }
   }
   ```

---

## Azure Blob Storage

### Create Storage Account

1. **Create Storage Account:**
   ```bash
   az storage account create \
     --name emsstorage \
     --resource-group ems-rg \
     --location eastus \
     --sku Standard_LRS
   ```

2. **Create Container:**
   ```bash
   az storage container create \
     --name documents-prod \
     --account-name emsstorage \
     --public-access off
   ```

3. **Get Connection String:**
   ```bash
   az storage account show-connection-string \
     --name emsstorage \
     --resource-group ems-rg \
     --query connectionString \
     --output tsv
   ```

4. **Configure in Backend API:**
   ```json
   {
     "AzureBlobStorage": {
       "ConnectionString": "DefaultEndpointsProtocol=https;AccountName=emsstorage;AccountKey=...",
       "ContainerName": "documents-prod"
     }
   }
   ```

---

## RabbitMQ Setup

The Employee Management System uses RabbitMQ for event-driven cache invalidation:
- **Backend API (Producer)**: Publishes domain events when entities are created, updated, or deleted
- **Gateway (Consumer)**: Listens for events and invalidates Redis cache accordingly

### Local Development

**Using Docker:**
```bash
docker run -d \
  --name rabbitmq \
  -p 5672:5672 \
  -p 15672:15672 \
  rabbitmq:management
```

**Access Management UI:**
- URL: http://localhost:15672
- Username: `guest`
- Password: `guest`

**Run Setup Script:**
```powershell
# Create vhost, exchange, queue, and bindings
cd server/scripts
.\setup-rabbitmq-queues.ps1
```

The setup script creates:
- **Virtual Host**: `ems`
- **Exchange**: `ems.events` (topic exchange, durable)
- **Queue**: `ems.gateway.cache-invalidation` (durable, 24h TTL, 10K max messages)
- **Binding**: Routes `com.ems.#` events to the queue

### Production Options

#### Option 1: CloudAMQP (Managed RabbitMQ)

1. **Create Account**: Sign up at [cloudamqp.com](https://www.cloudamqp.com/)

2. **Create Instance**: Choose region close to your Azure resources

3. **Get Connection String**: Format:
   ```
   amqps://user:password@host.cloudamqp.com/vhost
   ```

4. **Configure Both API and Gateway:**
   ```json
   {
     "RabbitMQ": {
       "HostName": "host.cloudamqp.com",
       "Port": 5671,
       "UserName": "your-username",
       "Password": "your-password",
       "VirtualHost": "your-vhost",
       "UseSsl": true
     }
   }
   ```

#### Option 2: Azure Service Bus (Alternative)

For production, you may consider Azure Service Bus as an alternative to RabbitMQ. However, this would require code changes to use the Azure Service Bus SDK.

**Create Service Bus:**
```bash
az servicebus namespace create \
  --name ems-servicebus \
  --resource-group ems-rg \
  --location eastus \
  --sku Standard

az servicebus topic create \
  --name ems-events \
  --namespace-name ems-servicebus \
  --resource-group ems-rg

az servicebus topic subscription create \
  --name gateway-cache-invalidation \
  --topic-name ems-events \
  --namespace-name ems-servicebus \
  --resource-group ems-rg
```

#### Option 3: Self-Hosted RabbitMQ on Azure VM

1. **Create VM:**
   ```bash
   az vm create \
     --resource-group ems-rg \
     --name ems-rabbitmq \
     --image Ubuntu2204 \
     --size Standard_B2s \
     --admin-username azureuser \
     --generate-ssh-keys
   ```

2. **Install RabbitMQ:**
   ```bash
   # SSH into VM
   ssh azureuser@<vm-ip>

   # Install RabbitMQ
   sudo apt-get update
   sudo apt-get install -y rabbitmq-server
   sudo rabbitmq-plugins enable rabbitmq_management
   sudo systemctl enable rabbitmq-server
   sudo systemctl start rabbitmq-server
   ```

3. **Configure Users:**
   ```bash
   sudo rabbitmqctl add_user ems_admin 'StrongPassword123!'
   sudo rabbitmqctl set_user_tags ems_admin administrator
   sudo rabbitmqctl add_vhost ems
   sudo rabbitmqctl set_permissions -p ems ems_admin ".*" ".*" ".*"
   sudo rabbitmqctl delete_user guest
   ```

4. **Configure SSL** (recommended for production):
   Follow [RabbitMQ TLS documentation](https://www.rabbitmq.com/ssl.html)

### Configuration

**Backend API (`appsettings.json`):**
```json
{
  "RabbitMQ": {
    "HostName": "your-rabbitmq-host",
    "Port": 5672,
    "UserName": "your-username",
    "Password": "[FROM ENVIRONMENT VARIABLES]",
    "VirtualHost": "ems",
    "ExchangeName": "ems.events",
    "ExchangeType": "topic"
  }
}
```

**Gateway (`appsettings.json`):**
```json
{
  "RabbitMQ": {
    "HostName": "your-rabbitmq-host",
    "Port": 5672,
    "UserName": "your-username",
    "Password": "[FROM ENVIRONMENT VARIABLES]",
    "VirtualHost": "ems",
    "ExchangeName": "ems.events",
    "QueueName": "ems.gateway.cache-invalidation"
  }
}
```

### Verify RabbitMQ Connection

**Check Queue Status:**
```bash
# Using RabbitMQ Management API
curl -u admin:password http://rabbitmq-host:15672/api/queues/ems/ems.gateway.cache-invalidation
```

**Test Event Publishing:**
1. Create or update an entity via the API
2. Check the queue in Management UI for pending messages
3. Verify Gateway logs show message consumption
4. Verify Redis cache was invalidated

### Troubleshooting RabbitMQ

| Issue | Solution |
|-------|----------|
| Connection refused | Verify firewall allows port 5672 (or 5671 for SSL) |
| Authentication failed | Check username/password and vhost permissions |
| Messages not consumed | Verify Gateway is running and queue binding exists |
| SSL handshake failed | Verify certificates and TLS version compatibility |

---

## Post-Deployment

### 1. Verify Services

**Backend API:**
```bash
curl https://ems-backend-api.azurewebsites.net/health
# Expected: {"status":"healthy","timestamp":"2026-02-05T..."}
```

**Gateway:**
```bash
curl https://ems-gateway.azurewebsites.net/health
# Expected: {"status":"healthy","timestamp":"2026-02-05T..."}
```

**GraphQL Gateway:**
```bash
curl -X POST https://ems-gateway.azurewebsites.net/graphql \
  -H "Content-Type: application/json" \
  -d '{"query":"{ __schema { queryType { name } } }"}'
```

### 2. Test Authentication

1. Open frontend: `https://ems-frontend.azurestaticapps.net`
2. Click "Login with Google"
3. Verify redirect to Google OAuth
4. Verify successful login and token storage

### 3. Database Migrations

If migrations were not run during deployment:
```bash
# Option 1: Run from local machine
export ConnectionStrings__DefaultConnection="[Azure SQL Connection String]"
cd server/EmployeeManagementSystem.Api
dotnet ef database update

# Option 2: Run from Azure Cloud Shell
dotnet ef database update --connection "[Azure SQL Connection String]"
```

### 4. Verify CORS

Test from browser console on frontend domain:
```javascript
fetch('https://ems-gateway.azurewebsites.net/graphql', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({ query: '{ __typename }' })
})
.then(res => res.json())
.then(console.log);
```

### 5. Monitor Logs

**Azure Portal:**
- Navigate to each App Service
- Select "Log stream" from left menu
- Watch for errors during first requests

**Azure CLI:**
```bash
az webapp log tail \
  --name ems-backend-api \
  --resource-group ems-rg
```

---

## Monitoring & Logging

### Application Insights

1. **Create Application Insights:**
   ```bash
   az monitor app-insights component create \
     --app ems-appinsights \
     --location eastus \
     --resource-group ems-rg
   ```

2. **Get Instrumentation Key:**
   ```bash
   az monitor app-insights component show \
     --app ems-appinsights \
     --resource-group ems-rg \
     --query instrumentationKey \
     --output tsv
   ```

3. **Configure in Backend:**
   ```bash
   az webapp config appsettings set \
     --resource-group ems-rg \
     --name ems-backend-api \
     --settings \
       APPLICATIONINSIGHTS_CONNECTION_STRING="InstrumentationKey=[KEY]"
   ```

4. **Add to `Program.cs`:**
   ```csharp
   builder.Services.AddApplicationInsightsTelemetry();
   ```

### Log Analytics

**Query logs in Azure Portal:**
- Navigate to Application Insights
- Click "Logs"
- Example queries:

```kusto
// Failed requests
requests
| where success == false
| order by timestamp desc

// Slow queries (> 1 second)
dependencies
| where duration > 1000
| order by timestamp desc

// Exception tracking
exceptions
| order by timestamp desc
```

### Health Checks

**Configure detailed health checks:**
```csharp
// Program.cs
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>()
    .AddRedis(builder.Configuration["Redis:ConnectionString"])
    .AddAzureBlobStorage(builder.Configuration["AzureBlobStorage:ConnectionString"]);

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
```

---

## Troubleshooting

### Issue: "Unable to connect to database"

**Cause:** Firewall rules or connection string issue

**Solution:**
1. Check Azure SQL firewall rules
2. Verify connection string format
3. Test connection from Azure Cloud Shell:
   ```bash
   sqlcmd -S ems-sql-server.database.windows.net -U sqladmin -P 'password' -d EmployeeManagementSystem -Q "SELECT 1"
   ```

### Issue: "CORS error when calling API"

**Cause:** Frontend domain not in allowed origins

**Solution:**
1. Update `appsettings.json`:
   ```json
   {
     "Cors": {
       "AllowedOrigins": ["https://your-actual-frontend-domain.com"]
     }
   }
   ```
2. Restart App Service

### Issue: "Redis connection timeout"

**Cause:** Redis not accessible or wrong connection string

**Solution:**
1. Verify Redis is running
2. Check connection string format (should include `ssl=True` for Azure)
3. Test connection:
   ```bash
   redis-cli -h ems-redis.redis.cache.windows.net -p 6380 -a YOUR_KEY --tls
   ```

### Issue: "401 Unauthorized on all requests"

**Cause:** JWT configuration mismatch

**Solution:**
1. Verify JWT settings match between Backend and environment variables
2. Check clock skew (servers should have synchronized time)
3. Validate token expiration is reasonable (15 minutes recommended)

### Issue: "File upload fails"

**Cause:** Azure Blob Storage misconfiguration

**Solution:**
1. Verify container exists and has correct permissions
2. Test connection string:
   ```bash
   az storage blob list \
     --account-name emsstorage \
     --container-name documents-prod \
     --connection-string "[CONNECTION_STRING]"
   ```

---

## Security Checklist Before Go-Live

- [ ] All secrets moved to environment variables (no hardcoded values)
- [ ] HTTPS enforced on all services
- [ ] CORS configured with exact production domain (no wildcards)
- [ ] JWT signing key is strong (min 256 bits)
- [ ] Refresh token rotation implemented
- [ ] Rate limiting enabled on auth endpoints
- [ ] SQL Server firewall rules restricted to Azure services only
- [ ] Redis AUTH enabled with strong password
- [ ] Azure Blob Storage has no public access
- [ ] Application Insights configured for monitoring
- [ ] Health check endpoints configured
- [ ] Database backups configured
- [ ] `.env` file not committed to repository
- [ ] Development authentication disabled in production

---

## Maintenance

### Database Backups

**Configure automated backups:**
```bash
az sql db ltr-policy set \
  --resource-group ems-rg \
  --server ems-sql-server \
  --database EmployeeManagementSystem \
  --weekly-retention P4W \
  --monthly-retention P12M \
  --yearly-retention P5Y \
  --week-of-year 1
```

### Scaling

**Scale App Service:**
```bash
# Scale up (more powerful instance)
az appservice plan update \
  --name ems-api-plan \
  --resource-group ems-rg \
  --sku P1V2

# Scale out (more instances)
az appservice plan update \
  --name ems-api-plan \
  --resource-group ems-rg \
  --number-of-workers 3
```

**Scale Redis:**
```bash
az redis update \
  --name ems-redis \
  --resource-group ems-rg \
  --sku Standard \
  --vm-size c1
```

### Updates

**Apply security patches:**
```bash
# Update all NuGet packages
cd server/EmployeeManagementSystem.Api
dotnet list package --outdated
dotnet add package [PackageName] --version [NewVersion]

# Update npm packages
cd application
npm outdated
npm update
```

---

## References

- [Azure App Service Documentation](https://learn.microsoft.com/en-us/azure/app-service/)
- [Azure SQL Database Documentation](https://learn.microsoft.com/en-us/azure/azure-sql/)
- [Azure Cache for Redis Documentation](https://learn.microsoft.com/en-us/azure/azure-cache-for-redis/)
- [Azure Static Web Apps Documentation](https://learn.microsoft.com/en-us/azure/static-web-apps/)
- [ASP.NET Core Deployment](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/)
- [RabbitMQ Documentation](https://www.rabbitmq.com/documentation.html)
- [CloudAMQP Documentation](https://www.cloudamqp.com/docs/)
- [Azure Service Bus Documentation](https://learn.microsoft.com/en-us/azure/service-bus-messaging/)
