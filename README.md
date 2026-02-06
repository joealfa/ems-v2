# Employee Management System

A comprehensive full-stack application for managing employees, persons, schools, and related records.

## Project Structure

```
ems-v2/
├── server/                                          # Backend API (ASP.NET Core with Clean Architecture)
│   ├── EmployeeManagementSystem.Domain/             # Entities and domain logic
│   ├── EmployeeManagementSystem.Application/        # Business logic and DTOs
│   ├── EmployeeManagementSystem.Infrastructure/     # Data access and external services
│   ├── EmployeeManagementSystem.Api/                # API controllers (v1, v2)
│   ├── EmployeeManagementSystem.ApiClient/          # NSwag-generated API client for Gateway
│   └── tests/                                       # Unit and integration tests
├── gateway/                                         # GraphQL Gateway (HotChocolate)
│   └── EmployeeManagementSystem.Gateway/            # GraphQL types, queries, mutations
│       ├── Types/                                   # Query.cs, Mutation.cs
│       ├── Controllers/                             # REST proxy for file operations
│       └── Caching/                                 # Redis caching
├── application/                                     # Frontend Application (React/TypeScript/Vite)
│   └── src/
│       ├── graphql/                                 # GraphQL operations and generated types
│       ├── components/                              # Reusable UI components
│       ├── contexts/                                # React context providers
│       ├── hooks/                                   # Custom React hooks
│       ├── pages/                                   # Page components
│       └── theme/                                   # Chakra-UI theme configuration
├── docs/                                            # Documentation
│   ├── server/                                      # Backend documentation
│   └── application/                                 # Frontend documentation
└── .github/                                         # GitHub configuration and copilot instructions
```

## Technology Stack

### Backend (.NET 10)
- **ASP.NET Core 10.0** - Web API framework
- **Clean Architecture** - Layered approach with strict separation of concerns
- **Entity Framework Core 10.0** - ORM for SQL database
- **Azure Blob Storage** - File storage for documents
- **JWT Authentication** - Secure API authentication with refresh token rotation
- **AspNetCoreRateLimit** - Rate limiting for API protection
- **Serilog + Seq** - Structured logging and centralized monitoring
- **Swagger/OpenAPI** - API documentation
- **xUnit** - Testing framework

### GraphQL Gateway (.NET 10)
- **HotChocolate 15** - GraphQL server for .NET
- **Redis** - Caching layer with hash-based key generation
- **Serilog + Seq** - Structured logging and centralized monitoring

### Frontend (React 19)
- **React** - UI framework
- **TypeScript** - Static typing
- **Vite** - Build tool
- **Chakra-UI** - Component library
- **AG Grid** - Data grid component
- **Apollo Client** - GraphQL client
- **GraphQL Code Generator** - Auto-generated types and hooks
- **React Router** - Client-side routing

## Features

- **Person Management** - Create, update, and manage person records
- **Employment Tracking** - Track employment history and status
- **Position Management** - Define and manage job positions
- **Salary Grades** - Configure salary grade structures
- **School Management** - Manage educational institutions
- **Document Storage** - Upload and manage documents with Azure Blob Storage
- **Item Inventory** - Track items and inventory
- **Reports** - Generate various reports
- **Authentication** - Secure login with JWT tokens and HttpOnly cookies for refresh tokens

## Getting Started

**📚 New to the project? See the [Quick Start Guide](docs/QUICK-START.md) for detailed step-by-step setup instructions.**

### Prerequisites
- .NET 10 SDK
- Node.js 18+ and npm
- SQL Server (LocalDB or SQL Server Express)
- Azure Storage Account (for blob storage)
- Docker (for Redis and Seq) or Redis/Seq installed locally

### Backend Setup

```bash
cd server
dotnet restore
dotnet build
cd EmployeeManagementSystem.Api
dotnet run
```

The API will be available at `https://localhost:7166` with Swagger UI at `https://localhost:7166/swagger`

### Gateway Setup

First, ensure Redis and Seq are running:
```bash
# Redis for caching
docker run -d --name redis -p 6379:6379 redis

# Seq for centralized logging
docker run -d --name seq -e ACCEPT_EULA=Y -p 5341:80 datalust/seq:latest
```

Then start the Gateway:
```bash
cd gateway/EmployeeManagementSystem.Gateway
dotnet run
```

The GraphQL Gateway will be available at `https://localhost:5003/graphql`

Access Seq UI for logs: `http://localhost:5341`

### Frontend Setup

Create a `.env` file from the template:
```bash
cd application
cp .env.example .env
# Edit .env with your Google OAuth Client ID
```

Install dependencies and start:
```bash
npm install
npm run dev
```

The application will be available at `http://localhost:5173`

### Generate GraphQL Types

After changes to the GraphQL schema, regenerate the frontend types:

```bash
cd application
npm run codegen
```

## Architecture

The application uses a **GraphQL Gateway** pattern:
- **Frontend** communicates with the **GraphQL Gateway** (HotChocolate) for most operations
- **Frontend** uses Gateway **REST endpoints** for file upload/download operations
- **Gateway** uses the **NSwag-generated API client** to communicate with the Backend
- **Backend** handles business logic, data persistence, and file storage

```
┌─────────────────────────────────────────────────────────────────────┐
│                         Frontend (React)                            │
│                  Apollo Client + REST fetch calls                   │
└─────────────────────────────────────────────────────────────────────┘
                │                                  │
                │ GraphQL                          │ REST (files)
                ▼                                  ▼
┌──────────────────────────────────────────────────────────────────────┐
│                     Gateway (HotChocolate)                           │
│  ┌───────────────────────────┐    ┌───────────────────────────────┐  │
│  │  GraphQL (Query/Mutation) │    │  REST Controllers (Documents) │  │
│  └───────────────────────────┘    └───────────────────────────────┘  │
│               │ Uses NSwag ApiClient             │                   │
└──────────────────────────────────────────────────────────────────────┘
                │                                  │
                ▼                                  ▼
┌─────────────────────────────────────────────────────────────────────┐
│                      Backend API (ASP.NET Core)                     │
│           Controllers → Services → EF Core → SQL Database           │
│                                 └──→ Azure Blob Storage             │
└─────────────────────────────────────────────────────────────────────┘
```

## API Standards

### REST API (Backend)
- **Lowercase URLs** - All API routes use lowercase (e.g., `/api/v1/persons`, `/api/v1/salarygrades`)
- **Versioning** - URL-based versioning (currently v1)
- **Secure Authentication** - JWT access tokens (15 min) with HttpOnly cookies for refresh tokens (7 days)
- **Token Rotation** - Automatic refresh token rotation with reuse detection
- **Rate Limiting** - Configurable rate limits (5 auth requests/min in production)
- **OpenAPI/Swagger** - Interactive API documentation at `/swagger`

### GraphQL (Gateway)
- **Type-safe** - Strongly typed schema
- **Single endpoint** - All queries/mutations via `/graphql`
- **Redis Caching** - Hash-based key generation for accurate cache invalidation
- **DataLoaders** - Prevents N+1 query problems

## Project Guidelines

See [.github/copilot-instructions.md](.github/copilot-instructions.md) for detailed coding standards and guidelines.

## Documentation

### Quick Links
- [**Changelog**](CHANGELOG.md) - Recent changes and updates

### Architecture & Development
- [Frontend Documentation](docs/application/README.md)
- [Backend Documentation](server/README.md)
- [Frontend Development Guide](docs/application/DEVELOPMENT.md)
- [Backend Development Guide](docs/server/DEVELOPMENT.md)

### Security & Deployment
- [Security Guide](docs/SECURITY.md) - Authentication, vulnerabilities, best practices
- [Deployment Guide](docs/DEPLOYMENT.md) - Azure deployment instructions
- [Implementation Summary](docs/IMPLEMENTATION-SUMMARY.md) - Recent security fixes

### Technical Details
- [Analysis Summary](docs/ANALYSIS-SUMMARY.md) - Architecture analysis and improvements
- [API Reference](docs/server/API-REFERENCE.md) - Complete API endpoint documentation
- [Database Schema](docs/server/DATABASE.md) - Database structure and relationships
- [GraphQL Quick Reference](docs/gateway/GRAPHQL-QUICK-REFERENCE.md) - GraphQL schema
- [Logging & Monitoring](docs/server/LOGGING.md) - Serilog and Seq logging implementation

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
