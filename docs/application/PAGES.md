# Pages Documentation

This document describes all pages in the EMS frontend application, their structure, and functionality.

---

## Page Architecture

All entity pages follow a consistent three-page pattern:

| Page Type       | Purpose                                 | File Pattern      |
|-----------------|-----------------------------------------|-------------------|
| **List Page**   | Display paginated data with search/sort | `*Page.tsx`       |
| **Form Page**   | Create new or edit existing records     | `*FormPage.tsx`   |
| **Detail Page** | View record details with actions        | `*DetailPage.tsx` |

---

## Dashboard

**File:** `src/pages/Dashboard.tsx`
**Route:** `/`

The main landing page displaying key system statistics.

### Statistics Cards

| Stat               | Source              | Description                      |
|--------------------|---------------------|----------------------------------|
| Total Persons      | `totalPersons`      | Count of all persons             |
| Active Employments | `activeEmployments` | Employments with `isActive=true` |
| Schools            | `totalSchools`      | Count of all schools             |
| Positions          | `totalPositions`    | Count of all positions           |
| Salary Grades      | `totalSalaryGrades` | Count of all salary grades       |
| Items              | `totalItems`        | Count of all items               |

### Features

- Real-time data fetching from `/api/v1/reports/dashboard`
- Loading skeletons during data fetch
- Responsive grid layout (1-3 columns)
- Color-coded stat cards

### Code Pattern

```tsx
useEffect(() => {
  const loadStats = async () => {
    setIsLoading(true);
    const response = await reportsApi.apiV1ReportsDashboardGet();
    setStats(response.data);
    setIsLoading(false);
  };
  loadStats();
}, []);
```

---

## Persons Module

**Location:** `src/pages/persons/`

### PersonsPage (List)

**Route:** `/persons`

Displays all persons with AG Grid infinite scrolling.

**Features:**
- Server-side pagination with infinite scroll
- Debounced search (300ms)
- Sortable columns
- Profile image thumbnail
- Click row to view details
- "Add New Person" button

**Grid Columns:**

| Column        | Field             | Features             |
|---------------|-------------------|----------------------|
| Profile       | `profileImageUrl` | Image thumbnail      |
| Name          | `fullName`        | Sortable, searchable |
| Date of Birth | `dateOfBirth`     | Date formatted       |
| Gender        | `gender`          | Badge display        |
| Civil Status  | `civilStatus`     | Badge display        |

**Data Source Pattern:**

```tsx
const dataSource: IDatasource = {
  getRows: async (params) => {
    const page = Math.floor(params.startRow / pageSize) + 1;
    const response = await personsApi.apiV1PersonsGet(
      page, 
      pageSize, 
      debouncedSearch
    );
    params.successCallback(
      response.data.items, 
      response.data.totalCount
    );
  }
};
```

---

### PersonFormPage (Create/Edit)

**Routes:** 
- `/persons/new` - Create mode
- `/persons/:displayId/edit` - Edit mode

Handles both creation and editing of person records.

**Form Sections:**

1. **Basic Information**
   - First Name (required)
   - Middle Name
   - Last Name (required)
   - Date of Birth (required)
   - Gender (required)
   - Civil Status (required)

2. **Profile Image**
   - Uses `ProfileImageUpload` component
   - Only in edit mode

3. **Addresses** (Edit mode only)
   - Add/Edit/Delete addresses
   - Address type (Home/Business)
   - Current/Permanent flags

4. **Contacts** (Edit mode only)
   - Add/Edit/Delete contacts
   - Contact type (Work/Personal)
   - Mobile, Landline, Email, Fax

5. **Documents** (Edit mode only)
   - Uses `PersonDocuments` component

**Mode Detection:**

```tsx
const { displayId } = useParams();
const isEditMode = displayId !== undefined;
```

---

### PersonDetailPage (View)

**Route:** `/persons/:displayId`

Read-only view of a person with all related data.

**Display Sections:**

1. **Header** - Full name, Edit/Delete buttons
2. **Basic Info** - DOB, Gender, Civil Status, Created/Modified dates
3. **Profile Image** - If available
4. **Addresses** - Table of all addresses
5. **Contacts** - Table of all contacts
6. **Documents** - Using `PersonDocuments` component (read-only)

**Actions:**
- Edit → Navigate to edit page
- Delete → Confirmation dialog → API delete → Navigate to list

---

## Schools Module

**Location:** `src/pages/schools/`

### SchoolsPage (List)

**Route:** `/schools`

**Grid Columns:**

| Column      | Field        | Features              |
|-------------|--------------|-----------------------|
| School Name | `schoolName` | Sortable              |
| Status      | `isActive`   | Active/Inactive badge |
| Created     | `createdAt`  | Date formatted        |

---

### SchoolFormPage (Create/Edit)

**Routes:**
- `/schools/new`
- `/schools/:displayId/edit`

**Form Fields:**
- School Name (required)
- Is Active (checkbox)

**Nested Management (Edit mode):**
- Addresses
- Contacts

---

### SchoolDetailPage (View)

**Route:** `/schools/:displayId`

Displays school information with related addresses and contacts.

---

## Positions Module

**Location:** `src/pages/positions/`

### PositionsPage (List)

**Route:** `/positions`

**Grid Columns:**

| Column      | Field         |
|-------------|---------------|
| Title       | `titleName`   |
| Description | `description` |
| Status      | `isActive`    |

---

### PositionFormPage (Create/Edit)

**Routes:**
- `/positions/new`
- `/positions/:displayId/edit`

**Form Fields:**
- Title Name (required)
- Description
- Is Active (checkbox)

---

### PositionDetailPage (View)

**Route:** `/positions/:displayId`

---

## Salary Grades Module

**Location:** `src/pages/salary-grades/`

### SalaryGradesPage (List)

**Route:** `/salary-grades`

**Grid Columns:**

| Column         | Field             |
|----------------|-------------------|
| Grade Name     | `salaryGradeName` |
| Step           | `step`            |
| Monthly Salary | `monthlySalary`   |
| Status         | `isActive`        |

---

### SalaryGradeFormPage (Create/Edit)

**Routes:**
- `/salary-grades/new`
- `/salary-grades/:displayId/edit`

**Form Fields:**
- Grade Name (required)
- Description
- Step (1-8)
- Monthly Salary (currency)
- Is Active (checkbox)

---

### SalaryGradeDetailPage (View)

**Route:** `/salary-grades/:displayId`

---

## Items Module

**Location:** `src/pages/items/`

### ItemsPage (List)

**Route:** `/items`

**Grid Columns:**

| Column      | Field         |
|-------------|---------------|
| Item Name   | `itemName`    |
| Description | `description` |
| Status      | `isActive`    |

---

### ItemFormPage (Create/Edit)

**Routes:**
- `/items/new`
- `/items/:displayId/edit`

**Form Fields:**
- Item Name (required)
- Description
- Is Active (checkbox)

---

### ItemDetailPage (View)

**Route:** `/items/:displayId`

---

## Employments Module

**Location:** `src/pages/employments/`

### EmploymentsPage (List)

**Route:** `/employments`

**Grid Columns:**

| Column       | Field              |
|--------------|--------------------|
| Person       | `personFullName`   |
| Position     | `positionTitle`    |
| Salary Grade | `salaryGradeName`  |
| Item         | `itemName`         |
| Status       | `employmentStatus` |
| Active       | `isActive`         |

---

### EmploymentFormPage (Create/Edit)

**Routes:**
- `/employments/new`
- `/employments/:displayId/edit`

**Form Fields:**

1. **Person Selection**
   - Dropdown of all persons

2. **Position Selection**
   - Dropdown of all active positions

3. **Salary Grade Selection**
   - Dropdown of all active salary grades

4. **Item Selection**
   - Dropdown of all active items

5. **Employment Details**
   - DepEd ID
   - PSIPOP Item Number
   - TIN ID
   - GSIS ID
   - PhilHealth ID
   - Date of Original Appointment
   - Appointment Status
   - Employment Status
   - Eligibility
   - Is Active

6. **School Assignments** (Edit mode)
   - Add/Remove schools
   - Start/End dates
   - Current assignment flag

---

### EmploymentDetailPage (View)

**Route:** `/employments/:displayId`

Displays complete employment information including:
- Person information
- Position details
- Salary grade details
- Item details
- All ID numbers
- School assignments with dates

---

## Common Page Patterns

### Loading State

```tsx
if (isLoading) {
  return (
    <Box p={8}>
      <Center>
        <VStack gap={4}>
          <Spinner size="xl" color="blue.500" />
          <Text>Loading...</Text>
        </VStack>
      </Center>
    </Box>
  );
}
```

### Not Found State

```tsx
if (!data) {
  return (
    <Box p={8}>
      <Alert.Root status="warning">
        <Alert.Indicator />
        <Alert.Title>Record not found</Alert.Title>
      </Alert.Root>
      <Button onClick={() => navigate('/persons')}>
        Back to List
      </Button>
    </Box>
  );
}
```

### Error Display

```tsx
{error && (
  <Alert.Root status="error" mb={4}>
    <Alert.Indicator />
    <Alert.Title>{error}</Alert.Title>
  </Alert.Root>
)}
```

### Page Header

```tsx
<Box p={6}>
  <Flex justify="space-between" align="center" mb={6}>
    <Heading size="lg">Page Title</Heading>
    <HStack gap={2}>
      <Button colorPalette="blue" onClick={handleAction}>
        Primary Action
      </Button>
    </HStack>
  </Flex>
  {/* Page content */}
</Box>
```

---

## Navigation Patterns

### From List to Detail

```tsx
// In AG Grid
onRowClicked: (event) => {
  if (event.data) {
    navigate(`/persons/${event.data.displayId}`);
  }
}
```

### From Detail to Edit

```tsx
<Button onClick={() => navigate(`/persons/${displayId}/edit`)}>
  Edit
</Button>
```

### After Form Submit

```tsx
// After successful save
navigate(`/persons/${response.data.displayId}`);
```

### After Delete

```tsx
// After successful delete
navigate('/persons');
```

### Back Navigation

```tsx
<Button variant="ghost" onClick={() => navigate(-1)}>
  ← Back
</Button>
```
