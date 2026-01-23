# GitHub Copilot Instructions for Employee Management System

## Project Overview

**Employee Management System** is a comprehensive full-stack application for managing employees, persons, schools, and related records:
- **Backend**: Clean Architecture ASP.NET Core Web API using Entity Framework Core, Azure Blob Storage and SQL database
- **Frontend**: Modern desktop and web application built with React, TypeScript and Vite


### Technology Stack

**Backend**:
- **ASP.NET Core** - Web API framework
- **Clean Architecture** - Layered approach with strict separation of concerns
- **Swagger/OpenAPI** - API documentation
- **Entity Framework Core** - SQL database
- **Azure Blob Storage** - File storage
- **xUnit** - Testing framework

**Frontend**:
- **React** - UI framework
- **CSS** - Styling
- **TypeScript** - Static typing
- **Vite** - Build tool
- **Chakra-UI** - Component library
- **Axios** - HTTP client
- **OpenAPI Generator** - Auto-generated API client
- **AG Grid** - Data grid component


### Folder Structure
```
ems-v2/
├── .github/                                        # GitHub configuration files and copilot instructions
├── server/                                         # Backend API (ASP.NET Core)
│   ├── EmployeeManagementSystem.Domain/            # Entities and domain logic
│   ├── EmployeeManagementSystem.Application/       # Business logic and DTOs
│   ├── EmployeeManagementSystem.Infrastructure/    # Data access and external services
│   ├── EmployeeManagementSystem.Api/               # API controllers and middleware
│   │   ├── v1/                                     # Version 1 of the API
│   │   ├── v2/                                     # Version 2 of the API
│   └── tests/
│       ├── EmployeeManagementSystem.Tests/         # Unit and integration tests
├── application/                                    # Frontend Application (React/TypeScript/Vite/Radix-UI)
│   └── src/                                        # Source code
│       ├── components/                             # Reusable UI components
├── docs/                                           # Shared documentation
│   ├── server/                                     # Backend related documentation  
│   ├── application/                                # Frontend related documentation
└── copilot-docs/                                   # Copilot specific documentation like guidelines, best practices and changes
```


### Coding Standards
- Use semicolons at the end of each statement.
- Use single quotes for strings.
- Use function based components in React.
- Use arrow functions for callbacks.


### Best Practices
- Write unit tests for all new features and bug fixes.


### UI guidelines
- A toggle is provided to switch between light and dark mode.
- Application should have a modern and clean design.
- Use Chakra-UI components for consistent styling.
- Ensure the application is responsive and works well on different screen sizes.
- Use AG Grid for displaying tabular data with features like sorting, filtering, and pagination. See reference: https://www.ag-grid.com/example-inventory/


### References
- [ASP.NET Core Documentation](https://learn.microsoft.com/en-us/aspnet/core/?view=aspnetcore-10.0)
- [React Reference](https://react.dev/reference/react)
- [TypeScript Documentation](https://www.typescriptlang.org/docs/handbook/)
- [Vite Documentation](https://vite.dev/guide/)
- [Chakra-UI Documentation](https://chakra-ui.com/docs/get-started/installation)
- [Axios Documentation](https://axios-http.com/docs/intro)
- [OpenAPI Specification](https://swagger.io/specification/)
- [Clean Architecture Principles](https://8thlight.com/blog/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Entity Framework Core Documentation](https://learn.microsoft.com/en-us/ef/core/)
- [Azure Blob Storage Documentation](https://learn.microsoft.com/en-us/azure/storage/blobs/)
- [xUnit Documentation](https://xunit.net/docs/getting-started/netcore/cmdline)
- [AG Grid Documentation](https://www.ag-grid.com/react-data-grid/getting-started/)
