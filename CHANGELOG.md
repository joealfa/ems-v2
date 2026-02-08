# Changelog

All notable changes to the Employee Management System project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added - 2026-02-08

#### GraphQL Subscriptions (Real-time Updates)

- **WebSocket Support in Gateway**
  - Added `HotChocolate.Subscriptions` package (v15)
  - Enabled WebSocket middleware in Program.cs (`app.UseWebSockets()`)
  - `Subscription.cs` - GraphQL subscription type with `subscribeToActivityEvents`
  - `ActivityEventDto.cs` - DTO for activity events with message, metadata, and timestamp

- **Activity Event Buffer** - Server-side event history
  - `ActivityEventBuffer.cs` - Thread-safe circular buffer (50 events)
  - New subscribers receive buffered history immediately
  - Prevents event loss for clients connecting after events occur

- **RabbitMQ Event Broadcasting**
  - Enhanced `RabbitMQEventConsumer` to broadcast events to GraphQL subscriptions
  - Transforms CloudEvents to ActivityEventDto with user-friendly messages
  - Automatic retry with exponential backoff
  - Connection recovery on failure

- **Frontend Subscription Client**
  - Added `graphql-ws` package for WebSocket subscriptions
  - `subscription-client.ts` - WebSocket client with auto-reconnection
  - `useRecentActivities` hook - Custom hook for activity feed subscription
  - Maintains local buffer of last 50 events
  - Connection status tracking (`isConnected`)
  - Automatic reconnection (5 retries) on connection loss
  - 10-second keep-alive ping interval

- **Dashboard Real-time Activity Feed**
  - Updated Dashboard to show live activity updates via subscription
  - Connection status indicator (Live badge when connected)
  - Replaces dummy data with real-time events
  - Uses `formatTimestamp` and `getActivityIcon` utilities

- **Environment Variables**
  - Added `VITE_GRAPHQL_WS_URL` for WebSocket endpoint
  - Example: `wss://localhost:5003/graphql`

#### Utils Refactoring for Better Organization

- **Centralized Utils Exports** - `src/utils/index.ts`
  - Single entry point for all utility functions
  - Prevents circular dependencies
  - Improved IntelliSense discoverability
  - Consistent import paths across application

- **New Utilities Added**
  - `formatTimestamp` - Relative time formatting (e.g., "2 hours ago", "just now")
  - `getActivityIcon` - Returns emoji icons based on entity type

- **Utils Structure**
  - `formatters.ts` - Currency, address, file size, timestamp, enum label formatting
  - `helper.ts` - Document type colors, initials, activity icons
  - `mapper.ts` - Enum option arrays for forms
  - `devAuth.ts` - Development authentication utilities

- **Updated Imports Across Application**
  - All imports updated to use `from '@/utils'` or `from '../../utils'`
  - Affected files: Dashboard, DocumentsTable, PersonDocuments, ProfileImageUpload, PersonFormPage, EmploymentFormPage, etc.

#### Person Profile Image Feature

- **HasProfileImage Property**
  - Added `HasProfileImage` boolean to Person entity
  - Migrated to database with EF Core migration
  - Updated DTOs: `PersonResponseDto`, `PersonListDto`
  - Automatically set to `true` on profile image upload
  - Set to `false` on profile image deletion

#### RabbitMQ Event-Driven Architecture

- **Backend Event Publisher (Producer)** - Domain events published to RabbitMQ
  - `RabbitMQEventPublisher` in Infrastructure layer
  - CloudEvents message format (CNCF standard)
  - Events: person.created, person.updated, person.deleted, school.*, item.*, position.*, salarygrade.*, employee.*, blob.*
  - Polly retry policies with exponential backoff
  - Automatic connection recovery
  - SSL/TLS support

- **Gateway Event Consumer** - Cache invalidation via RabbitMQ
  - `RabbitMQEventConsumer` background service in Gateway
  - `RabbitMQBackgroundService` for lifecycle management
  - Automatic cache invalidation based on event type
  - Entity-specific cache invalidation (persons, schools, items, positions, salary grades, employments)
  - Dashboard stats cache automatically invalidated on any entity change

- **RabbitMQ Configuration**
  - Topic exchange: `ems.events`
  - Queue: `ems.gateway.cache-invalidation`
  - Routing key pattern: `com.ems.{entity}.{operation}`
  - Virtual host: `ems`
  - 24-hour message TTL
  - Max queue length: 10,000 messages

- **Setup Scripts**
  - `server/scripts/setup-rabbitmq-queues.ps1` - Automated RabbitMQ setup (vhost, exchange, queue, bindings)

#### SQL Scripts for Data Management

- **Database Creation Script** - `server/scripts/create-database.sql`
  - Creates EMS database with proper settings
  - Safe to run multiple times

- **Data Seed Script** - `server/scripts/seed-data.sql`
  - Generates 5,000 mock persons with related data
  - Includes schools, positions, salary grades, items
  - Replaces code-based DataSeeder for faster seeding
  - Transaction-wrapped with error handling
  - Safety check: only seeds empty databases

### Documentation Updates - 2026-02-08

- **Root README.md**
  - Updated technology stack to include `graphql-ws` and GraphQL Subscriptions
  - Updated features list with real-time activity feed and structured logging
  - Updated architecture description to include WebSocket subscriptions
  - Updated ASCII diagram to show WebSocket flow
  - Updated Mermaid diagram with Subscriptions, EventBuffer, and broadcast flow

- **Copilot Instructions**
  - Added GraphQL Subscriptions to Gateway and Frontend sections
  - Updated folder structure to include Services/ and subscription-client.ts
  - Added Frontend Architecture Guidelines section
  - Added utils organization documentation
  - Added subscription usage example
  - Added graphql-ws reference

- **GATEWAY-STRUCTURE.md**
  - Updated folder structure to include Services/ and Subscription.cs
  - Added GraphQL Subscriptions section with schema example
  - Updated Event Types table to include Subscription Broadcast column
  - Updated Benefits section to include real-time updates

- **GRAPHQL_USAGE.md**
  - Added Subscriptions section to Available Custom Hooks
  - Added comprehensive WebSocket Subscriptions section
  - Added Activity Event structure documentation
  - Added WebSocket configuration details
  - Updated file structure to include subscription files
  - Updated Best Practices section
  - Added troubleshooting for subscription issues

- **ARCHITECTURE.md (Application)**
  - Updated Centralized API Layer to mention graphql-ws
  - Added WebSocket Subscriptions section to State Management
  - Added Real-time Event Flow diagram
  - Added Utility Organization section
  - Updated Global State to mention subscription connection

- **SUBSCRIPTIONS.md (New)**
  - Comprehensive GraphQL subscriptions documentation
  - Architecture flow diagram
  - Frontend usage examples with `useRecentActivities`
  - WebSocket configuration details
  - Event types reference table
  - Features: buffered history, auto-reconnection, local buffer
  - Troubleshooting guide
  - Testing guidelines
  - Best practices

### Changed

- **Event Broadcast in Gateway**
  - RabbitMQ events now broadcast to GraphQL subscriptions in addition to cache invalidation
  - Activity messages are user-friendly (e.g., "Person 'John Doe' was created")
  - Metadata extracted from CloudEvent payload for better context

### Added - 2026-02-06

#### Toast Notification System

- **Centralized Toast Provider** - `src/contexts/ToastProvider.tsx`
  - Wraps Chakra UI's toast system
  - Centralized notification management
  - Consistent styling and duration
  
- **Custom Toast Hook** - `src/hooks/useToast.ts`
  - Simplified API: `showSuccess`, `showError`, `showInfo`, `showWarning`
  - Automatic error message extraction from Error objects
  - Position: top-right, Duration: 4 seconds, Dismissible

- **Integration Across Application**
  - Person CRUD operations
  - Employment CRUD operations
  - School CRUD operations
  - Document operations
  - Authentication operations

### Added - 2026-02-06

#### Logging and Monitoring Infrastructure

- **Serilog Integration** - Added comprehensive structured logging to Backend API and Gateway
  - Installed Serilog.AspNetCore 10.0.0
  - Installed Serilog.Sinks.Seq 9.0.0 for centralized logging
  - Installed Serilog.Sinks.Async 2.1.0 for non-blocking logging
  - Installed Serilog.Enrichers.Environment 3.0.1
  - Installed Serilog.Enrichers.Thread 4.0.0
  - Installed Serilog.Settings.Configuration 10.0.0

- **Seq Integration** - Centralized log aggregation platform
  - Seq UI accessible at `http://localhost:5341`
  - Separate API keys for Backend and Gateway
  - API keys stored securely in user secrets

- **Backend API Logging** - Added logging to critical services
  - `AuthController` - Login attempts, token operations, failures
  - `AuthService` - User creation, token validation, refresh, revocation
  - `PersonService` - Person CRUD operations with entity details
  - `EmploymentService` - Employment operations with cascade deletion counts
  - `DocumentService` - Document uploads, deletions, profile images
  - `BlobStorageService` - Blob operations with sizes and status codes

- **Gateway Logging** - Added logging to GraphQL operations
  - **Mutation.cs** - All create/delete operations with structured properties
    - Person mutations (CreatePerson, DeletePerson)
    - Employment mutations (CreateEmployment, DeleteEmployment)
    - Document mutations (UploadDocument, DeleteDocument, UploadProfileImage, DeleteProfileImage)
    - Auth mutations (GoogleLogin, GoogleTokenLogin, RefreshToken, Logout)
  - **RedisCacheService** - Cache operations (already had logging)
  - **ApiExceptionErrorFilter** - API exceptions with status codes and GraphQL paths
  - **DataLoaders** - Entity fetching failures (already had logging)
  - **ProfileImageController** - Profile image proxy operations (already had logging)

- **Request Logging** - HTTP request/response logging with enrichment
  - Message template: `HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms`
  - Automatic enrichment with RequestHost, RequestScheme, UserAgent, UserId
  - Conditional log levels based on status codes (4xx = Warning, 5xx = Error)

- **Bootstrap Logger** - Early startup logging to capture initialization errors
  - Logs written to console before full Serilog configuration loads
  - Ensures startup failures are captured

- **Configuration Files**
  - Updated `appsettings.json` with Serilog configuration for both Backend and Gateway
  - Updated `appsettings.Development.json` with Debug level logging
  - Added Seq API keys to user secrets (not committed to git)

#### Documentation

- **New Documentation** - Created comprehensive logging documentation
  - `docs/server/LOGGING.md` - Complete Serilog and Seq implementation guide
    - Configuration examples
    - Logging best practices
    - Message template usage
    - Security guidelines
    - Seq setup and querying
    - Production configuration
    - Troubleshooting

- **Updated Documentation** - Enhanced existing documentation with logging information
  - `README.md` - Added Serilog + Seq to technology stack, prerequisites, and setup
  - `server/README.md` - Added logging dependencies and configuration section
  - `docs/server/SERVICES.md` - Noted which services have logging
  - `docs/server/GATEWAY-STRUCTURE.md` - Added logging section and updated component descriptions

### Changed - 2026-02-06

- **Removed Legacy Logging** - Replaced default ASP.NET Core logging with Serilog
  - Removed `Logging` section from appsettings.json
  - Added `Serilog` section with Console and Seq sinks
  - Both sinks wrapped in async wrappers for performance

- **Enhanced Error Handling** - ApiExceptionErrorFilter now logs all API exceptions
  - Logs include status code, message, and GraphQL path
  - Warning level for all API exceptions

### Best Practices Followed

- ✅ **Message Templates** - All logs use message templates, not string interpolation
- ✅ **Structured Properties** - Log events include typed properties for querying
- ✅ **Appropriate Log Levels** - Debug, Information, Warning, Error, Fatal used correctly
- ✅ **No PII** - No personally identifiable information or sensitive data logged
- ✅ **Async Sinks** - Non-blocking logging for better performance
- ✅ **Contextual Enrichment** - Automatic enrichment with MachineName, ThreadId, EnvironmentName, UserId
- ✅ **Security** - Seq API keys stored in user secrets, not committed to git

---

## Previous Changes

(Historical changes before logging implementation would be documented here)
