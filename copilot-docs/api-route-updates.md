# API Route Updates - January 2026

## Summary
Updated API routes from PascalCase to lowercase to comply with REST API best practices and added URL lowercasing configuration to ASP.NET Core.

## Backend Changes

### API Route Configuration
- **File**: `server/EmployeeManagementSystem.Api/Program.cs`
- **Change**: Added routing configuration to enforce lowercase URLs
  ```csharp
  builder.Services.AddRouting(options =>
  {
      options.LowercaseUrls = true;
      options.LowercaseQueryStrings = false; // Keep query strings as-is
  });
  ```

### URL Changes
All API routes changed from PascalCase to lowercase:
- `/api/v1/Persons` → `/api/v1/persons`
- `/api/v1/Employments` → `/api/v1/employments`
- `/api/v1/Items` → `/api/v1/items`
- `/api/v1/Positions` → `/api/v1/positions`
- `/api/v1/Schools` → `/api/v1/schools`
- `/api/v1/SalaryGrades` → `/api/v1/salarygrades`
- `/api/v1/Reports` → `/api/v1/reports`

### Controller Updates
All controllers updated to use primary constructors (C# 12 feature):
- Removed explicit constructor bodies
- Converted to primary constructor syntax with field initialization
- Updated XML documentation comments format

**Updated Files**:
- `server/EmployeeManagementSystem.Api/v1/Controllers/AuthController.cs`
- `server/EmployeeManagementSystem.Api/v1/Controllers/DevAuthController.cs`
- `server/EmployeeManagementSystem.Api/v1/Controllers/DocumentsController.cs`
- `server/EmployeeManagementSystem.Api/v1/Controllers/EmploymentsController.cs`
- `server/EmployeeManagementSystem.Api/v1/Controllers/ItemsController.cs`
- `server/EmployeeManagementSystem.Api/v1/Controllers/PersonsController.cs`
- `server/EmployeeManagementSystem.Api/v1/Controllers/PositionsController.cs`
- `server/EmployeeManagementSystem.Api/v1/Controllers/ReportsController.cs`
- `server/EmployeeManagementSystem.Api/v1/Controllers/SalaryGradesController.cs`
- `server/EmployeeManagementSystem.Api/v1/Controllers/SchoolsController.cs`

### Service Layer Updates
All services updated to use primary constructors:
- `server/EmployeeManagementSystem.Application/Services/DocumentService.cs`
- `server/EmployeeManagementSystem.Application/Services/EmploymentService.cs`
- `server/EmployeeManagementSystem.Application/Services/ItemService.cs`
- `server/EmployeeManagementSystem.Application/Services/PersonService.cs`
- `server/EmployeeManagementSystem.Application/Services/PositionService.cs`
- `server/EmployeeManagementSystem.Application/Services/ReportsService.cs`
- `server/EmployeeManagementSystem.Application/Services/SalaryGradeService.cs`
- `server/EmployeeManagementSystem.Application/Services/SchoolService.cs`

### Infrastructure Layer Updates
- `server/EmployeeManagementSystem.Infrastructure/Data/ApplicationDbContext.cs`
- `server/EmployeeManagementSystem.Infrastructure/Repositories/Repository.cs`
- `server/EmployeeManagementSystem.Infrastructure/Services/AuthService.cs`

### Domain Layer Updates
- `server/EmployeeManagementSystem.Domain/Entities/User.cs` - Updated collection initialization syntax

### Test Updates
- `server/tests/EmployeeManagementSystem.Tests/Helpers/AsyncQueryableHelper.cs`
- `server/tests/EmployeeManagementSystem.Tests/Services/EmploymentServiceTests.cs`
- `server/tests/EmployeeManagementSystem.Tests/Services/PersonServiceTests.cs`
- `server/tests/EmployeeManagementSystem.Tests/Services/SchoolServiceTests.cs`

## Frontend Changes

### Generated API Files
Re-generated using OpenAPI Generator:
- `application/src/api/generated/api/documents-api.ts`
- `application/src/api/generated/api/employments-api.ts`
- `application/src/api/generated/api/items-api.ts`
- `application/src/api/generated/api/persons-api.ts`
- `application/src/api/generated/api/positions-api.ts`
- `application/src/api/generated/api/reports-api.ts`
- `application/src/api/generated/api/salary-grades-api.ts`
- `application/src/api/generated/api/schools-api.ts`

### Key API Signature Changes

#### Documents API
**Parameter Name Changes**:
- `personDisplayId` → `displayId`

**Method Examples**:
```typescript
// Before
documentsApi.apiV1PersonsPersonDisplayIdDocumentsGet(personDisplayId, ...);

// After  
documentsApi.apiV1PersonsDisplayIdDocumentsGet(displayId, ...);
```

All DocumentsApi methods affected:
- `apiV1PersonsDisplayIdDocumentsGet`
- `apiV1PersonsDisplayIdDocumentsPost`
- `apiV1PersonsDisplayIdDocumentsDocumentDisplayIdGet`
- `apiV1PersonsDisplayIdDocumentsDocumentDisplayIdPut`
- `apiV1PersonsDisplayIdDocumentsDocumentDisplayIdDelete`
- `apiV1PersonsDisplayIdDocumentsDocumentDisplayIdDownloadGet`
- `apiV1PersonsDisplayIdDocumentsProfileImageGet`
- `apiV1PersonsDisplayIdDocumentsProfileImagePost`
- `apiV1PersonsDisplayIdDocumentsProfileImageDelete`

#### Salary Grades API
**Method Name Changes**:
- `apiV1SalaryGradesGet` → `apiV1SalarygradesGet`
- `apiV1SalaryGradesPost` → `apiV1SalarygradesPost`
- `apiV1SalaryGradesDisplayIdGet` → `apiV1SalarygradesDisplayIdGet`
- `apiV1SalaryGradesDisplayIdPut` → `apiV1SalarygradesDisplayIdPut`
- `apiV1SalaryGradesDisplayIdDelete` → `apiV1SalarygradesDisplayIdDelete`

### Updated Frontend Files

#### Components
- `application/src/components/documents/ProfileImageUpload.tsx`
  - Updated `apiV1PersonsPersonDisplayIdDocumentsProfileImagePost` → `apiV1PersonsDisplayIdDocumentsProfileImagePost`
  - Updated `apiV1PersonsPersonDisplayIdDocumentsProfileImageDelete` → `apiV1PersonsDisplayIdDocumentsProfileImageDelete`

- `application/src/components/documents/PersonDocuments.tsx`
  - Updated `apiV1PersonsPersonDisplayIdDocumentsGet` → `apiV1PersonsDisplayIdDocumentsGet`
  - Updated `apiV1PersonsPersonDisplayIdDocumentsPost` → `apiV1PersonsDisplayIdDocumentsPost`
  - Updated `apiV1PersonsPersonDisplayIdDocumentsDocumentDisplayIdDelete` → `apiV1PersonsDisplayIdDocumentsDocumentDisplayIdDelete`

#### Person Pages
- `application/src/pages/persons/PersonFormPage.tsx`
  - Updated all document API calls to use new `displayId` parameter
  - 5 method calls updated

#### Salary Grade Pages
- `application/src/pages/salary-grades/SalaryGradesPage.tsx`
  - Updated `apiV1SalaryGradesGet` → `apiV1SalarygradesGet` (2 occurrences)

- `application/src/pages/salary-grades/SalaryGradeFormPage.tsx`
  - Updated `apiV1SalaryGradesDisplayIdGet` → `apiV1SalarygradesDisplayIdGet`
  - Updated `apiV1SalaryGradesDisplayIdPut` → `apiV1SalarygradesDisplayIdPut`
  - Updated `apiV1SalaryGradesPost` → `apiV1SalarygradesPost`

- `application/src/pages/salary-grades/SalaryGradeDetailPage.tsx`
  - Updated `apiV1SalaryGradesDisplayIdGet` → `apiV1SalarygradesDisplayIdGet`
  - Updated `apiV1SalaryGradesDisplayIdDelete` → `apiV1SalarygradesDisplayIdDelete`

## Migration Impact

### Breaking Changes
- **API Routes**: All frontend applications must update their API base paths to use lowercase
- **Generated API Clients**: Any applications using the OpenAPI-generated client must regenerate their client code

### Non-Breaking Changes
- Backend code modernization (primary constructors) is backward compatible
- All functionality remains the same, only method signatures changed

## Testing Recommendations
1. Test all CRUD operations for each entity type
2. Verify document upload/download functionality
3. Test salary grade management features
4. Verify profile image upload/delete operations
5. Run integration tests to ensure API compatibility

## Benefits
- **Standards Compliance**: Follows REST API URL conventions (lowercase)
- **Code Modernization**: Uses C# 12 primary constructors
- **Consistency**: All routes follow the same naming pattern
- **Maintainability**: Cleaner, more concise code structure
