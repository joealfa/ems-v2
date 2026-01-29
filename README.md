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
- **ASP.NET Core** - Web API framework
- **Clean Architecture** - Layered approach with strict separation of concerns
- **Entity Framework Core** - ORM for SQL database
- **Azure Blob Storage** - File storage for documents
- **JWT Authentication** - Secure API authentication
- **Swagger/OpenAPI** - API documentation
- **xUnit** - Testing framework

### GraphQL Gateway
- **HotChocolate** - GraphQL server for .NET
- **Redis** - Caching layer

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

### Prerequisites
- .NET 10 SDK
- Node.js 18+ and npm
- SQL Server (LocalDB or SQL Server Express)
- Azure Storage Account (for blob storage)

### Backend Setup

```bash
cd server
dotnet restore
dotnet build
cd EmployeeManagementSystem.Api
dotnet run --launch-profile https
```

The API will be available at `https://localhost:5001/` with Swagger UI.

### Gateway Setup

```bash
cd gateway/EmployeeManagementSystem.Gateway
dotnet run --launch-profile https
```

The GraphQL Gateway will be available at `https://localhost:5003/graphql`

### Frontend Setup

```bash
cd application
npm install
npm run dev
```

The application will be available at `http://localhost:5173/`

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
┌─────────────────────────────────────────────────────────────────────┐
│                     Gateway (HotChocolate)                          │
│  ┌─────────────────────────┐    ┌──────────────────────────────┐   │
│  │  GraphQL (Query/Mutation) │    │  REST Controllers (Documents) │   │
│  └─────────────────────────┘    └──────────────────────────────┘   │
│                │ Uses NSwag ApiClient           │                   │
└─────────────────────────────────────────────────────────────────────┘
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
- **Secure Authentication** - JWT access tokens with HttpOnly cookies for refresh tokens
- **OpenAPI/Swagger** - API documentation

### GraphQL (Gateway)
- **Type-safe** - Strongly typed schema
- **Single endpoint** - All queries/mutations via `/graphql`
- **Caching** - Redis caching for improved performance

## Project Guidelines

See [.github/copilot-instructions.md](.github/copilot-instructions.md) for detailed coding standards and guidelines.

## Documentation

- [Frontend Documentation](docs/application/README.md)
- [Backend Documentation](docs/server/README.md)

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
