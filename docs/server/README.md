# Employee Management System - Backend API Documentation

## Table of Contents

1. [Overview](#overview)
2. [Quick Start](#quick-start)
3. [Technology Stack](#technology-stack)
4. [Architecture](#architecture)
5. [Additional Documentation](#additional-documentation)

---

## Overview

The Employee Management System (EMS) backend is a comprehensive ASP.NET Core Web API built using **Clean Architecture** principles. It provides RESTful endpoints for managing employees, persons, schools, positions, salary grades, plantilla items, and documents.

### Key Features

- **Clean Architecture** with strict separation of concerns
- **RESTful API** with versioning support (v1, v2)
- **Entity Framework Core** for data access
- **Azure Blob Storage** for document and image storage
- **OpenAPI/Swagger** documentation
- **Soft Delete** pattern for data integrity
- **Audit Trail** with created/modified tracking

---

## Quick Start

### Prerequisites

- .NET 8.0 SDK or higher
- SQL Server (LocalDB, SQL Server, or Azure SQL)
- Azure Storage Account (for document storage)
- Visual Studio 2022 or VS Code

### Configuration

1. **Update connection strings** in `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=EmployeeManagementSystem;Trusted_Connection=True;",
    "BlobStorage": "DefaultEndpointsProtocol=https;AccountName=...;AccountKey=..."
  }
}
```

2. **Apply database migrations**:

```bash
cd server
dotnet ef database update --project EmployeeManagementSystem.Infrastructure --startup-project EmployeeManagementSystem.Api
```

3. **Run the application**:

```bash
dotnet run --project EmployeeManagementSystem.Api
```

The API will be available at:
- HTTP: `http://localhost:5062`
- HTTPS: `https://localhost:7009`

### Swagger UI

Access API documentation at: `http://localhost:5062/swagger`

---

## Technology Stack

|          Technology       |        Purpose         |
|---------------------------|------------------------|
| **ASP.NET Core 8.0**      | Web API framework      |
| **Entity Framework Core** | ORM and data access    |
| **SQL Server**            | Relational database    |
| **Azure Blob Storage**    | File storage           |
| **Swagger/OpenAPI**       | API documentation      |
| **xUnit**                 | Unit testing framework |

---

## Architecture

The solution follows **Clean Architecture** with four main layers:

```
┌─────────────────────────────────────────────────────────────┐
│                    Presentation Layer                       │
│              EmployeeManagementSystem.Api                   │
│         (Controllers, Configuration, Middleware)            │
├─────────────────────────────────────────────────────────────┤
│                    Application Layer                        │
│           EmployeeManagementSystem.Application              │
│         (DTOs, Interfaces, Services, Business Logic)        │
├─────────────────────────────────────────────────────────────┤
│                   Infrastructure Layer                      │
│          EmployeeManagementSystem.Infrastructure            │
│      (Data Access, Repositories, External Services)         │
├─────────────────────────────────────────────────────────────┤
│                      Domain Layer                           │
│             EmployeeManagementSystem.Domain                 │
│               (Entities, Enums, Value Objects)              │
└─────────────────────────────────────────────────────────────┘
```

### Layer Dependencies

- **Domain** → No dependencies (innermost layer)
- **Application** → Depends on Domain
- **Infrastructure** → Depends on Application and Domain
- **API** → Depends on all layers

---

## Additional Documentation

|                Document               |               Description               |
|---------------------------------------|-----------------------------------------|
| [Domain Model](./DOMAIN-MODEL.md)     | Entities, enums, and relationships      |
| [API Reference](./API-REFERENCE.md)   | Complete API endpoint documentation     |
| [Data Transfer Objects](./DTOS.md)    | DTO definitions and mappings            |
| [Services](./SERVICES.md)             | Service layer architecture              |
| [Database](./DATABASE.md)             | Database schema and configuration       |
| [Development Guide](./DEVELOPMENT.md) | Development workflow and best practices |
