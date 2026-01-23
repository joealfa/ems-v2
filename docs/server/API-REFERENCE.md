# API Reference

This document provides complete API endpoint documentation for the EMS backend.

---

## Base URL

- **Development**: `http://localhost:5062/api/v1`
- **HTTPS Development**: `https://localhost:7009/api/v1`

## API Versioning

The API uses URL-based versioning:
- Current version: `v1`
- Base path: `/api/v1/{resource}`

---

## Common Response Codes

| Code                        | Description                   |
|-----------------------------|-------------------------------|
| `200 OK`                    | Successful GET or PUT request |
| `201 Created`               | Successful POST request       |
| `204 No Content`            | Successful DELETE request     |
| `400 Bad Request`           | Invalid request data          |
| `404 Not Found`             | Resource not found            |
| `500 Internal Server Error` | Server error                  |

---

## Pagination

All list endpoints support pagination:
| Parameter        | Type   | Default | Description                |
|------------------|--------|---------|----------------------------|
| `page`           | int    | 1       | Page number (1-based)      |
| `pageSize`       | int    | 10      | Items per page (max 100)   |
| `search`         | string | null    | Search filter              |
| `sortBy`         | string | null    | Sort field                 |
| `sortDescending` | bool   | false   | Sort direction             |

### Paginated Response Structure

```json
{
  "items": [...],
  "totalCount": 100,
  "page": 1,
  "pageSize": 10,
  "totalPages": 10,
  "hasNextPage": true,
  "hasPreviousPage": false
}
```

---

## Persons API

**Base Path**: `/api/v1/persons`

### Get All Persons

```http
GET /api/v1/persons
```

**Query Parameters**: page, pageSize, search

**Response**: `PagedResult<PersonListDto>`

### Get Person by DisplayId

```http
GET /api/v1/persons/{displayId}
```

**Response**: `PersonResponseDto`

### Create Person

```http
POST /api/v1/persons
Content-Type: application/json
```

**Request Body**:
```json
{
  "firstName": "string",
  "lastName": "string",
  "middleName": "string",
  "dateOfBirth": "2024-01-01",
  "gender": "Male",
  "civilStatus": "Single"
}
```

**Response**: `201 Created` with `PersonResponseDto`

### Update Person

```http
PUT /api/v1/persons/{displayId}
Content-Type: application/json
```

**Request Body**: Same as Create

**Response**: `PersonResponseDto`

### Delete Person

```http
DELETE /api/v1/persons/{displayId}
```

**Response**: `204 No Content`

---

## Employments API

**Base Path**: `/api/v1/employments`

### Get All Employments

```http
GET /api/v1/employments
```

**Response**: `PagedResult<EmploymentListDto>`

### Get Employment by DisplayId

```http
GET /api/v1/employments/{displayId}
```

**Response**: `EmploymentResponseDto`

### Create Employment

```http
POST /api/v1/employments
Content-Type: application/json
```

**Request Body**:
```json
{
  "personDisplayId": 123456789012,
  "positionDisplayId": 123456789013,
  "salaryGradeDisplayId": 123456789014,
  "itemDisplayId": 123456789015,
  "depEdId": "string",
  "psipopItemNumber": "string",
  "tinId": "string",
  "gsisId": "string",
  "philHealthId": "string",
  "dateOfOriginalAppointment": "2024-01-01",
  "appointmentStatus": "Original",
  "employmentStatus": "Permanent",
  "eligibility": "LET",
  "isActive": true
}
```

**Response**: `201 Created` with `EmploymentResponseDto`

### Update Employment

```http
PUT /api/v1/employments/{displayId}
Content-Type: application/json
```

**Request Body**: Same as Create

**Response**: `EmploymentResponseDto`

### Delete Employment

```http
DELETE /api/v1/employments/{displayId}
```

**Response**: `204 No Content`

### Add School Assignment

```http
POST /api/v1/employments/{displayId}/schools
Content-Type: application/json
```

**Request Body**:
```json
{
  "schoolDisplayId": 123456789012,
  "startDate": "2024-01-01",
  "endDate": null,
  "isCurrent": true
}
```

**Response**: `201 Created` with `EmploymentSchoolResponseDto`

### Remove School Assignment

```http
DELETE /api/v1/employments/{displayId}/schools/{schoolDisplayId}
```

**Response**: `204 No Content`

---

## Schools API

**Base Path**: `/api/v1/schools`

### Get All Schools

```http
GET /api/v1/schools
```

**Response**: `PagedResult<SchoolListDto>`

### Get School by DisplayId

```http
GET /api/v1/schools/{displayId}
```

**Response**: `SchoolResponseDto`

### Create School

```http
POST /api/v1/schools
Content-Type: application/json
```

**Request Body**:
```json
{
  "schoolName": "string",
  "isActive": true,
  "addresses": [
    {
      "address1": "string",
      "address2": "string",
      "barangay": "string",
      "city": "string",
      "province": "string",
      "country": "Philippines",
      "zipCode": "string",
      "addressType": "Business",
      "isCurrent": true,
      "isPermanent": false,
      "isActive": true
    }
  ],
  "contacts": [
    {
      "mobile": "string",
      "landLine": "string",
      "fax": "string",
      "email": "string",
      "contactType": "Work",
      "isActive": true
    }
  ]
}
```

**Response**: `201 Created` with `SchoolResponseDto`

### Update School

```http
PUT /api/v1/schools/{displayId}
Content-Type: application/json
```

**Request Body**: Same as Create

**Response**: `SchoolResponseDto`

### Delete School

```http
DELETE /api/v1/schools/{displayId}
```

**Response**: `204 No Content`

---

## Positions API

**Base Path**: `/api/v1/positions`

### Get All Positions

```http
GET /api/v1/positions
```

**Response**: `PagedResult<PositionResponseDto>`

### Get Position by DisplayId

```http
GET /api/v1/positions/{displayId}
```

**Response**: `PositionResponseDto`

### Create Position

```http
POST /api/v1/positions
Content-Type: application/json
```

**Request Body**:
```json
{
  "titleName": "string",
  "description": "string",
  "isActive": true
}
```

**Response**: `201 Created` with `PositionResponseDto`

### Update Position

```http
PUT /api/v1/positions/{displayId}
Content-Type: application/json
```

**Response**: `PositionResponseDto`

### Delete Position

```http
DELETE /api/v1/positions/{displayId}
```

**Response**: `204 No Content`

---

## Salary Grades API

**Base Path**: `/api/v1/salary-grades`

### Get All Salary Grades

```http
GET /api/v1/salary-grades
```

**Response**: `PagedResult<SalaryGradeResponseDto>`

### Get Salary Grade by DisplayId

```http
GET /api/v1/salary-grades/{displayId}
```

**Response**: `SalaryGradeResponseDto`

### Create Salary Grade

```http
POST /api/v1/salary-grades
Content-Type: application/json
```

**Request Body**:
```json
{
  "salaryGradeName": "SG-15",
  "description": "string",
  "step": 1,
  "monthlySalary": 35000.00,
  "isActive": true
}
```

**Response**: `201 Created` with `SalaryGradeResponseDto`

### Update Salary Grade

```http
PUT /api/v1/salary-grades/{displayId}
Content-Type: application/json
```

**Response**: `SalaryGradeResponseDto`

### Delete Salary Grade

```http
DELETE /api/v1/salary-grades/{displayId}
```

**Response**: `204 No Content`

---

## Items API

**Base Path**: `/api/v1/items`

### Get All Items

```http
GET /api/v1/items
```

**Response**: `PagedResult<ItemResponseDto>`

### Get Item by DisplayId

```http
GET /api/v1/items/{displayId}
```

**Response**: `ItemResponseDto`

### Create Item

```http
POST /api/v1/items
Content-Type: application/json
```

**Request Body**:
```json
{
  "itemName": "string",
  "description": "string",
  "isActive": true
}
```

**Response**: `201 Created` with `ItemResponseDto`

### Update Item

```http
PUT /api/v1/items/{displayId}
Content-Type: application/json
```

**Response**: `ItemResponseDto`

### Delete Item

```http
DELETE /api/v1/items/{displayId}
```

**Response**: `204 No Content`

---

## Documents API

**Base Path**: `/api/v1/persons/{personDisplayId}/documents`

### Get All Documents for Person

```http
GET /api/v1/persons/{personDisplayId}/documents
```

**Response**: `PagedResult<DocumentListDto>`

### Get Document by DisplayId

```http
GET /api/v1/persons/{personDisplayId}/documents/{documentDisplayId}
```

**Response**: `DocumentResponseDto`

### Upload Document

```http
POST /api/v1/persons/{personDisplayId}/documents
Content-Type: multipart/form-data
```

**Form Fields**:
- `file`: File (required)
- `documentType`: DocumentType enum (required)
- `description`: string (optional)

**Allowed File Types**: `.pdf`, `.doc`, `.docx`, `.xls`, `.xlsx`, `.ppt`, `.pptx`, `.jpg`, `.jpeg`, `.png`

**Max File Size**: 50 MB

**Response**: `201 Created` with `DocumentResponseDto`

### Update Document Metadata

```http
PUT /api/v1/persons/{personDisplayId}/documents/{documentDisplayId}
Content-Type: application/json
```

**Request Body**:
```json
{
  "description": "string",
  "documentType": "Pdf"
}
```

**Response**: `DocumentResponseDto`

### Download Document

```http
GET /api/v1/persons/{personDisplayId}/documents/{documentDisplayId}/download
```

**Response**: File stream with appropriate content type

### Delete Document

```http
DELETE /api/v1/persons/{personDisplayId}/documents/{documentDisplayId}
```

**Response**: `204 No Content`

### Upload Profile Image

```http
POST /api/v1/persons/{personDisplayId}/documents/profile-image
Content-Type: multipart/form-data
```

**Form Fields**:
- `file`: Image file (required)

**Allowed File Types**: `.jpg`, `.jpeg`, `.png`

**Max File Size**: 5 MB

**Response**:
```json
{
  "url": "https://storage.blob.core.windows.net/..."
}
```

### Delete Profile Image

```http
DELETE /api/v1/persons/{personDisplayId}/documents/profile-image
```

**Response**: `204 No Content`

---

## Reports API

**Base Path**: `/api/v1/reports`

### Get Dashboard Statistics

```http
GET /api/v1/reports/dashboard
```

**Response**:
```json
{
  "totalPersons": 150,
  "activeEmployments": 120,
  "totalSchools": 25,
  "totalPositions": 10,
  "totalSalaryGrades": 30,
  "totalItems": 50
}
```

---

## Error Responses

### Validation Error (400)

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "FirstName": ["The FirstName field is required."],
    "DateOfBirth": ["The DateOfBirth field is required."]
  }
}
```

### Not Found Error (404)

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
  "title": "Not Found",
  "status": 404,
  "detail": "Person with DisplayId 123456789012 not found."
}
```

---

## Enum Values Reference

### Gender
- `Male`
- `Female`

### CivilStatus
- `Single`
- `Married`
- `SoloParent`
- `Widow`
- `Separated`
- `Other`

### AddressType
- `Business`
- `Home`

### ContactType
- `Work`
- `Personal`

### AppointmentStatus
- `Original`
- `Promotion`
- `Transfer`
- `Reappointment`

### EmploymentStatus
- `Regular`
- `Permanent`

### Eligibility
- `LET`
- `PBET`
- `CivilServiceProfessional`
- `CivilServiceSubProfessional`
- `Other`

### DocumentType
- `Pdf`
- `Word`
- `Excel`
- `PowerPoint`
- `ImageJpeg`
- `ImagePng`
- `Other`
