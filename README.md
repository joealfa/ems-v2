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
│   └── tests/                                       # Unit and integration tests
├── application/                                     # Frontend Application (React/TypeScript/Vite)
│   └── src/
│       ├── api/                                     # API client (auto-generated from OpenAPI)
│       ├── components/                              # Reusable UI components
│       ├── contexts/                                # React context providers
│       ├── hooks/                                   # Custom React hooks
│       ├── pages/                                   # Page components
│       └── theme/                                   # Chakra-UI theme configuration
├── docs/                                            # Documentation
│   ├── server/                                      # Backend documentation
│   └── application/                                 # Frontend documentation
├── copilot-docs/                                    # GitHub Copilot specific documentation
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

### Frontend (React 19)
- **React** - UI framework
- **TypeScript** - Static typing
- **Vite** - Build tool
- **Chakra-UI** - Component library
- **AG Grid** - Data grid component
- **Axios** - HTTP client
- **React Router** - Client-side routing
- **OpenAPI Generator** - Auto-generated API client

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

### Frontend Setup

```bash
cd application
npm install
npm run dev
```

The application will be available at `http://localhost:5173/`

### Generate API Client

After making changes to the backend API, regenerate the frontend API client:

```bash
cd application
npm run generate-api
```

## API Standards

The API follows REST best practices:
- **Lowercase URLs** - All API routes use lowercase (e.g., `/api/v1/persons`, `/api/v1/salarygrades`)
- **Versioning** - URL-based versioning (currently v1)
- **Secure Authentication** - JWT access tokens with HttpOnly cookies for refresh tokens
- **OpenAPI/Swagger** - Complete API documentation and client generation

## Project Guidelines

See [.github/copilot-instructions.md](.github/copilot-instructions.md) for detailed coding standards and guidelines.

## Documentation

- [Frontend Documentation](docs/application/README.md)
- [Backend Documentation](docs/server/README.md)

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
