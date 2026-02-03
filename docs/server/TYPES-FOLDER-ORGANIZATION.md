# Types Folder Organization Guide

## Overview
The `Types/` folder contains all GraphQL-related type definitions, organized into logical subfolders for better maintainability.

## Folder Structure

```
Types/
├── Query.cs                           # Root query type
├── Mutation.cs                        # Root mutation type
├── Configuration/                     # GraphQL configuration & custom scalars
│   ├── PascalCaseNamingConventions.cs
│   └── LongType.cs
├── Extensions/                        # Type extensions for nested resolvers
│   └── TypeExtensions.cs
└── Inputs/                           # Input types for mutations
    ├── PersonInputs.cs
    ├── EmploymentInputs.cs
    ├── SchoolInputs.cs
    ├── PositionInputs.cs
    ├── SalaryGradeInputs.cs
    ├── ItemInputs.cs
    ├── AddressInputs.cs
    ├── ContactInputs.cs
    └── DocumentInputs.cs
```

## Folder Purposes

### Root Level (`Types/`)
**Files**: `Query.cs`, `Mutation.cs`

Contains the main GraphQL entry points:
- `Query.cs` - All GraphQL queries (read operations)
- `Mutation.cs` - All GraphQL mutations (write operations)

**Why here?** These are the primary GraphQL types that clients interact with, so they should be easily discoverable at the root level.

---

### `Configuration/`
**Files**: `PascalCaseNamingConventions.cs`, `LongType.cs`

Contains GraphQL server configuration and custom scalar types:
- **PascalCaseNamingConventions.cs** - Custom naming convention that preserves PascalCase for enum values (e.g., `SoloParent` instead of `SOLO_PARENT`)
- **LongType.cs** - Custom scalar type that accepts both string and numeric input for long values

**Why separate folder?** 
- Configuration files are infrastructure concerns, not business logic
- Scalars are type definitions but not direct GraphQL operations
- Keeps root Types folder clean and focused on Query/Mutation
- Groups related configuration together

---

### `Extensions/`
**Files**: `TypeExtensions.cs`

Contains GraphQL type extensions that add resolvers to existing DTO types:
- Extensions for `PersonResponseDto`, `EmploymentResponseDto`, etc.
- Nested resolvers for complex object graphs
- Pagination type extensions

**Why separate folder?**
- Type extensions are technical plumbing, not business operations
- Large file (500+ lines) would clutter the root Types folder
- Clear separation between "what you can query" (Query.cs) vs "how types are extended" (Extensions/)
- Easier to locate and maintain all type extensions in one place

**Example**:
```csharp
[ExtendObjectType<PersonResponseDto>]
public class PersonResponseDtoExtensions
{
    public long GetDisplayId([Parent] PersonResponseDto person)
    {
        return person.DisplayId;
    }
}
```

---

### `Inputs/`
**Files**: All `*Inputs.cs` files

Contains input types used in mutations:
- Create inputs (e.g., `CreatePersonInput`)
- Update inputs (e.g., `UpdatePersonInput`)
- Upsert inputs (e.g., `UpsertAddressInput`)

**Why separate folder?**
- Input types are numerous and would clutter the root
- Logically grouped by domain entity (Person, Employment, etc.)
- Easy to find all input types for a specific entity
- Follows GraphQL best practices for input type organization

**Example**:
```csharp
[GraphQLDescription("Input for creating a new person")]
public class CreatePersonInput
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    // ...
}
```

---

## Design Principles

### 1. **Discoverability**
- Root level contains primary entry points (`Query.cs`, `Mutation.cs`)
- Subfolders group related concerns

### 2. **Separation of Concerns**
- **Business Logic**: Query.cs, Mutation.cs
- **Configuration**: Configuration/
- **Technical Extensions**: Extensions/
- **Data Transfer**: Inputs/

### 3. **Scalability**
- Each subfolder can grow independently
- Adding new inputs doesn't clutter root folder
- Type extensions are isolated from queries/mutations

### 4. **Maintainability**
- Clear location for each type of file
- Related files are grouped together
- Easy to navigate for new developers

---

## When to Add New Files

### Add to Root (`Types/`)
- ✅ New root query type (rare)
- ✅ New root mutation type (rare)
- ❌ Don't add: Extensions, inputs, configuration

### Add to `Configuration/`
- ✅ Custom scalar types (e.g., `DateTimeType`, `UrlType`)
- ✅ Custom naming conventions
- ✅ GraphQL schema configuration
- ❌ Don't add: Type extensions, input types

### Add to `Extensions/`
- ✅ Type extensions for DTOs (e.g., `[ExtendObjectType<SomeDto>]`)
- ✅ Nested resolvers for complex objects
- ❌ Don't add: Query/mutation logic, input types

### Add to `Inputs/`
- ✅ Create/Update/Upsert input types
- ✅ Filter/sort input types
- ✅ Any input DTO used in mutations
- ❌ Don't add: Response types, type extensions

---

## Benefits of This Structure

### ✅ **Clarity**
Developers immediately know where to find:
- Queries/Mutations → Root level
- Configuration → `Configuration/`
- Type Extensions → `Extensions/`
- Input Types → `Inputs/`

### ✅ **Maintainability**
- Large files (like TypeExtensions.cs with 500+ lines) don't clutter the root
- Related files are grouped together
- Easy to refactor or split files within subfolders

### ✅ **Scalability**
- Project can grow without the Types folder becoming unwieldy
- Each subfolder can have its own organizational rules
- Clear patterns for adding new files

### ✅ **Best Practices**
- Follows HotChocolate/GraphQL community conventions
- Separates "what you can do" from "how it's implemented"
- Infrastructure concerns are isolated from business logic

---

## Alternative Structures (Not Recommended)

### ❌ Flat Structure
```
Types/
├── Query.cs
├── Mutation.cs
├── TypeExtensions.cs (500+ lines!)
├── LongType.cs
├── PascalCaseNamingConventions.cs
├── PersonInputs.cs
├── EmploymentInputs.cs
└── ... (20+ files at root level)
```
**Problems**: Hard to navigate, no logical grouping, root folder becomes cluttered

### ❌ By Entity
```
Types/
├── Person/
│   ├── PersonQueries.cs
│   ├── PersonMutations.cs
│   ├── PersonInputs.cs
│   └── PersonExtensions.cs
├── Employment/
│   └── ...
```
**Problems**: Splits Query/Mutation across multiple files, harder to see all available operations

---

## Summary

The current structure balances **discoverability**, **maintainability**, and **scalability**:

1. **Root level** → Primary GraphQL operations (Query, Mutation)
2. **Configuration/** → Infrastructure and GraphQL setup
3. **Extensions/** → Technical type extensions
4. **Inputs/** → Data transfer objects for mutations

This organization makes the codebase easy to navigate and maintain as it grows.
