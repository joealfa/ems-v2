# Employee Management System

A comprehensive full-stack application for managing employees, persons, schools, and related records.

## Project Structure

```
ems-v2/
├── server/                 # Backend API (ASP.NET Core with Clean Architecture)
├── application/            # Frontend Application (React/TypeScript/Vite)
├── docs/                   # Documentation
│   ├── server/             # Backend documentation
│   └── application/        # Frontend documentation
├── copilot-docs/           # GitHub Copilot specific documentation
└── .github/                # GitHub configuration and copilot instructions
```

## Technology Stack

### Backend
- **ASP.NET Core** - Web API framework
- **Clean Architecture** - Layered approach with strict separation of concerns
- **Entity Framework Core** - ORM for SQL database
- **Azure Blob Storage** - File storage
- **Swagger/OpenAPI** - API documentation
- **xUnit** - Testing framework

### Frontend
- **React** - UI framework
- **TypeScript** - Static typing
- **Vite** - Build tool
- **Chakra-UI** - Component library
- **Axios** - HTTP client
- **CSS** - Styling

## Getting Started

### Prerequisites
- .NET 8.0 SDK or later
- Node.js 18+ and npm
- SQL Server (LocalDB or SQL Server Express)
- Azure Storage Account (for blob storage)

### Backend Setup

```bash
cd server
dotnet restore
dotnet build
```

### Frontend Setup

```bash
cd application
npm install
npm run dev
```

## Project Guidelines

See [.github/copilot-instructions.md](.github/copilot-instructions.md) for detailed coding standards and guidelines.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
