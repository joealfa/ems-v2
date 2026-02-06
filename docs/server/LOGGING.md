# Logging and Monitoring

This document describes the structured logging implementation using Serilog and Datalust Seq in the EMS backend and gateway.

---

## Overview

The Employee Management System uses **Serilog** for structured logging with **Datalust Seq** as the centralized logging platform. This provides:

- **Structured Logging**: Log events with typed properties instead of plain text
- **Centralized Monitoring**: All logs aggregated in Seq for easy querying
- **Performance**: Async sinks prevent logging from blocking application threads
- **Rich Context**: Automatic enrichment with machine name, thread ID, environment, user ID
- **Security**: No PII or sensitive data logged

---

## Technology Stack

| Component | Version | Purpose |
|-----------|---------|---------|
| **Serilog.AspNetCore** | 10.0.0 | Core Serilog integration for ASP.NET Core |
| **Serilog.Sinks.Console** | - | Console output with colored themes |
| **Serilog.Sinks.Seq** | 9.0.0 | Seq sink for centralized logging |
| **Serilog.Sinks.Async** | 2.1.0 | Async wrapper for non-blocking logging |
| **Serilog.Enrichers.Environment** | 3.0.1 | Machine name and environment enrichment |
| **Serilog.Enrichers.Thread** | 4.0.0 | Thread ID enrichment |
| **Serilog.Settings.Configuration** | 10.0.0 | Configuration from appsettings.json |

---

## Architecture

Both the **Backend API** and **GraphQL Gateway** implement the same logging architecture:

```
┌─────────────────────────────────────────────────────────────────┐
│                    Application Code                             │
│  Controllers → Services → Repositories                          │
└─────────────────────────────────────────────────────────────────┘
                │                    │
                ▼                    ▼
┌─────────────────────────────────────────────────────────────────┐
│                         Serilog                                 │
│  Message Templates + Structured Properties                      │
└─────────────────────────────────────────────────────────────────┘
                │                    │
       ┌────────┴────────┐          │
       ▼                 ▼          ▼
┌──────────┐    ┌──────────┐    ┌──────────┐
│ Console  │    │   Seq    │    │  Future  │
│  Sink    │    │  Sink    │    │  Sinks   │
└──────────┘    └──────────┘    └──────────┘
   (Local)      (Centralized)    (Optional)
```

---

## Configuration

### Backend API Configuration

**appsettings.json**:
```json
{
  "Serilog": {
    "Using": [ "Serilog.Sinks.Console", "Serilog.Sinks.Seq", "Serilog.Sinks.Async" ],
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.AspNetCore": "Warning",
        "Microsoft.EntityFrameworkCore": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "Async",
        "Args": {
          "configure": [
            {
              "Name": "Console",
              "Args": {
                "theme": "Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme::Code, Serilog.Sinks.Console",
                "outputTemplate": "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}"
              }
            }
          ]
        }
      },
      {
        "Name": "Async",
        "Args": {
          "configure": [
            {
              "Name": "Seq",
              "Args": {
                "serverUrl": "http://localhost:5341"
              }
            }
          ]
        }
      }
    ],
    "Enrich": [ "FromLogContext", "WithMachineName", "WithThreadId", "WithEnvironmentName" ],
    "Properties": {
      "Application": "EmployeeManagementSystem.Api"
    }
  }
}
```

**appsettings.Development.json**:
```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Debug",
      "Override": {
        "Microsoft": "Information",
        "Microsoft.AspNetCore": "Warning",
        "Microsoft.EntityFrameworkCore.Database.Command": "Information",
        "System": "Information"
      }
    }
  }
}
```

### Gateway Configuration

**appsettings.json** (identical structure):
```json
{
  "Serilog": {
    "Using": [ "Serilog.Sinks.Console", "Serilog.Sinks.Seq", "Serilog.Sinks.Async" ],
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.AspNetCore": "Warning",
        "HotChocolate": "Information",
        "HotChocolate.Execution": "Information",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "Async",
        "Args": {
          "configure": [
            {
              "Name": "Console",
              "Args": {
                "theme": "Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme::Code, Serilog.Sinks.Console",
                "outputTemplate": "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}"
              }
            }
          ]
        }
      },
      {
        "Name": "Async",
        "Args": {
          "configure": [
            {
              "Name": "Seq",
              "Args": {
                "serverUrl": "http://localhost:5341"
              }
            }
          ]
        }
      }
    ],
    "Enrich": [ "FromLogContext", "WithMachineName", "WithThreadId", "WithEnvironmentName" ],
    "Properties": {
      "Application": "EmployeeManagementSystem.Gateway"
    }
  }
}
```

### Sensitive Configuration (User Secrets)

Seq API keys are stored in **user secrets** for security:

**Backend API**:
```bash
dotnet user-secrets set "Serilog:WriteTo:1:Args:configure:0:Args:apiKey" "eNbAoVON73DB9XV3ygAO"
```

**Gateway**:
```bash
dotnet user-secrets set "Serilog:WriteTo:1:Args:configure:0:Args:apiKey" "0irSuiu7B4ZPuKkSTHMf"
```

---

## Program.cs Setup

Both Backend and Gateway use the same bootstrap pattern:

```csharp
using Serilog;
using Serilog.Events;

// Configure Serilog early to capture startup errors
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting up Employee Management System API");

try
{
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    // Add Serilog to the application
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .Enrich.WithThreadId()
        .Enrich.WithEnvironmentName());

    // ... rest of configuration ...

    WebApplication app = builder.Build();

    // Add Serilog request logging
    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
        options.GetLevel = (httpContext, elapsed, ex) => ex != null
            ? LogEventLevel.Error
            : httpContext.Response.StatusCode > 499
                ? LogEventLevel.Error
                : httpContext.Response.StatusCode > 399
                    ? LogEventLevel.Warning
                    : LogEventLevel.Information;

        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
            diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
            diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent.ToString());

            if (httpContext.User.Identity?.IsAuthenticated == true)
            {
                diagnosticContext.Set("UserId", httpContext.User.FindFirst("sub")?.Value);
            }
        };
    });

    Log.Information("Employee Management System API started successfully");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
```

---

## Logging Best Practices

### 1. Message Templates (NOT String Interpolation)

✅ **Correct** - Use message templates:
```csharp
logger.LogInformation("Creating person: {FirstName} {LastName}", firstName, lastName);
logger.LogWarning("Document upload failed for person {PersonDisplayId}: {StatusCode}", personId, statusCode);
```

❌ **Incorrect** - Don't use string interpolation:
```csharp
logger.LogInformation($"Creating person: {firstName} {lastName}"); // WRONG
logger.LogWarning($"Document upload failed for person {personId}: {statusCode}"); // WRONG
```

**Why?** Message templates enable structured logging, making logs queryable by properties.

### 2. Appropriate Log Levels

| Level | When to Use | Examples |
|-------|-------------|----------|
| **Debug** | Detailed flow information for debugging | Cache hits/misses, detailed API calls |
| **Information** | General application flow milestones | User login, entity created, operation completed |
| **Warning** | Abnormal/unexpected situations (recoverable) | Validation failures, retries, API errors |
| **Error** | Errors and exceptions requiring attention | Database errors, unhandled exceptions |
| **Fatal** | Application crashes | Startup failures, critical unrecoverable errors |

### 3. Structured Properties

Include relevant context as structured properties:

```csharp
logger.LogInformation(
    "Person created successfully: DisplayId {DisplayId}, Name: {FullName}",
    person.DisplayId,
    person.FullName);

logger.LogWarning(
    "API exception in GraphQL request: {StatusCode} - {Message}. Path: {Path}",
    statusCode,
    message,
    path);
```

### 4. Security - No PII or Sensitive Data

❌ **Never log**:
- Passwords, tokens, API keys
- Full credit card numbers, SSNs
- Personal identifiable information (except IDs)
- Authorization headers
- Raw request/response bodies with sensitive data

✅ **Safe to log**:
- User IDs (non-identifiable)
- Display IDs, entity IDs
- Operation names
- Status codes
- Error messages (sanitized)

---

## Logged Components

### Backend API

**Controllers**:
- `AuthController` - Login attempts, token operations, failures
- All other controllers inherit request logging from middleware

**Services**:
- `AuthService` - User creation, token validation, refresh, revocation
- `PersonService` - CRUD operations with person details
- `EmploymentService` - Employment operations with cascade deletion counts
- `DocumentService` - Document uploads, deletions, profile images
- `BlobStorageService` - Blob operations with sizes and status codes

### Gateway

**GraphQL Mutations** (`Mutation.cs`):
- **Person mutations**: CreatePerson, DeletePerson
- **Employment mutations**: CreateEmployment, DeleteEmployment
- **Document mutations**: UploadDocument, DeleteDocument, UploadProfileImage, DeleteProfileImage
- **Auth mutations**: GoogleLogin, GoogleTokenLogin, RefreshToken, Logout

**Components**:
- `RedisCacheService` - Cache operations (hit/miss, failures)
- `ApiExceptionErrorFilter` - API exceptions with status codes
- All DataLoaders - Entity fetching failures
- `ProfileImageController` - Profile image proxy operations

---

## Common Logging Patterns

### Service Operation Pattern

```csharp
public async Task<PersonResponseDto> CreateAsync(CreatePersonDto dto, string createdBy)
{
    _logger.LogInformation("Creating new person: {FirstName} {LastName} by user {CreatedBy}",
        dto.FirstName, dto.LastName, createdBy);

    Person person = new()
    {
        FirstName = dto.FirstName,
        LastName = dto.LastName,
        CreatedBy = createdBy
    };

    await _repository.AddAsync(person);

    _logger.LogInformation("Person created successfully: DisplayId {DisplayId}, Name: {FullName}",
        person.DisplayId, person.FullName);

    return person.ToResponseDto();
}
```

### Error Handling Pattern

```csharp
try
{
    string blobUrl = await _blobStorageService.UploadAsync(
        DocumentsContainer,
        blobName,
        dto.FileStream,
        dto.ContentType,
        cancellationToken);

    _logger.LogInformation("Blob uploaded successfully: {BlobName} in container {ContainerName}",
        blobName, DocumentsContainer);
}
catch (Exception ex)
{
    _logger.LogError(ex, "Failed to upload blob {BlobName} to container {ContainerName}",
        blobName, DocumentsContainer);
    throw;
}
```

### GraphQL Mutation Pattern

```csharp
public async Task<PersonResponseDto?> CreatePersonAsync(
    CreatePersonInput input,
    [Service] EmsApiClient client,
    [Service] IRedisCacheService cache,
    [Service] ILogger<Mutation> logger,
    CancellationToken ct)
{
    logger.LogInformation("Creating person via GraphQL: {FirstName} {LastName}",
        input.FirstName, input.LastName);

    PersonResponseDto result = await client.PersonsPOSTAsync(input.ToDto(), ct);
    await cache.RemoveByPrefixAsync(CacheKeys.PersonsListPrefix, ct);
    await cache.RemoveAsync(CacheKeys.DashboardStats, ct);

    logger.LogInformation("Person created successfully: DisplayId {DisplayId}, Name: {FullName}",
        result.DisplayId, result.FullName);

    return result;
}
```

---

## Seq Setup and Usage

### Installation (Development)

Run Seq in Docker:
```bash
docker run -d --name seq \
  -e ACCEPT_EULA=Y \
  -p 5341:80 \
  -v /path/to/seq/data:/data \
  datalust/seq:latest
```

Access Seq UI: `http://localhost:5341`

### Querying Logs in Seq

**Find all login attempts**:
```
@Message like '%login%'
```

**Find errors for a specific user**:
```
Level = 'Error' and UserId = 'abc123'
```

**Find slow requests (> 1 second)**:
```
Elapsed > 1000
```

**Find document upload failures**:
```
@Message like '%upload%' and Level = 'Warning'
```

**Group by status code**:
```
select StatusCode, count(*) from stream group by StatusCode
```

---

## Production Configuration

### Seq (Azure/Cloud)

For production, use Seq Cloud or self-hosted Seq with:
- **TLS/HTTPS** enabled
- **API key authentication** (already configured in user secrets)
- **Retention policies** to manage log storage
- **Alerts** for critical errors

Update `appsettings.Production.json`:
```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.AspNetCore": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "Async",
        "Args": {
          "configure": [
            {
              "Name": "Seq",
              "Args": {
                "serverUrl": "https://your-seq-instance.com",
                "restrictedToMinimumLevel": "Information"
              }
            }
          ]
        }
      }
    ]
  }
}
```

---

## Troubleshooting

### Logs not appearing in Seq

1. Check Seq is running: `docker ps` or visit `http://localhost:5341`
2. Verify Seq server URL in configuration
3. Check API key is set correctly in user secrets
4. Review application startup logs for Serilog errors

### Too many logs

Adjust log levels in `appsettings.json`:
```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Warning",  // Reduce verbosity
      "Override": {
        "YourNamespace": "Information"  // Keep specific namespace verbose
      }
    }
  }
}
```

### Performance impact

Serilog async sinks ensure minimal performance impact. If needed:
- Increase async buffer size
- Filter out high-frequency debug logs
- Use conditional logging

---

## Summary

The EMS logging infrastructure provides:

✅ **Structured Logging**: Queryable, machine-readable logs
✅ **Centralized Monitoring**: All logs in Seq for easy analysis
✅ **Rich Context**: Automatic enrichment with environment, user, request details
✅ **Performance**: Async sinks prevent blocking
✅ **Security**: No PII or sensitive data logged
✅ **Comprehensive Coverage**: Backend API + Gateway fully instrumented

For more information:
- [Serilog Documentation](https://serilog.net/)
- [Seq Documentation](https://docs.datalust.co/docs)
- [Serilog Best Practices](https://github.com/serilog/serilog/wiki/Best-Practices)
