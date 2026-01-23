# API Integration

This document describes how the frontend application integrates with the backend API.

---

## Overview

The EMS frontend uses **OpenAPI Generator** to automatically generate TypeScript API clients from the backend's Swagger specification. This ensures type safety and keeps the frontend in sync with backend changes.

---

## Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    Frontend Application                     │
├─────────────────────────────────────────────────────────────┤
│  Components/Pages                                           │
│       │                                                     │
│       ▼                                                     │
│  ┌─────────────────┐                                        │
│  │   API Exports   │  ← src/api/index.ts                    │
│  │   (Instances)   │                                        │
│  └────────┬────────┘                                        │
│           │                                                 │
│           ▼                                                 │
│  ┌─────────────────┐    ┌─────────────────┐                 │
│  │  Generated API  │    │  Axios Config   │                 │
│  │    Classes      │◄───│   (Interceptors)│                 │
│  └────────┬────────┘    └─────────────────┘                 │
│           │                    src/api/config.ts            │
│           ▼                                                 │
│  ┌─────────────────┐                                        │
│  │  Generated      │                                        │
│  │  TypeScript     │  ← src/api/generated/                  │
│  │  Models         │                                        │
│  └─────────────────┘                                        │
└─────────────────────────────────────────────────────────────┘
                          │
                          ▼
┌─────────────────────────────────────────────────────────────┐
│                    Backend API                              │
│                 http://localhost:5062                       │
└─────────────────────────────────────────────────────────────┘
```

---

## Configuration

### Base Configuration

**File:** `src/api/config.ts`

```typescript
import axios from 'axios';

// Base URL for all API requests
export const API_BASE_URL = 'http://localhost:5062';

// Configured Axios instance
export const axiosInstance = axios.create({
  baseURL: API_BASE_URL,
});

// Request interceptor (for auth tokens, etc.)
axiosInstance.interceptors.request.use(
  (config) => {
    // Add authorization header when available
    // const token = getAuthToken();
    // config.headers.Authorization = `Bearer ${token}`;
    return config;
  },
  (error) => Promise.reject(error)
);

// Response interceptor (for error handling)
axiosInstance.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      // Handle unauthorized
      console.error('Unauthorized access');
    }
    return Promise.reject(error);
  }
);
```

---

## Generated API Client

### Generation Process

The API client is generated using OpenAPI Generator CLI:

```bash
npm run generate-api
```

This command:
1. Fetches OpenAPI spec from `http://localhost:5062/swagger/v1/swagger.json`
2. Generates TypeScript/Axios client code
3. Outputs to `src/api/generated/`

### OpenAPI Generator Configuration

**File:** `openapitools.json`

```json
{
  "$schema": "node_modules/@openapitools/openapi-generator-cli/config.schema.json",
  "spaces": 2,
  "generator-cli": {
    "version": "7.13.0"
  }
}
```

**Generation Command (in package.json):**

```json
{
  "scripts": {
    "generate-api": "openapi-generator-cli generate -i http://localhost:5062/swagger/v1/swagger.json -g typescript-axios -o src/api/generated --additional-properties=supportsES6=true,withSeparateModelsAndApi=true,modelPackage=models,apiPackage=api"
  }
}
```

---

## API Classes

### Available API Classes

|       Class       |             Base Path            |           Purpose            |
|-------------------|----------------------------------|------------------------------|
| `PersonsApi`      | `/api/v1/persons`                | Person CRUD operations       |
| `EmploymentsApi`  | `/api/v1/employments`            | Employment CRUD operations   |
| `SchoolsApi`      | `/api/v1/schools`                | School CRUD operations       |
| `PositionsApi`    | `/api/v1/positions`              | Position CRUD operations     |
| `SalaryGradesApi` | `/api/v1/salary-grades`          | Salary grade CRUD operations |
| `ItemsApi`        | `/api/v1/items`                  | Item CRUD operations         |
| `DocumentsApi`    | `/api/v1/persons/{id}/documents` | Document management          |
| `ReportsApi`      | `/api/v1/reports`                | Dashboard statistics         |

### API Instance Exports

**File:** `src/api/index.ts`

```typescript
import { Configuration } from './generated';
import {
  PersonsApi,
  EmploymentsApi,
  SchoolsApi,
  PositionsApi,
  SalaryGradesApi,
  ItemsApi,
  DocumentsApi,
  ReportsApi,
} from './generated/api';
import { axiosInstance, API_BASE_URL } from './config';

// Configuration for generated APIs
const config = new Configuration({
  basePath: API_BASE_URL,
});

// Pre-configured API instances
export const personsApi = new PersonsApi(config, API_BASE_URL, axiosInstance);
export const employmentsApi = new EmploymentsApi(config, API_BASE_URL, axiosInstance);
export const schoolsApi = new SchoolsApi(config, API_BASE_URL, axiosInstance);
export const positionsApi = new PositionsApi(config, API_BASE_URL, axiosInstance);
export const salaryGradesApi = new SalaryGradesApi(config, API_BASE_URL, axiosInstance);
export const itemsApi = new ItemsApi(config, API_BASE_URL, axiosInstance);
export const documentsApi = new DocumentsApi(config, API_BASE_URL, axiosInstance);
export const reportsApi = new ReportsApi(config, API_BASE_URL, axiosInstance);

// Re-export models and BASE_URL
export * from './generated/models';
export { API_BASE_URL };
```

---

## TypeScript Models

### Model Categories

#### Entity Response DTOs

```typescript
interface PersonResponseDto {
  displayId: number;
  firstName: string;
  lastName: string;
  middleName?: string | null;
  fullName: string;
  dateOfBirth: string;
  gender: Gender;
  civilStatus: CivilStatus;
  profileImageUrl?: string | null;
  addresses: AddressResponseDto[];
  contacts: ContactResponseDto[];
  createdAt: string;
  createdBy: string;
  modifiedAt?: string | null;
  modifiedBy?: string | null;
}
```

#### Create/Update DTOs

```typescript
interface CreatePersonDto {
  firstName: string;
  lastName: string;
  middleName?: string | null;
  dateOfBirth: string;
  gender: Gender;
  civilStatus: CivilStatus;
}

interface UpdatePersonDto extends CreatePersonDto {
  // Same fields, used for PUT requests
}
```

#### Paged Results

```typescript
interface PersonListDtoPagedResult {
  items: PersonListDto[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}
```

#### Enums

```typescript
enum Gender {
  Male = 'Male',
  Female = 'Female'
}

enum CivilStatus {
  Single = 'Single',
  Married = 'Married',
  SoloParent = 'SoloParent',
  Widow = 'Widow',
  Separated = 'Separated',
  Other = 'Other'
}

enum DocumentType {
  Pdf = 'Pdf',
  Word = 'Word',
  Excel = 'Excel',
  PowerPoint = 'PowerPoint',
  ImageJpeg = 'ImageJpeg',
  ImagePng = 'ImagePng',
  Other = 'Other'
}
```

---

## Usage Patterns

### Basic CRUD Operations

#### Get List (Paginated)

```typescript
import { personsApi, type PersonListDtoPagedResult } from '../../api';

const loadPersons = async (
  page: number, 
  pageSize: number, 
  search?: string
): Promise<PersonListDtoPagedResult> => {
  const response = await personsApi.apiV1PersonsGet(page, pageSize, search);
  return response.data;
};
```

#### Get Single Record

```typescript
import { personsApi, type PersonResponseDto } from '../../api';

const loadPerson = async (displayId: number): Promise<PersonResponseDto | null> => {
  try {
    const response = await personsApi.apiV1PersonsDisplayIdGet(displayId);
    return response.data;
  } catch (error) {
    if (axios.isAxiosError(error) && error.response?.status === 404) {
      return null;
    }
    throw error;
  }
};
```

#### Create Record

```typescript
import { personsApi, type CreatePersonDto, type PersonResponseDto } from '../../api';

const createPerson = async (data: CreatePersonDto): Promise<PersonResponseDto> => {
  const response = await personsApi.apiV1PersonsPost(data);
  return response.data;
};
```

#### Update Record

```typescript
import { personsApi, type UpdatePersonDto, type PersonResponseDto } from '../../api';

const updatePerson = async (
  displayId: number, 
  data: UpdatePersonDto
): Promise<PersonResponseDto> => {
  const response = await personsApi.apiV1PersonsDisplayIdPut(displayId, data);
  return response.data;
};
```

#### Delete Record

```typescript
import { personsApi } from '../../api';

const deletePerson = async (displayId: number): Promise<void> => {
  await personsApi.apiV1PersonsDisplayIdDelete(displayId);
};
```

---

### File Upload

#### Document Upload

```typescript
import { documentsApi, DocumentType } from '../../api';

const uploadDocument = async (
  personDisplayId: number,
  file: File,
  documentType: DocumentType,
  description?: string
): Promise<void> => {
  await documentsApi.apiV1PersonsPersonDisplayIdDocumentsPost(
    personDisplayId,
    file,
    documentType,
    description
  );
};
```

#### Profile Image Upload

```typescript
import { documentsApi } from '../../api';

const uploadProfileImage = async (
  personDisplayId: number,
  file: File
): Promise<string> => {
  const response = await documentsApi.apiV1PersonsPersonDisplayIdDocumentsProfileImagePost(
    personDisplayId,
    file
  );
  return response.data.url;
};
```

---

### File Download

```typescript
import { documentsApi, API_BASE_URL } from '../../api';

const downloadDocument = async (
  personDisplayId: number,
  documentDisplayId: number,
  fileName: string
): Promise<void> => {
  const response = await documentsApi.apiV1PersonsPersonDisplayIdDocumentsDocumentDisplayIdDownloadGet(
    personDisplayId,
    documentDisplayId,
    { responseType: 'blob' }
  );
  
  // Create download link
  const url = window.URL.createObjectURL(new Blob([response.data]));
  const link = document.createElement('a');
  link.href = url;
  link.download = fileName;
  document.body.appendChild(link);
  link.click();
  document.body.removeChild(link);
  window.URL.revokeObjectURL(url);
};
```

---

### AG Grid Server-Side Data

```typescript
import { personsApi, type PersonListDto } from '../../api';
import { IDatasource, IGetRowsParams } from 'ag-grid-community';

const createDataSource = (
  pageSize: number, 
  searchTerm: string
): IDatasource => ({
  getRows: async (params: IGetRowsParams) => {
    try {
      const page = Math.floor(params.startRow / pageSize) + 1;
      
      const response = await personsApi.apiV1PersonsGet(
        page,
        pageSize,
        searchTerm || undefined
      );
      
      const { items, totalCount } = response.data;
      
      params.successCallback(
        items,
        totalCount
      );
    } catch (error) {
      console.error('Error loading data:', error);
      params.failCallback();
    }
  }
});
```

---

## Error Handling

### Standard Error Handler

```typescript
import axios, { AxiosError } from 'axios';

interface ApiError {
  message: string;
  statusCode: number;
  details?: string[];
}

const handleApiError = (error: unknown): ApiError => {
  if (axios.isAxiosError(error)) {
    const axiosError = error as AxiosError<{ message?: string; errors?: string[] }>;
    
    return {
      message: axiosError.response?.data?.message || 'An error occurred',
      statusCode: axiosError.response?.status || 500,
      details: axiosError.response?.data?.errors
    };
  }
  
  return {
    message: 'An unexpected error occurred',
    statusCode: 500
  };
};
```

### Usage in Components

```typescript
try {
  await personsApi.apiV1PersonsPost(formData);
  navigate('/persons');
} catch (error) {
  const { message, statusCode } = handleApiError(error);
  
  if (statusCode === 400) {
    setError('Invalid data. Please check your input.');
  } else if (statusCode === 409) {
    setError('A record with this information already exists.');
  } else {
    setError(message);
  }
}
```

---

## Regenerating the API Client

When the backend API changes:

1. **Ensure backend is running** on `http://localhost:5062`
2. **Run generation command:**
   ```bash
   npm run generate-api
   ```
3. **Check for breaking changes** in generated code
4. **Update components** if needed
5. **Test affected functionality**

### Post-Generation Checklist

- [ ] New models generated correctly
- [ ] Existing imports still work
- [ ] New endpoints accessible
- [ ] Enum values match backend
- [ ] No TypeScript compilation errors
