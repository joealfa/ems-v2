# Domain Model

This document describes the domain entities, enums, and relationships in the EMS backend.

---

## Entity Hierarchy

```
BaseEntity (abstract)
├── Id: Guid
├── DisplayId: long
├── CreatedBy: string
├── CreatedAt: DateTime
├── ModifiedBy: string?
├── ModifiedAt: DateTime?
└── IsDeleted: bool

    ├── Person
    ├── Employment
    ├── School
    ├── Position
    ├── SalaryGrade
    ├── Item
    ├── Document
    ├── Address
    ├── Contact
    └── EmploymentSchool
```

---

## Core Entities

### Person

Represents an individual in the system.

| Property            | Type          | Constraints           | Description                  |
|---------------------|---------------|-----------------------|------------------------------|
| `Id`                | `Guid`        | PK                    | Internal identifier          |
| `DisplayId`         | `long`        | Unique, 12-digit      | Public identifier            |
| `FirstName`         | `string`      | Required, max 100     | First name                   |
| `LastName`          | `string`      | Required, max 100     | Last name                    |
| `MiddleName`        | `string?`     | max 100               | Middle name                  |
| `DateOfBirth`       | `DateOnly`    | Required              | Date of birth                |
| `Gender`            | `Gender`      | Required              | Gender enum                  |
| `CivilStatus`       | `CivilStatus` | Required              | Civil status enum            |
| `ProfileImageUrl`   | `string?`     | max 2048              | Azure Blob URL               |
| `FullName`          | `string`      | Computed              | Full name (computed)         |

**Relationships:**
- One-to-Many → `Addresses`
- One-to-Many → `Contacts`
- One-to-Many → `Documents`
- One-to-Many → `Employments`

---

### Employment

Represents an employment record linking a person to a position.

| Property                      | Type                | Constraints       | Description                   |
|-------------------------------|---------------------|-------------------|-------------------------------|
| `Id`                          | `Guid`              | PK                | Internal identifier           |
| `DisplayId`                   | `long`              | Unique, 12-digit  | Public identifier             |
| `DepEdId`                     | `string?`           | max 50            | DepEd Employee ID             |
| `PSIPOPItemNumber`            | `string?`           | max 50            | PSIPOP Item Number            |
| `TINId`                       | `string?`           | max 20            | Tax Identification Number     |
| `GSISId`                      | `string?`           | max 20            | GSIS ID                       |
| `PhilHealthId`                | `string?`           | max 20            | PhilHealth ID                 |
| `DateOfOriginalAppointment`   | `DateOnly?`         | -                 | Original appointment date     |
| `AppointmentStatus`           | `AppointmentStatus` | Required          | Appointment status enum       |
| `EmploymentStatus`            | `EmploymentStatus`  | Required          | Employment status enum        |
| `Eligibility`                 | `Eligibility`       | Required          | Eligibility enum              |
| `IsActive`                    | `bool`              | Default: true     | Active flag                   |
| `PersonId`                    | `Guid`              | FK, Required      | Reference to Person           |
| `PositionId`                  | `Guid`              | FK, Required      | Reference to Position         |
| `SalaryGradeId`               | `Guid`              | FK, Required      | Reference to SalaryGrade      |
| `ItemId`                      | `Guid`              | FK, Required      | Reference to Item             |

**Relationships:**
- Many-to-One → `Person`
- Many-to-One → `Position`
- Many-to-One → `SalaryGrade`
- Many-to-One → `Item`
- Many-to-Many → `Schools` (via `EmploymentSchool`)

---

### School

Represents an educational institution.

| Property      | Type     | Constraints       | Description         |
|---------------|----------|-------------------|---------------------|
| `Id`          | `Guid`   | PK                | Internal identifier |
| `DisplayId`   | `long`   | Unique, 12-digit  | Public identifier   |
| `SchoolName`  | `string` | Required, max 200 | School name         |
| `IsActive`    | `bool`   | Default: true     | Active flag         |

**Relationships:**
- One-to-Many → `Addresses`
- One-to-Many → `Contacts`
- One-to-Many → `EmploymentSchools`

---

### Position

Represents a job position/title.

| Property      | Type     | Constraints       | Description         |
|---------------|----------|-------------------|---------------------|
| `Id`          | `Guid`   | PK                | Internal identifier |
| `DisplayId`   | `long`   | Unique, 12-digit  | Public identifier   |
| `TitleName`   | `string` | Required, max 100 | Position title      |
| `Description` | `string?`| max 500           | Description         |
| `IsActive`    | `bool`   | Default: true     | Active flag         |

**Relationships:**
- One-to-Many → `Employments`

---

### SalaryGrade

Represents a salary classification with step increments.

| Property           | Type      | Constraints       | Description                    |
|--------------------|-----------|-------------------|--------------------------------|
| `Id`               | `Guid`    | PK                | Internal identifier            |
| `DisplayId`        | `long`    | Unique, 12-digit  | Public identifier              |
| `SalaryGradeName`  | `string`  | Required, max 50  | Grade name (e.g., "SG-15")     |
| `Description`      | `string?` | max 500           | Description                    |
| `Step`             | `int`     | Required, 1-8     | Step increment                 |
| `MonthlySalary`    | `decimal` | Required          | Monthly salary amount          |
| `IsActive`         | `bool`    | Default: true     | Active flag                    |

**Relationships:**
- One-to-Many → `Employments`

---

### Item

Represents a plantilla item (funded position).

| Property      | Type      | Constraints       | Description         |
|---------------|-----------|-------------------|---------------------|
| `Id`          | `Guid`    | PK                | Internal identifier |
| `DisplayId`   | `long`    | Unique, 12-digit  | Public identifier   |
| `ItemName`    | `string`  | Required, max 100 | Item name           |
| `Description` | `string?` | max 500           | Description         |
| `IsActive`    | `bool`    | Default: true     | Active flag         |

**Relationships:**
- One-to-Many → `Employments`

---

### Document

Represents a file/document attached to a person.

| Property         | Type           | Constraints           | Description            |
|------------------|----------------|-----------------------|------------------------|
| `Id`             | `Guid`         | PK                    | Internal identifier    |
| `DisplayId`      | `long`         | Unique, 12-digit      | Public identifier      |
| `FileName`       | `string`       | Required, max 255     | Original file name     |
| `FileExtension`  | `string`       | Required, max 10      | File extension         |
| `ContentType`    | `string`       | Required, max 100     | MIME type              |
| `FileSizeBytes`  | `long`         | Required              | File size in bytes     |
| `DocumentType`   | `DocumentType` | Required              | Document type enum     |
| `BlobUrl`        | `string`       | Required, max 2048    | Azure Blob URL         |
| `BlobName`       | `string`       | Required, max 500     | Blob path/name         |
| `ContainerName`  | `string`       | Required, max 100     | Container name         |
| `Description`    | `string?`      | max 500               | Description            |
| `PersonId`       | `Guid`         | FK, Required          | Reference to Person    |

**Relationships:**
- Many-to-One → `Person`

---

### Address

Represents an address for a person or school.

| Property       | Type          | Constraints                          | Description               |
|----------------|---------------|--------------------------------------|---------------------------|
| `Id`           | `Guid`        | PK                                   | Internal identifier       |
| `DisplayId`    | `long`        | Unique, 12-digit                     | Public identifier         |
| `Address1`     | `string`      | Required, max 200                    | Primary address line      |
| `Address2`     | `string?`     | max 200                              | Secondary line            |
| `Barangay`     | `string?`     | max 100                              | Barangay                  |
| `City`         | `string`      | Required, max 100                    | City/Municipality         |
| `Province`     | `string`      | Required, max 100                    | Province                  |
| `Country`      | `string`      | Required, max 100, Default: "Philippines" | Country              |
| `ZipCode`      | `string?`     | max 20                               | Postal code               |
| `IsCurrent`    | `bool`        | Default: false                       | Current address flag      |
| `IsPermanent`  | `bool`        | Default: false                       | Permanent address flag    |
| `IsActive`     | `bool`        | Default: true                        | Active flag               |
| `AddressType`  | `AddressType` | Required                             | Address type enum         |
| `PersonId`     | `Guid?`       | FK, Nullable                         | Reference to Person       |
| `SchoolId`     | `Guid?`       | FK, Nullable                         | Reference to School       |
| `FullAddress`  | `string`      | Computed                             | Formatted full address    |

**Relationships:**
- Many-to-One → `Person` (optional)
- Many-to-One → `School` (optional)

---

### Contact

Represents contact information for a person or school.

| Property      | Type          | Constraints           | Description               |
|---------------|---------------|-----------------------|---------------------------|
| `Id`          | `Guid`        | PK                    | Internal identifier       |
| `DisplayId`   | `long`        | Unique, 12-digit      | Public identifier         |
| `Mobile`      | `string?`     | max 20                | Mobile number             |
| `LandLine`    | `string?`     | max 20                | Landline number           |
| `Fax`         | `string?`     | max 20                | Fax number                |
| `Email`       | `string?`     | max 256               | Email address             |
| `IsActive`    | `bool`        | Default: true         | Active flag               |
| `ContactType` | `ContactType` | Required              | Contact type enum         |
| `PersonId`    | `Guid?`       | FK, Nullable          | Reference to Person       |
| `SchoolId`    | `Guid?`       | FK, Nullable          | Reference to School       |

**Relationships:**
- Many-to-One → `Person` (optional)
- Many-to-One → `School` (optional)

---

### EmploymentSchool

Junction table for Employment-School many-to-many relationship.

| Property       | Type       | Constraints       | Description                |
|----------------|------------|-------------------|----------------------------|
| `Id`           | `Guid`     | PK                | Internal identifier        |
| `DisplayId`    | `long`     | Unique, 12-digit  | Public identifier          |
| `EmploymentId` | `Guid`     | FK, Required      | Reference to Employment    |
| `SchoolId`     | `Guid`     | FK, Required      | Reference to School        |
| `StartDate`    | `DateOnly?`| -                 | Assignment start date      |
| `EndDate`      | `DateOnly?`| -                 | Assignment end date        |
| `IsCurrent`    | `bool`     | Default: false    | Current assignment flag    |
| `IsActive`     | `bool`     | Default: true     | Active flag                |

**Relationships:**
- Many-to-One → `Employment`
- Many-to-One → `School`

---

## Domain Enums

### Gender

```csharp
public enum Gender
{
    Male = 1,
    Female = 2
}
```

### CivilStatus

```csharp
public enum CivilStatus
{
    Single = 1,
    Married = 2,
    SoloParent = 3,
    Widow = 4,
    Separated = 5,
    Other = 99
}
```

### AddressType

```csharp
public enum AddressType
{
    Business = 1,
    Home = 2
}
```

### ContactType

```csharp
public enum ContactType
{
    Work = 1,
    Personal = 2
}
```

### AppointmentStatus

```csharp
public enum AppointmentStatus
{
    Original = 1,
    Promotion = 2,
    Transfer = 3,
    Reappointment = 4
}
```

### EmploymentStatus

```csharp
public enum EmploymentStatus
{
    Regular = 1,
    Permanent = 2
}
```

### Eligibility

```csharp
public enum Eligibility
{
    LET = 1,
    PBET = 2,
    CivilServiceProfessional = 3,
    CivilServiceSubProfessional = 4,
    Other = 99
}
```

### DocumentType

```csharp
public enum DocumentType
{
    Pdf = 1,
    Word = 2,
    Excel = 3,
    PowerPoint = 4,
    ImageJpeg = 5,
    ImagePng = 6,
    Other = 99
}
```

---

## Entity Relationship Diagram

```
                    ┌─────────────────┐
                    │     Person      │
                    ├─────────────────┤
                    │ FirstName       │
                    │ LastName        │
                    │ DateOfBirth     │
                    │ Gender          │
                    │ CivilStatus     │
                    └────────┬────────┘
                             │
         ┌───────────────────┼───────────────────┐
         │                   │                   │
         ▼                   ▼                   ▼
┌─────────────────┐ ┌─────────────────┐ ┌─────────────────┐
│    Address      │ │    Contact      │ │    Document     │
├─────────────────┤ ├─────────────────┤ ├─────────────────┤
│ Address1        │ │ Mobile          │ │ FileName        │
│ City, Province  │ │ Email           │ │ BlobUrl         │
│ AddressType     │ │ ContactType     │ │ DocumentType    │
└─────────────────┘ └─────────────────┘ └─────────────────┘
         │                   │
         │                   │
         ▼                   ▼
┌─────────────────────────────────────┐
│              School                 │
├─────────────────────────────────────┤
│ SchoolName                          │
│ IsActive                            │
└─────────────────────────────────────┘
                    │
                    ▼
┌─────────────────────────────────────────────────────────────────┐
│                         Employment                              │
├─────────────────────────────────────────────────────────────────┤
│ DepEdId, TINId, GSISId, PhilHealthId                            │
│ AppointmentStatus, EmploymentStatus, Eligibility                │
└─────────────────────────────────────────────────────────────────┘
         │              │              │              │
         ▼              ▼              ▼              ▼
┌─────────────┐ ┌─────────────┐ ┌─────────────┐ ┌─────────────┐
│   Person    │ │  Position   │ │ SalaryGrade │ │    Item     │
└─────────────┘ └─────────────┘ └─────────────┘ └─────────────┘
```

---

## Key Design Patterns

### 1. Display ID Pattern

Every entity has two identifiers:
- **`Id` (Guid)**: Internal use, database relationships
- **`DisplayId` (long)**: External use, API routes, user-facing

```csharp
// 12-digit random number generation
DisplayId = new Random().NextInt64(100000000000, 999999999999);
```

### 2. Soft Delete Pattern

Entities are never physically deleted:

```csharp
public bool IsDeleted { get; set; } = false;
```

Global query filter automatically excludes deleted records:

```csharp
modelBuilder.Entity<Person>()
    .HasQueryFilter(e => !e.IsDeleted);
```

### 3. Audit Trail Pattern

All entities track creation and modification:

```csharp
public string CreatedBy { get; set; }
public DateTime CreatedAt { get; set; }
public string? ModifiedBy { get; set; }
public DateTime? ModifiedAt { get; set; }
```

### 4. Polymorphic Association

Address and Contact can belong to either Person or School:

```csharp
public Guid? PersonId { get; set; }
public Guid? SchoolId { get; set; }
```

Only one foreign key should be set at a time.
