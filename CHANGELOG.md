# Changelog

All notable changes to the Employee Management System project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

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
